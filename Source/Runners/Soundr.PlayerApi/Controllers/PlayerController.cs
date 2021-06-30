using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Soundr.YouTube.Services;

namespace Soundr.PlayerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PlayerController> _logger;
        private readonly IYoutubePlayer _youtubePlayer;

        public PlayerController(ILogger<PlayerController> logger, IYoutubePlayer youtubePlayer )
        {
            _logger = logger;
            _youtubePlayer = youtubePlayer;
        }

        [HttpGet("play")]
        public async Task<IActionResult> Play([FromQuery] string videoId)
        {
            await _youtubePlayer.Play(videoId);
            return Ok();
        }
        [HttpGet("pause")]
        public async Task<IActionResult> Pause()
        {
            await _youtubePlayer.Pause();
            return Ok();
        }
        [HttpGet("resume")]
        public async Task<IActionResult> Resume()
        {
            await _youtubePlayer.Resume();
            return Ok();
        }
        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            await _youtubePlayer.Stop();
            return Ok();
        }
    }
}