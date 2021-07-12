using YoutubeExplode.Videos.Streams;

namespace Soundr.YouTube.Models
{
    public class CachedAudioOnlyStreamInfo : AudioOnlyStreamInfo
    {
        public CachedAudioOnlyStreamInfo(string videoId, string url, Container container, FileSize size, Bitrate bitrate, string audioCodec) : base(url, container, size, bitrate, audioCodec)
        {
            VideoId = videoId;
        }

        public string VideoId { get; }

        public CachedAudioOnlyStreamInfo(string videoId, IAudioStreamInfo info) : base(info.Url, info.Container, info.Size, info.Bitrate, info.AudioCodec)
        {
            VideoId = videoId;
        }
        
        
    }
}