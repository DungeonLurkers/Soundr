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
using OpenTidl.Core.Enums;
using OpenTidl.Core.Models.Base;
using OpenTidl.Core.Transport;

namespace OpenTidl.Core.Models
{
    [DataContract]
    public class VideoModel : ModelBase
    {
        [DataMember(Name = "artist")]
        public ArtistModel Artist { get; private set; }

        [DataMember(Name = "artists")]
        public ArtistModel[] Artists { get; private set; }

        [DataMember(Name = "duration")]
        public int Duration { get; private set; }

        [DataMember(Name = "id")]
        public int Id { get; private set; }

        [DataMember(Name = "imageId")]
        public string ImageId { get; private set; }

        [DataMember(Name = "imagePath")]
        public string ImagePath { get; private set; }

        [IgnoreDataMember]
        public VideoQuality Quality { get; private set; }

        [IgnoreDataMember]
        public DateTime? ReleaseDate { get; private set; }

        [DataMember(Name = "streamReady")]
        public bool StreamReady { get; private set; }

        [IgnoreDataMember]
        public DateTime? StreamStartDate { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }


        #region json helpers

        [DataMember(Name = "quality")]
        private string QualityEnumHelper
        {
            get { return Quality.ToString(); }
            set { Quality = RestUtility.ParseEnum<VideoQuality>(value); }
        }

        [DataMember(Name = "releaseDate")]
        private string ReleaseDateHelper
        {
            get { return RestUtility.FormatDate(ReleaseDate); }
            set { ReleaseDate = RestUtility.ParseDate(value); }
        }

        [DataMember(Name = "streamStartDate")]
        private string StreamStartDateHelper
        {
            get { return RestUtility.FormatDate(StreamStartDate); }
            set { StreamStartDate = RestUtility.ParseDate(value); }
        }

        #endregion
    }
}
