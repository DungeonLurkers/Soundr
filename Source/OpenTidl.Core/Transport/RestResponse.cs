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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using OpenTidl.Core.Models.Base;

namespace OpenTidl.Core.Transport
{
    public class RestResponse<T> where T : ModelBase
    {
        #region properties

        public T Model { get; private set; }
        public int StatusCode { get; private set; }
        public Exception Exception { get; internal set; }

        #endregion


        #region methods
        
        private TModel DeserializeObject<TModel>(string data) where TModel : class
        {
            if (string.IsNullOrEmpty(data))
                return Activator.CreateInstance<TModel>();
            var serializer = new DataContractJsonSerializer(typeof(TModel));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return serializer.ReadObject(ms) as TModel;
            }
        }

        #endregion


        #region construction

        public RestResponse(string responseData, int statusCode, string eTag)
        {
            if (statusCode < 300)
                this.Model = DeserializeObject<T>(responseData);
            if (statusCode >= 400)
                this.Exception = new OpenTidlException(DeserializeObject<ErrorModel>(responseData));
            this.StatusCode = statusCode;
            if (this.Model != null)
                (this.Model as ModelBase).ETag = eTag;
        }

        public RestResponse(Exception ex)
        {
            this.Exception = ex;
        }

        #endregion
    }
}
