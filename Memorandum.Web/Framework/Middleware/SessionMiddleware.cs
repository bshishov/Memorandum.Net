using System;
using System.Collections.Generic;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework.Middleware
{
    /// <summary>
    /// Middleware that provides basic identification of incoming requests giving each request a session object
    /// </summary>
    class SessionMiddleware : IMiddleware
    {
        private const string SessionKeyCookieName = "SID";
        readonly Dictionary<string, Session> _sessions = new Dictionary<string, Session>();

        /// <summary>
        /// Befor request
        /// </summary>
        /// <param name="request">Input request</param>
        public void Handle(Request request)
        {
            string key = null;
            
            if (request.Cookies != null)
                key = request.Cookies[SessionKeyCookieName];

            if (String.IsNullOrEmpty(key))
            {
                InitNewSession(request, Guid.NewGuid().ToString());
            }
            else
            {
                if (_sessions.ContainsKey(key))
                {
                    // Set existing session
                    request.Session = _sessions[key];
                    request.Session.CookieExist = true;
                }
                else
                {
                    // Create new session with provided key
                    InitNewSession(request, key, true);
                }
            }
        }

        /// <summary>
        /// After request
        /// </summary>
        /// <param name="request">Input request</param>
        /// <param name="response">Output request</param>
        public void Handle(Request request, Response response)
        {
            if(request.Session != null && !request.Session.CookieExist)
            {
                var httpresponse = response as HttpResponse;
                if(httpresponse == null)
                    return;
                if(httpresponse.Attributes == null)
                    httpresponse.Attributes = new Dictionary<string, string>();
                httpresponse.Attributes.Add("Set-Cookie", string.Format("{0}={1}; path=/;", SessionKeyCookieName, request.Session.Key));
            }
        }

        /// <summary>
        /// Initialize new session and adds it to the dictionary
        /// </summary>
        /// <param name="request">Input request</param>
        /// <param name="key">Session key (GUID or existing session key)</param>
        /// <param name="cookieExists">Defines wheter cookie set on client or not</param>
        private void InitNewSession(Request request, string key, bool cookieExists = false)
        {
            var session = new Session(key) { CookieExist = cookieExists };
            _sessions.Add(session.Key, session);
            Console.WriteLine("New Session: {0}", session.Key);
            request.Session = session;
        }
    }
}
