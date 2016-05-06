using System;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Framework.Middleware.Session;

namespace Memorandum.Web.Middleware
{
    class CustomSessionContext : SessionContext
    {
        public static string UserSessionKey = "username";
        private User _user;

        public User User
        {
            get
            {
                if (_user == null)
                {
                    var name = Get<string>(UserSessionKey);
                    if (!string.IsNullOrEmpty(name))
                        _user = UserManager.Get(name);
                }
                return _user;
            }
            set
            {
                if (_user == null)
                {
                    Set(UserSessionKey, value.Name);
                    _user = UserManager.Get(value.Name);
                }
                else if(_user != null && value == null) // logout
                {
                    Remove(UserSessionKey);
                    _user = null;
                }
                else
                {
                    throw new InvalidOperationException("User already set");
                }
            }
        }

        public CustomSessionContext(string key, DateTime expires) : base(key, expires)
        {
        }
    }

    class CustomSessionFactory : ISessionFactory
    {
        public ISessionContext Create(string key, DateTime expires)
        {
            return new CustomSessionContext(key, expires);
        }
    }

    class CustomSessionMiddleware : SessionMiddleware
    {
        public CustomSessionMiddleware(ISessionStorage storage) : base(storage, new CustomSessionFactory())
        {
        }
    }
}
