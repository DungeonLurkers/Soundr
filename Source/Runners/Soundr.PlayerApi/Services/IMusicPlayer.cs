using System;
using System.Threading.Tasks;
using Soundr.Commons.Enums;
using Soundr.Commons.Models;

namespace Soundr.PlayerApi.Services
{
    public interface IMusicPlayer
    {
        IObservable<MusicPlayerEvent> MusicPlayerEventObservable { get; }
        Task Add(string uriOrId);
        Task Play();
        Task Pause();
        Task Resume();
        Task Stop();

        PlaylistDto GetCurrentPlaylist();
        PlaylistEntry? GetCurrentSong();

        void SetVolume(float volume);
        float GetVolume();

        Task<PlaylistEntry?> JumpNext();
        Task<PlaylistEntry?> JumpPrevious();

        Task<PlaylistEntry?> JumpTo(int index);
        Task JumpTo(TimeSpan position);
    }
}