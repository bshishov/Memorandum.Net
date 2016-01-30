using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework.Middleware
{
    /// <summary>
    /// Session object, identifies source of incoming request. You may use its datacontainer for attaching any data to current session e.g. User
    /// </summary>
    class Session : Context
    {
        public bool CookieExist { get; set; }
        public string Key { get; private set; }

        public Session(string key)
        {
            Key = key;
        }
    }
}