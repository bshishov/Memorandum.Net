using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using HttpMultipartParser;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework.Middleware.Session;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework.Backend.HttpListener
{
    class RequestWrapper : IRequest
    {
        private NameValueCollection _cookies;
        private NameValueCollection _postArgs;
        private NameValueCollection _querySet;
        private MultipartFormDataParser _parser;

        public RequestWrapper(HttpListenerRequest request)
        {
            _request = request;
        }

        public string Method => _request.HttpMethod;
        public string Path => _request.RawUrl;
        public string ContentType => _request.ContentType;
        public ISessionContext Session { get; set; }
        
        public User User { get; set; }

        /// <summary>
        ///     Lazy loaded cookies from request
        /// </summary>
        public NameValueCollection Cookies
        {
            get
            {
                if (_cookies == null)
                {
                    _cookies = new NameValueCollection();
                    foreach (Cookie cookie in _request.Cookies)
                    {
                        _cookies.Add(cookie.Name, cookie.Value);
                    }
                }

                return _cookies;
            }
        }

        /// <summary>
        ///     Lazy loaded query arguments from QUERY_STRING
        /// </summary>
        public NameValueCollection QuerySet => _querySet ?? (_querySet = HttpUtilities.ParseQueryString(_request.Url.Query));

        private MultipartFormDataParser MultipartParser => _parser ?? (_parser = new MultipartFormDataParser(_request.InputStream));

        /// <summary>
        ///     Lazy loaded post arguments
        /// </summary>
        public NameValueCollection PostArgs
        {
            get
            {
                if (_postArgs == null && (Method.Equals("POST") || Method.Equals("PUT")) && _request.InputStream.CanRead)
                {
                    if (ContentType.Contains("multipart"))
                    {
                        _postArgs = new NameValueCollection();
                        foreach (var par in MultipartParser.Parameters)
                        {
                            _postArgs.Add(par.Name, par.Data);
                        }
                    }
                    else
                    {
                        using (System.IO.Stream body = _request.InputStream) // here we have data
                        {
                            using (
                                System.IO.StreamReader reader = new System.IO.StreamReader(body,
                                    _request.ContentEncoding))
                            {
                                _postArgs = HttpUtilities.ParseQueryString(reader.ReadToEnd());
                            }
                        }
                    }
                }

                return _postArgs;
            }
        }

        public IEnumerable<FilePart> Files => MultipartParser.Files;
     
        private readonly HttpListenerRequest _request;
    }
}
