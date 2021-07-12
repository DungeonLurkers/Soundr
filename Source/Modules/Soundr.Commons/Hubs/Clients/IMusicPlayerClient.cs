using System.Threading.Tasks;
using Soundr.Commons.Enums;
using Soundr.Commons.Models;

namespace Soundr.Commons.Hubs.Clients
{
    public interface IMusicPlayerClient
    {
        Task OnMusicPlayerEvent(string serialized);
    }
}