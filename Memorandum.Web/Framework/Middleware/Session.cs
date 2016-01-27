using System.Collections.Generic;

namespace Memorandum.Web.Framework.Middleware
{
    /// <summary>
    /// Session object, identifies source of incoming request. You may use its datacontainer for attaching any data to current session e.g. User
    /// </summary>
    class Session
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public bool CookieExist { get; set; }
        public string Key { get; private set; }

        public Session(string key)
        {
            Key = key;
        }

        public T Get<T>(string key) where T : class
        {
            if (!_data.ContainsKey(key))
                return null;
            return _data[key] as T;
        }

        public void Set(string key, object value)
        {
            if (_data.ContainsKey(key))
                _data[key] = value;
            else
                _data.Add(key, value);
        }

        public void Remove(string key)
        {
            if(_data.ContainsKey(key))
                _data.Remove(key);
        }
    }
}