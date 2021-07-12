using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Soundr.Commons.Attributes;
using Soundr.Commons.Enums;
using Soundr.Commons.Hubs;
using Soundr.Commons.Hubs.Clients;
using Soundr.Commons.Models;

namespace PlayerWasm.Services
{
    [Service(ServiceLifetime.Scoped)]
    public class MusicPlayerHubProxy : IMusicPlayerHubProxy, IAsyncDisposable
    {
        private readonly ILogger<MusicPlayerHubProxy> _logger;
        private HubConnection _hub;
        public MusicPlayerHubProxy(IHubConnectionBuilder hubConnectionBuilder, ILogger<MusicPlayerHubProxy> logger, NavigationManager navManager)
        {
            _logger = logger;
            _playerEventSubject = new Subject<MusicPlayerEvent>();
            _isConnectedSubject = new BehaviorSubject<bool>(false);
            
            var baseAddress = navManager.BaseUri;
            _hub = hubConnectionBuilder.WithUrl($"{baseAddress}musicplayer").WithAutomaticReconnect()
                .AddJsonProtocol()
                .Build();

            Task.Run(StartAsync);
        }

        private ISubject<MusicPlayerEvent> _playerEventSubject;
        public IObservable<MusicPlayerEvent> MusicPlayerEventObservable => _playerEventSubject.AsObservable();

        private ISubject<bool> _isConnectedSubject;
        public IObservable<bool> IsConnectedObservable => _isConnectedSubject.AsObservable(); 

        public async Task StartAsync()
        {
            _playerEventSubject
                .Do(x => _logger.LogInformation($"MusicPlayerHubProxy: new event {x}"))
                .Subscribe();
            
            await _hub.StartAsync();

            _hub.On<string>(nameof(IMusicPlayerClient.OnMusicPlayerEvent),
                s=>
                {
                    var e = JsonConvert.DeserializeObject<MusicPlayerEvent>(s, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                    _playerEventSubject.OnNext(e);
                });
        }
        public Task Add(string uriOrId) => _hub.InvokeAsync(nameof(IMusicPlayerHub.Add), uriOrId);

        public Task Play()
        {
            _logger.LogInformation("Play");
            return _hub.InvokeAsync(nameof(IMusicPlayerHub.Play));
        }

        public Task Pause()
        {
            _logger.LogInformation("Pause");

            return _hub.InvokeAsync(nameof(IMusicPlayerHub.Pause));
        }

        public Task Resume()
        {
            _logger.LogInformation("Resume");

            return _hub.InvokeAsync(nameof(IMusicPlayerHub.Resume));
        }

        public Task Stop()
        {
            _logger.LogInformation("Stop");

            
            return _hub.InvokeAsync(nameof(IMusicPlayerHub.Stop));
        }

        public Task<PlaylistDto> GetCurrentPlaylist()
        {
            _logger.LogInformation("GetCurrentPlaylist");

            return _hub.InvokeAsync<PlaylistDto>(nameof(IMusicPlayerHub.GetCurrentPlaylist));
        }

        public Task<PlaylistEntry?> GetCurrentSong()
        {
            _logger.LogInformation("GetCurrentSong");

            return _hub.InvokeAsync<PlaylistEntry?>(nameof(IMusicPlayerHub.GetCurrentSong));
        }

        public Task SetVolume(float volume)
        {
            _logger.LogInformation("SetVolume");

            return _hub.InvokeAsync<PlaylistEntry?>(nameof(IMusicPlayerHub.SetVolume), volume);
        }

        public Task<float> GetVolume()
        {
            _logger.LogInformation("GetVolume");

            return _hub.InvokeAsync<float>(nameof(IMusicPlayerHub.GetVolume));
        }

        public Task<PlaylistEntry?> JumpNext()
        {
            _logger.LogInformation("JumpNext");

            return _hub.InvokeAsync<PlaylistEntry?>(nameof(IMusicPlayerHub.JumpNext));
        }

        public Task<PlaylistEntry?> JumpPrevious()
        {
            _logger.LogInformation("JumpPrevious");

            return _hub.InvokeAsync<PlaylistEntry?>(nameof(IMusicPlayerHub.JumpPrevious));
        }

        public Task JumpTo(string position)
        {
            _logger.LogInformation($"JumpTo({position})");
            
            return _hub.InvokeAsync(nameof(IMusicPlayerHub.JumpTo), position);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hub is not null)
            {
                await _hub.DisposeAsync();
            }
        }

        private async Task Command(string methodName, object param)
        {
            _logger.LogTrace($"{methodName}({param})");
            await IsConnectedObservable.Where(x => x);
            
            await  _hub.InvokeAsync(methodName, param);
        }
        
        private async Task Command(string methodName)
        {
            _logger.LogTrace($"{methodName}");
            await IsConnectedObservable.Where(x => x);
            
            await  _hub.InvokeAsync(methodName);
        }
        
        private async Task<T> Request<T>(string methodName, object param)
        {
            _logger.LogTrace($"{methodName}({param})");
            await IsConnectedObservable.Where(x => x);
            
            return await _hub.InvokeAsync<T>(methodName, param);
        }
        
        private async Task<T> Request<T>(string methodName)
        {
            _logger.LogTrace($"{methodName}");
            await IsConnectedObservable.Where(x => x);
            
            return await _hub.InvokeAsync<T>(methodName);
        }
    }
}