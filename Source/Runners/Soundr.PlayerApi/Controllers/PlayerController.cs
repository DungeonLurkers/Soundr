using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Soundr.PlayerApi.Services;
using Soundr.YouTube.Services;

namespace Soundr.PlayerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {

        private readonly ILogger<PlayerController> _logger;
        private readonly IMusicPlayer _musicPlayer;

        public PlayerController(ILogger<PlayerController> logger, IMusicPlayer musicPlayer)
        {
            _logger = logger;
            _musicPlayer = musicPlayer;
        }
        
        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            _logger.LogDebug("Get Current");
            
            return Ok(_musicPlayer.GetCurrentSong());
        }
        
        [HttpGet("playlist")]
        public IActionResult GetPlaylist()
        {
            _logger.LogDebug("Get Playlist");
            
            return Ok(_musicPlayer.GetCurrentPlaylist());
        }
        
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromQuery] string videoId)
        {
            _logger.LogDebug("Add song {videoId}");
            await _musicPlayer.Add(videoId);
            return Ok();
        }

        [HttpGet("play")]
        public async Task<IActionResult> Play()
        {
            _logger.LogDebug("Play");
            await _musicPlayer.Play();
            return Ok();
        }
        [HttpGet("pause")]
        public async Task<IActionResult> Pause()
        {
            _logger.LogDebug("Pause");
            await _musicPlayer.Pause();
            return Ok();
        }
        [HttpGet("resume")]
        public async Task<IActionResult> Resume()
        {
            _logger.LogDebug("Resume");
            await _musicPlayer.Resume();
            return Ok();
        }
        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            _logger.LogDebug("Stop");
            await _musicPlayer.Stop();
            return Ok();
        }
    }
}