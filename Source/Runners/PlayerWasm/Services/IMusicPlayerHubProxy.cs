using System;
using System.Threading.Tasks;
using Soundr.Commons.Hubs;

namespace PlayerWasm.Services
{
    public interface IMusicPlayerHubProxy : IMusicPlayerHub
    {
        IObservable<bool> IsConnectedObservable { get; }
        Task StartAsync();
    }
}