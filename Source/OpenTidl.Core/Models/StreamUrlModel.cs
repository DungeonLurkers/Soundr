﻿/*
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
    public class StreamUrlModel : ModelBase
    {
        [DataMember(Name = "playTimeLeftInMinutes")]
        public int PlayTimeLeftInMinutes { get; private set; }

        [IgnoreDataMember]
        public SoundQuality SoundQuality { get; private set; }

        [IgnoreDataMember]
        public StreamResponseType StreamResponseType { get; private set; }

        [IgnoreDataMember]
        public StreamSource StreamSource { get; private set; }

        [DataMember(Name = "trackId")]
        public int TrackId { get; private set; }
        
        [DataMember(Name = "url")]
        public string Url { get; private set; }


        #region enum helpers

        [DataMember(Name = "soundQuality")]
        private string RoleEnumHelper
        {
            get { return SoundQuality.ToString(); }
            set { SoundQuality = RestUtility.ParseEnum<SoundQuality>(value); }
        }

        [DataMember(Name = "streamResponseType")]
        private string StreamResponseTypeEnumHelper
        {
            get { return StreamResponseType.ToString(); }
            set { StreamResponseType = RestUtility.ParseEnum<StreamResponseType>(value); }
        }

        [DataMember(Name = "streamSource")]
        private string StreamSourceEnumHelper
        {
            get { return StreamSource.ToString(); }
            set { StreamSource = RestUtility.ParseEnum<StreamSource>(value); }
        }

        #endregion
    }
}
