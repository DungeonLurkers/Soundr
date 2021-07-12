using System;

namespace Soundr.Commons.Models
{
    public record MusicPlayerEvent;

    public record SongAdded(PlaylistEntry Entry) : MusicPlayerEvent;

    public record SongRemoved(PlaylistEntry Entry) : MusicPlayerEvent;
    public record Ready : MusicPlayerEvent;

    public record Loading(string Uri) : MusicPlayerEvent;

    public record StartPlaying(PlaylistEntry Entry) : MusicPlayerEvent;

    public record Paused : MusicPlayerEvent;

    public record Resumed : MusicPlayerEvent;

    public record Stopped : MusicPlayerEvent;

}