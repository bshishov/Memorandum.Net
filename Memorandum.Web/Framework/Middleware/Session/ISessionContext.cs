using System;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework.Middleware.Session
{
    public interface ISessionContext
    {
        bool CookieExist { get; set; }
        string Key { get; }
        DateTime Expires { get; }
        string Serialize();
        void Deserialize(string input);
        event Action<Context, string, object> ContextChanged;
    }
}