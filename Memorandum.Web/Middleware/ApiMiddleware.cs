using System;
using System.Collections.Generic;
using Shine;
using Shine.Middleware;
using Shine.Responses;

namespace Memorandum.Web.Middleware
{
    class ApiMiddleware : IMiddleware
    {
        public static readonly Dictionary<string, string> Tokens = new Dictionary<string, string>();

        const string TokenQueryStrigKey = "token";

        public ApiMiddleware()
        {
        }

        public void Handle(IRequest request)
        {
            var token = request.QuerySet[TokenQueryStrigKey];
            if (!string.IsNullOrEmpty(token))
                if (Tokens.ContainsKey(token))
                    ((CustomSessionContext)request.Session).User = null; //Tokens[token];

            throw new NotImplementedException();
        }

        public void Handle(IRequest request, IResponse arg2)
        {
        }
    }
}
