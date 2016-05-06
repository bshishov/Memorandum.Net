using System;
using System.Collections.Generic;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Middleware
{
    class ApiMiddleware : IMiddleware
    {
        public static readonly Dictionary<string, string> Tokens = new Dictionary<string, string>();
        private readonly string _apiPath;
        const string TokenQueryStrigKey = "token";

        public ApiMiddleware(string apiPath)
        {
            _apiPath = apiPath;
        }

        public void Handle(IRequest request)
        {
            if (request.Path.StartsWith(_apiPath))
            {
                var token = request.QuerySet[TokenQueryStrigKey];
                if (!string.IsNullOrEmpty(token))
                    if (Tokens.ContainsKey(token))
                        ((CustomSessionContext)request.Session).User = null;//Tokens[token];

                throw new NotImplementedException();
            }
        }

        public void Handle(IRequest request, Response arg2)
        {
        }
    }
}
