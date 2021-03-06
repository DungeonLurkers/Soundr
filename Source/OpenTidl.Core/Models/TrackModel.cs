/*
    Copyright (C) 2015  Jack Fagner

    This file is part of OpenTidl.

    OpenTidl is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    OpenTidl is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with OpenTidl.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.Serialization;
using OpenTidl.Core.Models.Base;
using OpenTidl.Core.Transport;

namespace OpenTidl.Core.Models
{
    [DataContract]
    public class TrackModel : ModelBase
    {
        [DataMember(Name = "album")]
        public AlbumModel Album { get; private set; }

        [DataMember(Name = "allowStreaming")]
        public bool AllowStreaming { get; private set; }

        [DataMember(Name = "artist")]
        public ArtistModel Artist { get; private set; }

        [DataMember(Name = "artists")]
        public ArtistModel[] Artists { get; private set; }

        [DataMember(Name = "duration")]
        public int Duration { get; private set; }

        [DataMember(Name = "id")]
        public int Id { get; private set; }

        [DataMember(Name = "streamReady")]
        public bool StreamReady { get; private set; }

        [IgnoreDataMember]
        public DateTime? StreamStartDate { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "trackNumber")]
        public int TrackNumber { get; private set; }

        [DataMember(Name = "version")]
        public string Version { get; private set; }

        [DataMember(Name = "volumeNumber")]
        public int VolumeNumber { get; private set; }


        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "premiumStreamingOnly")]
        public bool PremiumStreamingOnly { get; private set; }

        [DataMember(Name = "popularity")]
        public int Popularity { get; private set; }

        [DataMember(Name = "copyright")]
        public string Copyright { get; private set; }

        [DataMember(Name = "isrc")]
        public string Isrc { get; private set; }

        [DataMember(Name = "audioQuality")]
        public string AudioQuality { get; private set; }

        #region json helpers

        [DataMember(Name = "streamStartDate")]
        private string StreamStartDateHelper
        {
            get { return RestUtility.FormatDate(StreamStartDate); }
            set { StreamStartDate = RestUtility.ParseDate(value); }
        }

        #endregion
    }
}
