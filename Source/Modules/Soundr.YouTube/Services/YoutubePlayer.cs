using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.Utils;
using NAudio.Wave;
using Soundr.Commons.Attributes;
using Soundr.Commons.Enums;
using Soundr.Commons.Models;
using Soundr.YouTube.Extensions;
using Soundr.YouTube.Models;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Soundr.YouTube.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class YoutubePlayer : IYoutubePlayer, IDisposable
    {
        private readonly ILogger<YoutubePlayer> _logger;
        private readonly ICacheService _cacheService;
        private YoutubeClient _ytClient;
        private WaveOutEvent _outputDevice;
        private MediaFoundationReader _audioFile;
        private ISubject<MusicPlayerEvent> _playerEventSubject;
        private ConcurrentDictionary<VideoId, AudioOnlyStreamInfo> _cache = new();
        public MusicPlayerState PlayerState { get; private set; }

        public YoutubePlayer(ILogger<YoutubePlayer> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _ytClient = new YoutubeClient();
            _playerEventSubject = new BehaviorSubject<MusicPlayerEvent>(new Stopped());

            _outputDevice = new WaveOutEvent();
            
            _outputDevice.PlaybackStopped += (_, _) =>
            {
                _logger.LogTrace($"Playback stopped");
                _playerEventSubject.OnNext(new Stopped());
                _playerEventSubject.OnNext(new Ready());
            };

            _playerEventSubject.OnNext(new Ready());
        }
        public IObservable<MusicPlayerEvent> PlayerEventObservable => _playerEventSubject.AsObservable();
        public PlaybackState PlaybackState => _outputDevice.PlaybackState;

        public async Task<PlaylistEntry> Load(string videoUrl)
        {
            _logger.LogTrace($"Load({videoUrl})");
            var videoId = VideoId.Parse(videoUrl);
            _playerEventSubject.OnNext(new Loading(videoUrl));
            var audioInfo = await GetAudioInfo(videoUrl);

            _logger.LogTrace($"Found video {audioInfo.Title} with duration {audioInfo.Duration}");

            _logger.LogDebug($"Searching cache for {audioInfo}");
            AudioOnlyStreamInfo audioStream;
            if(_cache.TryGetValue(videoId, out var cachedStream))
            {
                _logger.LogDebug("Using cached stream info");
                audioStream = cachedStream;
            }
            else
            {
                _logger.LogDebug("Selecting stream");
                var streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(videoId);

                if (streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate() is not AudioOnlyStreamInfo stream) throw new InvalidOperationException("GetAudioOnlyStreams found not only audio only audio stream");
                _cache[videoId] = stream;
                audioStream = stream;
            }

            _logger.LogDebug($"Using audio stream {audioStream.Container.Name} - {audioStream.AudioCodec} at {audioStream.Url}");

            _logger.LogTrace("Passing audio stream to NAudio");
            _audioFile = new MediaFoundationReader(audioStream.Url);

            return audioInfo;
        }

        public async Task Play(string videoIdOrUrl)
        {
            _logger.LogTrace($"Play");
            var audioInfo = await Load(videoIdOrUrl);
            _logger.LogDebug("Initializing output device");
            _outputDevice.Init(_audioFile);
            
            _logger.LogDebug($"Playing audio from {videoIdOrUrl}");
             _playerEventSubject.OnNext(new StartPlaying(audioInfo));
              _outputDevice.Play();
        }
        
        public Task Pause()
        {
            _logger.LogTrace($"Pause");
            _playerEventSubject.OnNext(new Paused());
            _outputDevice?.Pause();

            return Task.CompletedTask;
        }
        
        public Task Resume()
        {
            _logger.LogTrace($"Resume");
            _playerEventSubject.OnNext(new Resumed());
            _outputDevice?.Play();
            
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            _logger.LogTrace($"Stop");
            _outputDevice?.Stop();
            
            return Task.CompletedTask;
        }

        public async Task<PlaylistEntry> GetAudioInfo(string uri)
        {
            _logger.LogTrace($"GetAudioInfo");
            var videoId = VideoId.Parse(uri);
            _logger.LogDebug($"Searching for video with id {videoId} in cache");
            if (_cacheService.TryGetValue<PlaylistEntry>(x => x.Id.Equals(videoId.Value)) is {} entry)
            {
                _logger.LogDebug($"Found entry in cache for videoId {videoId}");
                return entry;
            }
            
            var video = await _ytClient.Videos.GetAsync(videoId);
            _logger.LogTrace($"Found video {{{video.Title}}}");
            var thumbnail = video.Thumbnails.OrderBy(x => x.Resolution.Area).First();

            var playlistEntry = new PlaylistEntry(videoId.Value, video.Title, video.Duration?.ToString("g") ?? string.Empty, thumbnail.Url, uri);

            _cacheService.Insert(playlistEntry);
            return playlistEntry;
        }

        public void SetVolume(float volume)
        {
            _logger.LogTrace($"SetVolume({volume})");
            _outputDevice.Volume = volume;
        }
        
        public float GetVolume()
        {
            _logger.LogTrace($"GetVolume"); 
            return _outputDevice.Volume;
        }

        public void JumpTo(TimeSpan position)
        {
            _logger.LogTrace($"JumpTo({position})");
            _audioFile.SetPosition(position);
        }

        public void Dispose()
        {
            _logger.LogTrace($"Dispose");
            if(_outputDevice?.PlaybackState is not PlaybackState.Stopped) _outputDevice?.Stop();
            _outputDevice?.Dispose();
            
            _audioFile?.Dispose();
        }
    }
}