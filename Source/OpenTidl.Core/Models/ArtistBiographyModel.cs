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
    public class ArtistBiographyModel : ModelBase
    {
        [DataMember(Name = "source")]
        public string Source { get; private set; }

        [DataMember(Name = "text")]
        public string Text { get; private set; }


        [DataMember(Name = "summary")]
        public string Summary { get; private set; }

        [IgnoreDataMember]
        public DateTime? LastUpdated { get; private set; }


        #region json helpers

        [DataMember(Name = "lastUpdated")]
        private string LastUpdatedDateHelper
        {
            get { return RestUtility.FormatDate(LastUpdated); }
            set { LastUpdated = RestUtility.ParseDate(value); }
        }

        #endregion
    }
}
