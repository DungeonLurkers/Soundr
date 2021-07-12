using System;

namespace Soundr.Commons.Models
{
    public record PlaylistEntry(string Id, string Title, string Duration, string ThumbnailUri, string Uri);
}