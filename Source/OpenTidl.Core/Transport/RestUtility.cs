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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace OpenTidl.Core.Transport
{
    internal static class RestUtility
    {
        internal static string GetFormEncodedString(object data)
        {
            if (data == null)
                return null;
            return string.Join("&", data.GetType().GetProperties().Select(o =>
                string.Format("{0}={1}", WebUtility.UrlEncode(o.Name), FormEncode(o.GetValue(data)))));
        }

        private static string FormEncode(object value)
        {
            if (value == null)
                return string.Empty;
            return WebUtility.UrlEncode(value.ToString());
        }

        internal static string FormatUrl(string format, object data)
        {
            var dict = data == null ? new Dictionary<string, string>() : 
                data.GetType().GetProperties().ToDictionary(o => o.Name, o => FormEncode(o.GetValue(data)));
            return Regex.Replace(format, @"\{(\w+)\}", (m) => {
                return dict.ContainsKey(m.Groups[1].Value) ? dict[m.Groups[1].Value] : "";
            }, RegexOptions.Singleline);
        }

        internal static T ParseEnum<T>(string value) where T : struct
        {
            T result;
            if (Enum.TryParse(value, true, out result))
                return result;
            return (T)Enum.ToObject(typeof(T), 0);
            //return Enum.GetValues(typeof(T)).OfType<T>().FirstOrDefault();
        }

        internal static string FormatDate(DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            return "";
        }

        internal static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            DateTime date;
            if (DateTime.TryParse(value, out date))
                return date;
            return null;
        }

        internal static bool ParseImageSize(string sizeStr, out int width, out int height)
        {
            width = 0;
            height = 0;
            var match = Regex.Match(sizeStr ?? "", @"_(?<w>\d+)x(?<h>\d+)$", RegexOptions.Singleline);
            if (!match.Success)
                return false;
            width = int.Parse(match.Groups["w"].Value);
            height = int.Parse(match.Groups["h"].Value);
            return true;
        }
    }
}
