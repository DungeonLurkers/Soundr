using System;
using NAudio.Wave;
// ReSharper disable MemberCanBePrivate.Global

namespace Soundr.YouTube.Extensions
{
    public static class WaveStreamExtensions
    {
        // Set position of WaveStream to nearest block to supplied position
        public static void SetPosition(this WaveStream stream, long position)
        {
            // distance from block boundary (may be 0)
            var adj = position % stream.WaveFormat.BlockAlign;
            // adjust position to boundary and clamp to valid range
            var newPos = Math.Max(0, Math.Min(stream.Length, position - adj));
            
            // set playback position
            stream.Position = newPos;
        }

        // Set playback position of WaveStream by seconds
        public static void SetPosition(this WaveStream stream, double seconds)
        {
            stream.SetPosition((long)(seconds * stream.WaveFormat.AverageBytesPerSecond));
        }

        // Set playback position of WaveStream by time (as a TimeSpan)
        public static void SetPosition(this WaveStream stream, TimeSpan time)
        {
            stream.SetPosition(time.TotalSeconds);
        }

        // Set playback position of WaveStream relative to current position
        public static void Seek(this WaveStream stream, double offset)
        {
            stream.SetPosition(stream.Position + (long)(offset* stream.WaveFormat.AverageBytesPerSecond));
        }
    }
}