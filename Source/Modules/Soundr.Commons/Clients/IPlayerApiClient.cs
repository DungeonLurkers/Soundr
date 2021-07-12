using System.Threading.Tasks;
using RestEase;
using Soundr.Commons.Models;

namespace Soundr.Commons.Clients
{
    public interface IPlayerApiClient
    {
        [Get("/player/playlist")]
        Task<PlaylistDto> GetPlaylist();
        
        [Get("/player/current")]
        Task<PlaylistEntry?> GetCurrentSong();

        [Post("/player/add")]
        Task Add([Query] string videoId);

        [Get("/player/play")]
        Task Play();

        [Get("/player/pause")]
        Task Pause();

        [Get("/player/resume")]
        Task Resume();

        [Get("/player/stop")]
        Task Stop();
    }
}