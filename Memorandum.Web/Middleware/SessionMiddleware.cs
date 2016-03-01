using System;
using System.Collections.Generic;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Framework;
using Memorandum.Web.Framework.Middleware;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Utilities;
using Memorandum.Web.Properties;

namespace Memorandum.Web.Middleware
{
    /// <summary>
    ///     Middleware that provides basic identification of incoming requests giving each request a session object
    /// </summary>
    internal class SessionMiddleware : IMiddleware
    {
        private const string SessionKeyCookieName = "SID";
        private static readonly TimeSpan SessionLifeTime = TimeSpan.FromDays(1); // TODO: To settings?
        private readonly Dictionary<string, SessionContext> _sessions = new Dictionary<string, SessionContext>();

        /// <summary>
        ///     Before request
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
                    var existingSession = LoadSession(request.UnitOfWork, key);
                    if (existingSession != null)
                    {
                        RegisterSession(request, existingSession);
                    }
                    else
                    {
                        // Create new session with provided key
                        // TODO: Remove ? throw "bad session key"? 
                        InitNewSession(request, key, true);
                    }
                }
            }
        }

        /// <summary>
        ///     After request
        /// </summary>
        /// <param name="request">Input request</param>
        /// <param name="response">Output request</param>
        public void Handle(Request request, Response response)
        {
            if (request.Session != null && !request.Session.CookieExist)
            {
                var httpresponse = response as HttpResponse;
                if (httpresponse == null)
                    return;
                if (httpresponse.Attributes == null)
                    httpresponse.Attributes = new Dictionary<string, string>();
                httpresponse.Attributes.Add("Set-Cookie",
                    $"{SessionKeyCookieName}={request.Session.Key}; Expires={request.Session.Expires:R}; Path=/; HttpOnly");
            }
        }

        /// <summary>
        ///     Initialize new session and adds it to the dictionary
        /// </summary>
        /// <param name="request">Input request</param>
        /// <param name="key">SessionContext key (GUID or existing session key)</param>
        /// <param name="cookieExists">Defines wheter cookie set on client or not</param>
        private void InitNewSession(Request request, string key, bool cookieExists = false)
        {
            var session = new SessionContext(key, DateTime.Now + SessionLifeTime) { CookieExist = cookieExists };
            session.ContextChanged += SessionOnContextChanged;
            SaveSession(request.UnitOfWork, session);
            RegisterSession(request, session);
        }

        private void SessionOnContextChanged(Context context, string s, object arg3)
        {
            var session = context as SessionContext;
            if(session == null)
                throw new InvalidOperationException("SessionContext expected");

            using (var unit = new UnitOfWork())
            {
                SaveSession(unit, session);
            }
        }

        private void RegisterSession(Request request, SessionContext session)
        {
            _sessions.Add(session.Key, session);
            request.Session = session;
        }

        private void SaveSession(UnitOfWork unit, SessionContext sessionContext)
        {
            var encryptedData = StringCipher.Encrypt(sessionContext.Serialize(), Settings.Default.Secret);

            unit.Sessions.Save(new Session()
            {
                Key = sessionContext.Key,
                Expires = sessionContext.Expires,
                Data = encryptedData
            });
        }

        private SessionContext LoadSession(UnitOfWork unit, string key)
        {
            var existingSession = unit.Sessions.FindById(key);
            if (existingSession != null)
            {
                // If session expired
                if (DateTime.Now > existingSession.Expires)
                {
                    // Delete sesssion if it is expired
                    unit.Sessions.Delete(existingSession);
                    return null;
                }
                var sessionContext = new SessionContext(existingSession.Key, existingSession.Expires);
                sessionContext.ContextChanged += SessionOnContextChanged;

                var data = StringCipher.Decrypt(existingSession.Data, Settings.Default.Secret);
                sessionContext.Deserialize(data);
                return sessionContext;
            }
            return null;
        }
    }
}