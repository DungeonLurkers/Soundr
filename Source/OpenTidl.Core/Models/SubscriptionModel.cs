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
    public class SubscriptionModel : ModelBase
    {
        [DataMember(Name = "daysInPeriod")]
        public int DaysInPeriod { get; private set; }

        [DataMember(Name = "id")]
        public int Id { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "price")]
        public int Price { get; private set; }

        [DataMember(Name = "offlineGracePeriod")]
        public int OfflineGracePeriod { get; private set; }

        [IgnoreDataMember]
        public SubscriptionType Type { get; private set; }


        #region enum helpers

        [DataMember(Name = "type")]
        private string TypeEnumHelper
        {
            get { return Type.ToString(); }
            set { Type = RestUtility.ParseEnum<SubscriptionType>(value); }
        }

        #endregion
    }
}
