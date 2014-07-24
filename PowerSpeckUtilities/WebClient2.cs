using System;
using System.Net;

namespace PowerSpeckUtilities
{
    public class WebClient2 : WebClient
    {
        public WebClient2() : this(60000)
        {
        }

        public WebClient2(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        ///     Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
            }
            return request;
        }
    }
}