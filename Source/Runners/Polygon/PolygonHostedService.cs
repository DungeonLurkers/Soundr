using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenTidl;
using OpenTidl.Core;

namespace Polygon
{
    public class PolygonHostedService : IHostedService
    {
        private readonly IConfiguration _configuration;

        public PolygonHostedService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var tidalPassword = _configuration["TidalPassword"];
            var client = new OpenTidlClient(ClientConfiguration.Default);

            await client.LoginWithUsername("andrzej.piotrowski76@gmail.com", tidalPassword);


            var searchResult = await client.SearchTracks("Three Days Grace - Riot");


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}