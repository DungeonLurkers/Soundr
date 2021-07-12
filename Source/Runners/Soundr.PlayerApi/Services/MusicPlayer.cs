using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Newtonsoft.Json;
using Soundr.Commons.Attributes;
using Soundr.Commons.Enums;
using Soundr.Commons.Hubs.Clients;
using Soundr.Commons.Models;
using Soundr.PlayerApi.Hubs;
using Soundr.YouTube.Services;

namespace Soundr.PlayerApi.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class MusicPlayer : IMusicPlayer
    {
        private readonly IYoutubePlayer _player;

        private IObservable<PlaylistEntry> _playerObservable;
        private Task _playerTask;
        private readonly ILogger<MusicPlayer> _logger;

        private readonly List<PlaylistEntry> _playlist;
        private int _currentIndex = 0;
        private readonly ISubject<MusicPlayerEvent> _playerEventSubject;
        private int NextIndex => Math.Min(_currentIndex + 1, _playlist.Count);
        private int PreviousIndex => Math.Max(_currentIndex - 1, 0);

        public MusicPlayer(IYoutubePlayer player, ILogger<MusicPlayer> logger, IHubContext<MusicPlayerHub, IMusicPlayerClient> hubContext)
        {
            _player = player;
            _logger = logger;
            _playlist = new List<PlaylistEntry>();

            _playerEventSubject = new Subject<MusicPlayerEvent>();

            MusicPlayerEventObservable = _playerEventSubject.Merge(_player.PlayerEventObservable);

            MusicPlayerEventObservable
                .Do(e => _logger.LogTrace($"New Music Player Event: {e}"))
                .Do(async e =>
                {
                    var s = JsonConvert.SerializeObject(e, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                    await hubContext.Clients.All.OnMusicPlayerEvent(s);
                })
                .Subscribe();
        }

        public IObservable<MusicPlayerEvent> MusicPlayerEventObservable { get; private set; }

        public async Task Add(string uriOrId)
        {
            _logger.LogTrace($"Adding song from {uriOrId}");
            var playlistEntry = await _player.GetAudioInfo(uriOrId);

            _playlist.Add(playlistEntry);
            _playerEventSubject.OnNext(new SongAdded(playlistEntry));
            _logger.LogTrace("Song added");
        }

        public Task Play()
        {
            switch (_player.PlaybackState)
            {
                case PlaybackState.Paused:
                    return Resume();
                case PlaybackState.Playing:
                    _logger.LogTrace("Already playing");
                    break;
                case PlaybackState.Stopped:
                    _logger.LogTrace("Play");
                    if (!_playerTask?.IsCompleted ?? false) _playerTask?.Dispose();

                    _currentIndex = 0;
                    var newSub = Task.Factory.StartNew(Work, TaskCreationOptions.LongRunning);

                    _playerTask = newSub;
            
                    return Task.CompletedTask;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }
        
        private async Task Work()
        {
            while (_currentIndex < _playlist.Count)
            {
                var entry = _playlist.ElementAtOrDefault(_currentIndex);
                if (entry is null) return;
                _currentIndex++;
                _logger.LogDebug($"Playing {entry}");
                await _player.Play(entry.Uri);
                _logger.LogDebug($"Awaiting until player is ready");
                await _player.PlayerEventObservable.Where(state => state is Ready).FirstAsync();
                _logger.LogInformation("Player is ready. Continuing");
            }
        }

        public Task Pause()
        {
            _logger.LogTrace("Pause");
            return _player.Pause();
        }

        public Task Resume()
        {
            _logger.LogTrace("Resume");
            return _player.Resume();
        }

        public async Task Stop()
        {
            _logger.LogTrace("Stop");
            _playerTask?.Dispose();
            
            await _player.Stop();
        }

        public PlaylistDto GetCurrentPlaylist()
        {
            _logger.LogTrace("GetCurrentPlaylist");
            return new (_playlist.ToList());
        }

        public PlaylistEntry? GetCurrentSong() => _playlist.ElementAtOrDefault(_currentIndex);

        public void SetVolume(float volume) => _player.SetVolume(volume);

        public float GetVolume() => _player.GetVolume();
        public async Task<PlaylistEntry?> JumpNext()
        {
            var nextSongIndex = NextIndex;
            var nextSong = _playlist.ElementAtOrDefault(nextSongIndex);

            if (nextSong is null) return nextSong;
            await Stop();
            _currentIndex = nextSongIndex;
            
            await Play();
            return nextSong;
        }

        public async Task<PlaylistEntry?> JumpPrevious()
        {
            var previousSongIndex = PreviousIndex;
            var previousSong = _playlist.ElementAtOrDefault(previousSongIndex);

            if (previousSong is null) return previousSong;
            await Stop();
            _currentIndex = previousSongIndex;
            
            await Play();
            return previousSong;
        }

        public async Task<PlaylistEntry?> JumpTo(int index)
        {
            var song = _playlist.ElementAtOrDefault(index);

            if (song is { } entry)
            {
                await Stop();
                _currentIndex = index;
                await Play();
                return entry;
            }
            return null;
        }

        public async Task JumpTo(TimeSpan position)
        {
            switch (_player.PlaybackState)
            {
                case PlaybackState.Stopped:
                    return;
                case PlaybackState.Playing:
                    await Pause();
                    _player.JumpTo(position);
                    await Play();
                    break;
                case PlaybackState.Paused:
                    _player.JumpTo(position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}