using System;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.Properties;
using Shine.Middleware.Session;

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
                if ((_user == null || _user is Guest) && value != null)
                {
                    Set(UserSessionKey, value.Name);
                    _user = value;
                }
                else if (_user != null && !(_user is Guest) && value == null) // logout
                {
                    Remove(UserSessionKey);
                    _user = UserManager.GuestAuth();
                }
                else
                {
                    throw new InvalidOperationException("User already set");
                }
            }
        }

        public CustomSessionContext(string key, DateTime expires) : base(key, expires)
        {
            User = UserManager.GuestAuth();
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
        public CustomSessionMiddleware(ISessionStorage storage) : base(Settings.Default.Secret, storage, new CustomSessionFactory(), TimeSpan.FromDays(1))
        {
        }

        public CustomSessionMiddleware() : this(new MemorySessionStorage())
        {
        }
    }
}
