using System;
using System.Collections.Generic;

namespace Memorandum.Web.Framework.Utilities
{
    /// <summary>
    ///     Simple key-value storage with changed event
    /// </summary>
    internal class Context
    {
        protected Dictionary<string, object> Data = new Dictionary<string, object>();

        /// <summary>
        /// Fired each time the context changed
        /// </summary>
        public event Action<Context, string, object> ContextChanged;

        /// <summary>
        /// Get value from context with given key
        /// </summary>
        /// <typeparam name="T">Type of value to store</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Item. Returns null if it doesn't exist in context</returns>
        public T? Get<T>(string key) where T : struct
        {
            if (!Data.ContainsKey(key))
                return null;
            return (T)Convert.ChangeType(Data[key], typeof(T));
        }

        /// <summary>
        /// Adds or sets (if exists) value in context, setting value to null will remove an item from context
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Some value</param>
        public void Set(string key, object value)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            if (Data.ContainsKey(key))
                Data[key] = value;
            else
                Data.Add(key, value);

            ContextChanged?.Invoke(this, key, value);
        }

        /// <summary>
        /// Remove item with given key from context if it exists
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (Data.ContainsKey(key))
            {
                Data.Remove(key);
                ContextChanged?.Invoke(this, key, null);
            }
        }
    }
}