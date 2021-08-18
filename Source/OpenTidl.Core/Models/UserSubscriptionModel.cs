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
    public class UserSubscriptionModel : ModelBase
    {
        [IgnoreDataMember]
        public SoundQuality HighestSoundQuality { get; private set; }

        [DataMember(Name = "status")]
        public string Status { get; private set; }

        [DataMember(Name = "subscription")]
        public SubscriptionModel Subscription { get; private set; }

        [IgnoreDataMember]
        public DateTime? ValidUntil { get; private set; }


        [DataMember(Name = "premiumAccess")]
        public bool PremiumAccess { get; private set; }

        [DataMember(Name = "canGetTrial")]
        public bool CanGetTrial { get; private set; }


        [IgnoreDataMember]
        public bool IsBasicSubscription
        {
            get { return this.Subscription != null && this.Subscription.Type == SubscriptionType.BASIC; }
        }

        [IgnoreDataMember]
        public bool IsFreeSubscription
        {
            get { return this.Subscription != null && this.Subscription.Type == SubscriptionType.FREE; }
        }

        [IgnoreDataMember]
        public bool IsHifiAvailable
        {
            get { return (int)this.HighestSoundQuality >= (int)SoundQuality.LOSSLESS; }
        }


        #region json helpers

        [DataMember(Name = "highestSoundQuality")]
        private string HighestSoundQualityEnumHelper
        {
            get { return HighestSoundQuality.ToString(); }
            set { HighestSoundQuality = RestUtility.ParseEnum<SoundQuality>(value); }
        }

        [DataMember(Name = "validUntil")]
        private string ValidUntilDateHelper
        {
            get { return RestUtility.FormatDate(ValidUntil); }
            set { ValidUntil = RestUtility.ParseDate(value); }
        }

        #endregion
    }
}
