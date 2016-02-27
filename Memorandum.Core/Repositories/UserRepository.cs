using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Memorandum.Core.Domain;
using Memorandum.Core.Utilities;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class UserRepository : DatabaseRepository<User, int>
    {
        public UserRepository(ISession session)
            : base(session)
        {
        }

        public string CreateNewPasswordString(string password, int iterations = 24000)
        {
            var salt = CreateSalt();
            var rfc2898 = new Rfc2898DeriveBytes_sha256(password, Encoding.ASCII.GetBytes(salt), iterations);
            var hash = Convert.ToBase64String(rfc2898.GetBytes(32));

            return $"pbkdf2_sha256${iterations}${salt}${hash}";
        }

        private static string CreateSalt(int length = 12)
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[length];
            random.GetNonZeroBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public User Auth(string login, string password)
        {
             var user = Where(u => u.Username.Equals(login)).FirstOrDefault();
             if (user == null)
                 throw new Exception("User not found");

             var passParts = user.Password.Split('$');
             var iterations = Convert.ToInt32(passParts[1]);
             var salt = passParts[2];
             var hash = passParts[3];

             var rfc2898 = new Rfc2898DeriveBytes_sha256(password, Encoding.ASCII.GetBytes(salt), iterations);
             var newHash = Convert.ToBase64String(rfc2898.GetBytes(32));

             if (hash.Equals(newHash))
             {
                 user.LastLogin = DateTime.Now;
                 Save(user);
                 return user;
             }

             throw new Exception("Password mismatch");
        }
    }
}
