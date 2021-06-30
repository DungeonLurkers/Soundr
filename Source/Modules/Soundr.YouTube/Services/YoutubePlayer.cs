using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Soundr.Commons.Attributes;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Soundr.YouTube.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class YoutubePlayer : IYoutubePlayer, IDisposable
    {
        private readonly ILogger<YoutubePlayer> _logger;
        private YoutubeClient _ytClient;
        private WaveOut _outputDevice;
        private MediaFoundationReader _audioFile;

        public YoutubePlayer(ILogger<YoutubePlayer> logger)
        {
            _logger = logger;
            _ytClient = new YoutubeClient();
        }

        public async Task Play(string videoIdOrUrl)
        {
            var videoId = VideoId.Parse(videoIdOrUrl);
            _logger.LogDebug($"Playing video with id {videoId}");

            _logger.LogDebug($"Not found any cached videos for {videoId}");
            _logger.LogDebug("Selecting stream");
            var streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(videoId);

            if (streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate() is not AudioOnlyStreamInfo audioStream) return;
            
            _logger.LogDebug($"Using audio stream {audioStream.Container.Name} - {audioStream.AudioCodec} at {audioStream.Url}");
                
            _logger.LogDebug("Passing audio file to NAudio");
            
            _audioFile = new MediaFoundationReader(audioStream.Url);
            _outputDevice = new WaveOut();

            _logger.LogDebug("Initializing output device");
            _outputDevice.Init(_audioFile);
            
            _logger.LogDebug($"Playing audio from {videoIdOrUrl}");
            _outputDevice.Play();

        }
        
        public Task Pause()
        {
            _logger.LogDebug($"Pausing audio");
            _outputDevice?.Pause();

            return Task.CompletedTask;
        }
        
        public Task Resume()
        {
            _logger.LogDebug($"Resuming audio");
            _outputDevice?.Play();
            
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            _logger.LogDebug($"Stopping audio");
            _outputDevice?.Stop();
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if(_outputDevice?.PlaybackState is not PlaybackState.Stopped) _outputDevice?.Stop();
            _outputDevice?.Dispose();
            
            _audioFile?.Dispose();
        }
    }
}