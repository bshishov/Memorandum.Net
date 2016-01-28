using System.Collections.Specialized;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework
{
    class Request
    {
        public string QueryString { get { return RawRequest.GetParameterASCII("QUERY_STRING"); } }
        public string Method { get { return RawRequest.GetParameterASCII("REQUEST_METHOD"); } }
        public string Path { get { return RawRequest.GetParameterUTF8("DOCUMENT_URI"); } }
        public Session Session { get; set; }

        public NameValueCollection Cookies
        {
            get
            {
                if (_cookies == null && RawRequest.Parameters.ContainsKey("HTTP_COOKIE"))
                {
                    var cookiesStr = RawRequest.GetParameterUTF8("HTTP_COOKIE");
                    _cookies = HttpUtilities.ParseQueryString(cookiesStr);
                }

                return _cookies;
            }
        }

        public FastCGI.Request RawRequest { get; private set; }

        private NameValueCollection _cookies;
        public Request(FastCGI.Request rawRequest)
        {
            RawRequest = rawRequest;
        }
    }
}
