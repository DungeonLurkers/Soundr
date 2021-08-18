using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Blazorise;
using DynamicData;
using DynamicData.Binding;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using PlayerWasm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Soundr.Commons.Clients;
using Soundr.Commons.Hubs;
using Soundr.Commons.Models;

namespace PlayerWasm.ViewModels
{
    public class MusicPlayerViewModel : ReactiveObject
    {
        private readonly IMusicPlayerHubProxy _playerHub;
        private readonly ILogger<MusicPlayerViewModel> _logger;
        private readonly IMusicPlayerApiClient _apiClient;
        private IObservable<long> _playerTimerObservable;
        private IDisposable? _timerDisposable;
        private ISubject<bool> _isTimerPaused;
        public ObservableCollection<PlaylistEntry> Playlist { get; }

        [Reactive] public PlaylistEntry? Current { get; set; }
        public double CurrentSongMs => TimeSpan.Parse(Current?.Duration ?? "0:00:00").TotalMilliseconds;
        [Reactive] public string? SongUri { get; set; }
        
        [Reactive] public IconName PlayIconName {get; set; } = IconName.PlayCircle;
        [Reactive] public float Volume { get; set; } = 1f;
        [Reactive] public double PositionInMs { get; set; } = 0.0d;
        [Reactive] public bool IsSongLoading { get; set; } = false;

        public ReactiveCommand<Unit, Unit> LoadData {get;}
        public ReactiveCommand<Unit, Unit> Add { get; }
        public ReactiveCommand<Unit, Unit> Play { get; }
        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }
        public ReactiveCommand<Unit, Unit> SetVolume { get; }
        public ReactiveCommand<Unit, Unit> JumpNext { get; }
        public ReactiveCommand<Unit, Unit> JumpPrevious { get; }
        public ReactiveCommand<Unit, Unit> JumpTo { get; }
        
        public MusicPlayerViewModel(IMusicPlayerHubProxy playerHub, ILogger<MusicPlayerViewModel> logger, IMusicPlayerApiClient apiClient)
        {
            _playerHub = playerHub;
            _logger = logger;
            _apiClient = apiClient;
            _isTimerPaused = new BehaviorSubject<bool>(false);
            Playlist = new ObservableCollectionExtended<PlaylistEntry>();

            LoadData = ReactiveCommand.CreateFromTask(async _ =>
            {
                _logger.LogInformation("Loading playlist from server");
                var currentPlaylist = await _apiClient.GetPlaylist();
                _logger.LogInformation($"Loaded {currentPlaylist.Songs.Count} songs");

                Playlist.Clear();
                Playlist.AddRange(currentPlaylist.Songs);
                _logger.LogInformation("Fetching current song");
                Current = await _apiClient.GetCurrentSong();

                Volume = await _playerHub.GetVolume();
            });

            Add = ReactiveCommand.CreateFromTask(Execute);
            Play = ReactiveCommand.CreateFromTask(playerHub.Play);
            Pause = ReactiveCommand.CreateFromTask(playerHub.Pause);
            Stop = ReactiveCommand.CreateFromTask(playerHub.Stop);
            SetVolume = ReactiveCommand.CreateFromTask(() => playerHub.SetVolume(Volume));
            JumpNext = ReactiveCommand.CreateFromTask(async () =>
            {
                Current = await playerHub.JumpNext();
            });
            JumpPrevious = ReactiveCommand.CreateFromTask(async () =>
            {
                Current = await _playerHub.JumpPrevious();
            });
            JumpTo = ReactiveCommand.CreateFromTask(async () =>
            {
                var position = PositionInMs;
                var span = TimeSpan.FromMilliseconds(position);
                var spanAsString = span.ToString();
                await _playerHub.JumpTo(spanAsString);
            });

            _playerHub.MusicPlayerEventObservable.OfType<SongAdded>()
                .Do(x => _logger.LogInformation($"New event {x}"))
                .Do(x => Playlist.Add(x.Entry))
                .Subscribe();
                
            _playerHub.MusicPlayerEventObservable.OfType<SongRemoved>()
                .Do(x => _logger.LogInformation($"New event {x}"))
                .Do(x => Playlist.Remove(x.Entry))
                .Subscribe();

            _playerHub.MusicPlayerEventObservable.OfType<StartPlaying>()
                .Do(x => _logger.LogInformation($"New event {x}"))
                .Do(x => PlayIconName = IconName.PauseCircle)
                .Do(_ => PositionInMs = 0)
                .Do(_ => StartTimer())
                .Do(x => Current = x.Entry)
                .Subscribe();
            
            _playerHub.MusicPlayerEventObservable.OfType<Resumed>()
                .Do(x => _logger.LogInformation($"New event {x}"))
                .Do(x => PlayIconName = IconName.PauseCircle)
                .Do(_ => StartTimer())
                .Subscribe();

            _playerHub.MusicPlayerEventObservable.OfType<Paused>()
                .Do(x => _logger.LogInformation($"New event {x}"))
                .Do(x => PlayIconName = IconName.PlayCircle)
                .Do(_ => StopTimer())
                .Subscribe();

            _playerHub.MusicPlayerEventObservable.OfType<Stopped>()
                .Do(x => _logger.LogInformation($"New event {x}"))
                .Do(x => PlayIconName = IconName.PlayCircle)
                .Do(_ => StopTimer())
                .Subscribe();
            
            this.WhenValueChanged(x => x.Volume)
                .Throttle(TimeSpan.FromSeconds(0.125))
                .Do(async x => await _playerHub.SetVolume(x))
                .Subscribe();

            _playerTimerObservable = Observable.Interval(TimeSpan.FromMilliseconds(25))
                .Where(_ => Current is not null)
                .Do(i =>
                {
                    PositionInMs += 25;
                });
            
            Play
                .Do(_ => PlayIconName = IconName.Sync)
                .Subscribe();
        }

        private void StartTimer()
        {
            _logger.LogInformation("StartTimer");
            _isTimerPaused.OnNext(false);
            _timerDisposable = _playerTimerObservable.Subscribe();
        }
        
        private void PauseTimer()
        {
            _logger.LogInformation("PauseTimer");
            _isTimerPaused.OnNext(true);
        }

        private void StopTimer()
        {
            _logger.LogInformation("StopTimer");
            _isTimerPaused.OnNext(true);
            _timerDisposable?.Dispose();
        }

        private async Task Execute()
        {
            var uri = SongUri;
            _logger.LogInformation($"Sending {uri} to server");
            if (uri is not null) await _playerHub.Add(uri);
        }
    }
}