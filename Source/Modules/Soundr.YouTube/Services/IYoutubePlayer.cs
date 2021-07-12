using System;
using System.Threading.Tasks;
using NAudio.Wave;
using Soundr.Commons.Enums;
using Soundr.Commons.Models;

namespace Soundr.YouTube.Services
{
    public interface IYoutubePlayer
    {
        IObservable<MusicPlayerEvent> PlayerEventObservable { get; }
        PlaybackState PlaybackState {get;}
        Task Play(string videoIdOrUrl);
        Task Pause();
        Task Resume();
        Task Stop();

        Task<PlaylistEntry> GetAudioInfo(string uri);

        void SetVolume(float volume);
        float GetVolume();

        void JumpTo(TimeSpan position);
    }
}