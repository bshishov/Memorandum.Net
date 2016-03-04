using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using HttpMultipartParser;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework.Utilities;
using Memorandum.Web.Middleware;

namespace Memorandum.Web.Framework.Backend.FastCGI
{
    internal class FastCGIRequest : IRequest
    {
        private NameValueCollection _cookies;
        private NameValueCollection _postArgs;
        private NameValueCollection _querySet;
        private MultipartFormDataParser _parser;

        public FastCGIRequest(global::FastCGI.Request rawRequest)
        {
            RawRequest = rawRequest;
        }

        public string Method => RawRequest.GetParameterASCII("REQUEST_METHOD");
        public string Path => RawRequest.GetParameterUTF8("DOCUMENT_URI");
        public string ContentType => RawRequest.GetParameterASCII("CONTENT_TYPE");
        public SessionContext Session { get; set; }
        public UnitOfWork UnitOfWork { get; set; }
        
        /// <summary>
        /// Current User identifier, stored in session
        /// </summary>
        public int? UserId
        {
            get { return Session.Get<int>("userId"); }
            set { Session.Set("userId", value); }
        }

        private User _user;
        /// <summary>
        /// Lazy loaded User
        /// </summary>
        public User User => _user ?? (_user = UnitOfWork.Users.FindById(UserId.GetValueOrDefault()));

        /// <summary>
        ///     Lazy loaded cookies from request
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
        ///     Lazy loaded query arguments from QUERY_STRING
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

        private MultipartFormDataParser MultipartParser => _parser ?? (_parser = new MultipartFormDataParser(RawRequest.RequestBodyStream));

        /// <summary>
        ///     Lazy loaded post arguments
        /// </summary>
        public NameValueCollection PostArgs
        {
            get
            {
                if (_postArgs == null && (Method.Equals("POST") || Method.Equals("PUT")) && RawRequest.RequestBodyStream.Length > 0)
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
                        _postArgs = HttpUtilities.ParseQueryString(Encoding.UTF8.GetString(RawRequest.GetBody()));
                    }
                }

                return _postArgs;
            }
        }

        public IEnumerable<FilePart> Files => MultipartParser.Files;

        private global::FastCGI.Request RawRequest { get; }
    }
}