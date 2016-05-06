using System;
using System.Collections.Generic;

namespace Memorandum.Web.Framework.Middleware.Session
{
    class SessionInfo
    {
        public string Key { get; set; }
        public DateTime Expires { get; set; }
        public string Data { get; set; }
    }

    interface ISessionStorage
    {
        void Save(SessionInfo sessionInfo);
        SessionInfo Load(string key);
        void Delete(SessionInfo sessionInfo);
    }

    class MemorySessionStorage : ISessionStorage
    {
        private readonly Dictionary<string, SessionInfo> _sessions = new Dictionary<string, SessionInfo>();

        public void Save(SessionInfo sessionInfo)
        {
            if (_sessions.ContainsKey(sessionInfo.Key))
                _sessions[sessionInfo.Key] = sessionInfo;
            else
                _sessions.Add(sessionInfo.Key, sessionInfo);
        }

        public SessionInfo Load(string key)
        {
            if(_sessions.ContainsKey(key))
                return _sessions[key];
            return null;
        }

        public void Delete(SessionInfo sessionInfo)
        {
            _sessions.Remove(sessionInfo.Key);
        }
    }
}
