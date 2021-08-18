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
    public class PlaylistModel : ModelBase
    {
        [IgnoreDataMember]
        public DateTime? Created { get; private set; }

        [DataMember(Name = "creator")]
        public CreatorModel Creator { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "duration")]
        public int Duration { get; private set; }
        
        [DataMember(Name = "image")]
        public string Image { get; private set; }

        [IgnoreDataMember]
        public DateTime? LastUpdated { get; private set; }

        [DataMember(Name = "numberOfTracks")]
        public int NumberOfTracks { get; private set; }

        [DataMember(Name = "offlineDateAdded")]
        public long OfflineDateAdded { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [IgnoreDataMember]
        public PlaylistType Type { get; private set; }

        [DataMember(Name = "uuid")]
        public string Uuid { get; private set; }



        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "publicPlaylist")]
        public bool PublicPlaylist { get; private set; }


        #region json helpers

        [DataMember(Name = "type")]
        private string TypeEnumHelper
        {
            get { return Type.ToString(); }
            set { Type = RestUtility.ParseEnum<PlaylistType>(value); }
        }

        [DataMember(Name = "created")]
        private string CreatedDateHelper
        {
            get { return RestUtility.FormatDate(Created); }
            set { Created = RestUtility.ParseDate(value); }
        }

        [DataMember(Name = "lastUpdated")]
        private string LastUpdatedDateHelper
        {
            get { return RestUtility.FormatDate(LastUpdated); }
            set { LastUpdated = RestUtility.ParseDate(value); }
        }

        #endregion
    }
}
