using System;
using Microsoft.Extensions.Configuration;

namespace Soundr.Tidal
{
    public class TidalPlayer
    {
        public TidalPlayer(IConfiguration config)
        {
            var tidalPassword = config["TidalPassword"];
        }
    }
}