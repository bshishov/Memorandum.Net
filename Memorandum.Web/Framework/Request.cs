using System.Collections.Specialized;
using Memorandum.Core;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework
{
    class Request
    {
        public string Method { get { return RawRequest.GetParameterASCII("REQUEST_METHOD"); } }
        public string Path { get { return RawRequest.GetParameterUTF8("DOCUMENT_URI"); } }
        public Session Session { get; set; }
        public UnitOfWork UnitOfWork { get; set; }

        /// <summary>
        /// Lazy loaded cookies from request
        /// </summary>
        public NameValueCollection Cookies
        {
            get
            {
                if (_cookies == null && RawRequest.Parameters.ContainsKey("HTTP_COOKIE"))
                {
                    _cookies = HttpUtilities.ParseQueryString(RawRequest.GetParameterUTF8("HTTP_COOKIE"));
                }

                return _cookies;
            }
        }

        /// <summary>
        /// Lazy loaded query arguments from QUERY_STRING
        /// </summary>
        public NameValueCollection QuerySet
        {
            get
            {
                if (_querySet == null && RawRequest.Parameters.ContainsKey("QUERY_STRING"))
                {
                    _querySet = HttpUtilities.ParseQueryString(RawRequest.GetParameterASCII("QUERY_STRING"));
                }

                return _querySet;
            }
        }

        /// <summary>
        /// Lazy loaded post arguments
        /// </summary>
        public NameValueCollection PostArgs
        {
            get
            {
                if (_postArgs == null && Method.Equals("POST") && !string.IsNullOrEmpty(RawRequest.Body))
                {
                    _postArgs = HttpUtilities.ParseQueryString(RawRequest.Body);
                }

                return _postArgs;
            }
        }

        public FastCGI.Request RawRequest { get; private set; }

        private NameValueCollection _cookies;
        private NameValueCollection _querySet;
        private NameValueCollection _postArgs;

        public Request(FastCGI.Request rawRequest)
        {
            RawRequest = rawRequest;
        }
    }
}
