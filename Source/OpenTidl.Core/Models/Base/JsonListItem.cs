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
using OpenTidl.Core.Transport;

namespace OpenTidl.Core.Models.Base
{
    [DataContract]
    public class JsonListItem<T>
    {
        [IgnoreDataMember]
        public DateTime? Created { get; private set; }

        [DataMember(Name = "item")]
        public T Item { get; private set; }


        #region json helpers

        [DataMember(Name = "created")]
        private string CreatedDateHelper
        {
            get { return RestUtility.FormatDate(Created); }
            set { Created = RestUtility.ParseDate(value); }
        }

        #endregion
    }
}
