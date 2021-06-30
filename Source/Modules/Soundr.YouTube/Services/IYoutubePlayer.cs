using System;
using System.Threading.Tasks;

namespace Soundr.YouTube.Services
{
    public interface IYoutubePlayer
    {
        Task Play(string videoIdOrUrl);
        Task Pause();
        Task Resume();
        Task Stop();
    }
}