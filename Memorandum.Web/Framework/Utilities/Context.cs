using System.Collections.Generic;

namespace Memorandum.Web.Framework.Utilities
{
    /// <summary>
    ///     Simple key-value storage
    /// </summary>
    internal class Context
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

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
            if (_data.ContainsKey(key))
                _data.Remove(key);
        }
    }
}