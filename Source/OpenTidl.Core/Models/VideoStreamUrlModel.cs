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
using OpenTidl.Core.Models.Base;

namespace OpenTidl.Core.Models
{
    [DataContract]
    public class VideoStreamUrlModel : ModelBase
    {
        /*[IgnoreDataMember]
        public VideoQuality VideoQuality { get; private set; }*/

        [DataMember(Name = "url")]
        public string Url { get; private set; }


        #region enum helpers

        /*[DataMember(Name = "videoQuality")]
        private String VideoQualityEnumHelper
        {
            get { return VideoQuality.ToString(); }
            set { VideoQuality = RestUtility.ParseEnum<VideoQuality>(value); }
        }*/

        #endregion
    }
}
