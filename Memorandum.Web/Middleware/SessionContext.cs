using System;
using System.Collections.Generic;
using Memorandum.Web.Framework.Utilities;
using Newtonsoft.Json;

namespace Memorandum.Web.Middleware
{
    /// <summary>
    ///     SessionContext object, identifies source of incoming request. Used as datacontainer for attaching any data to
    ///     current session e.g. User
    /// </summary>
    internal class SessionContext : Context
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = 
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

        public SessionContext(string key, DateTime expires)
        {
            Key = key;
            Expires = expires;
        }

        public bool CookieExist { get; set; }
        public string Key { get; private set; }
        public DateTime Expires { get; private set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(Data, JsonSerializerSettings);
        }

        public void Deserialize(string input)
        {
            Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(input, JsonSerializerSettings);
        }
    }
}