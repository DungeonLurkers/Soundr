using System;
using System.IO;
using System.Net;

namespace OpenTidl.Core.Models.Base
{
    public class WebStreamModel
    {
        #region properties

        public Stream Stream { get; private set; }
        public long ContentLength { get; private set; }

        #endregion


        #region methods

        public byte[] ToArray()
        {
            return this.Stream.ToArray();
        }

        #endregion


        #region construction

        internal WebStreamModel(HttpWebResponse response)
        {
            try
            {
                if (response != null)
                {
                    this.ContentLength = response.ContentLength;
                    this.Stream = response.GetResponseStream();
                }
            }
            catch { }
        }

        #endregion
    }
}
