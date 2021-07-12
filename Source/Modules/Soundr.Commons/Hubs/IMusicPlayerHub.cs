using System;
using System.Threading.Tasks;
using Soundr.Commons.Enums;
using Soundr.Commons.Models;

namespace Soundr.Commons.Hubs
{
    public interface IMusicPlayerHub
    {
        IObservable<MusicPlayerEvent> MusicPlayerEventObservable { get; }

        Task Add(string uriOrId);
        Task Play();
        Task Pause();
        Task Resume();
        Task Stop();

        Task<PlaylistDto> GetCurrentPlaylist();

        Task<PlaylistEntry?> GetCurrentSong();

        Task SetVolume(float volume);
        Task<float> GetVolume();

        Task<PlaylistEntry?> JumpNext();
        Task<PlaylistEntry?> JumpPrevious();
        Task JumpTo(string position);
    }
}