using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Soundr.Commons.Enums;
using Soundr.Commons.Hubs;
using Soundr.Commons.Hubs.Clients;
using Soundr.Commons.Models;
using Soundr.PlayerApi.Services;

namespace Soundr.PlayerApi.Hubs
{
    public class MusicPlayerHub : Hub<IMusicPlayerClient>, IMusicPlayerHub
    {
        private readonly IMusicPlayer _player;
        private readonly ILogger<MusicPlayerHub> _logger;

        public MusicPlayerHub(IMusicPlayer player, ILogger<MusicPlayerHub> logger)
        {
            _player = player;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            var connId = Context.ConnectionId;
            _logger.LogTrace($"New connection {connId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connId = Context.ConnectionId;
            _logger.LogTrace($"Ended connection {connId}");

            if (exception is { } e)
            {
                _logger.LogError(e, $"Error in connection {connId}");
            }
            return base.OnDisconnectedAsync(exception);
        }

        public IObservable<MusicPlayerEvent> MusicPlayerEventObservable => _player.MusicPlayerEventObservable;

        public Task Add(string uriOrId)
        {
            _logger.LogTrace("Add");
            return _player.Add(uriOrId);
        }

        public async Task Play()
        {
            _logger.LogTrace("Play");
            await _player.Play();
        }

        public async Task Pause()
        {
            _logger.LogTrace("Pause");

            await _player.Pause();
        }

        public async Task Resume()
        {
            _logger.LogTrace("Resume");
            await _player.Resume();
        }

        public async Task Stop()
        {
            _logger.LogTrace("Stop");
            await _player.Stop();
        }

        public Task<PlaylistDto> GetCurrentPlaylist()
        {
            _logger.LogTrace("GetCurrentPlaylist");
            return Task.FromResult(_player.GetCurrentPlaylist());
        }

        public Task<PlaylistEntry?> GetCurrentSong()
        {
            _logger.LogTrace("GetCurrentSong");
            return Task.FromResult(_player.GetCurrentSong());
        }

        public Task SetVolume(float volume)
        {
            _logger.LogTrace("SetVolume");
            _player.SetVolume(volume);
            return Task.CompletedTask;
        }

        public Task<float> GetVolume() => Task.FromResult(_player.GetVolume());
        public async Task<PlaylistEntry?> JumpNext()
        {
            _logger.LogTrace("JumpNext");
            return await _player.JumpNext();
        }

        public async Task<PlaylistEntry?> JumpPrevious()
        {
            _logger.LogTrace("JumpPrevious");
            return await _player.JumpPrevious(); 
        }

        public Task JumpTo(string position)
        {
            _logger.LogTrace($"JumpTo({position})");
            _player.JumpTo(TimeSpan.Parse(position));
            return Task.CompletedTask;
        }
    }
}