using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Utilities;

namespace Memorandum.Core.Domain.Users
{
    public static class UserManager
    {
        private const string UsersFilePath = "users.memo";

        private static YamlFileItem<List<UserInfo>> UsersFile { get; }
        private static List<User> _users { get; set; }

        public static IEnumerable<User> Users => _users;
        public static User System { get; }

        static UserManager()
        {
            System = new User(new UserInfo()
            {
                BaseDirectory = Directory.GetCurrentDirectory(),
                IpAddress = "localhost",
                Name = "System",
                UsePasswordAuth = false,
                UseIpAuth = true,
            });

            UsersFile = new YamlFileItem<List<UserInfo>>(System, UsersFilePath);
        }

        public static void Load()
        {
            _users = UsersFile.Data.Select(info => new User(info)).ToList();
        }
        
        public static User AuthByPassword(string name, string password)
        {
            var user = Users.FirstOrDefault(u => u.UsePasswordAuth && u.Name.Equals(name));
            if (user == null)
                throw new Exception("User not found");

            var passParts = user.Password.Split('$');
            if (passParts.Length < 4)
                throw new InvalidOperationException("Corrupted password");

            var iterations = Convert.ToInt32(passParts[1]);
            var salt = passParts[2];
            var hash = passParts[3];

            var rfc2898 = new Rfc2898DeriveBytes_sha256(password, Encoding.ASCII.GetBytes(salt), iterations);
            var newHash = Convert.ToBase64String(rfc2898.GetBytes(32));

            if (hash.Equals(newHash))
                return user;

            throw new Exception("Password mismatch");
        }

        public static User AuthByIp(string ip)
        {
            return Users.FirstOrDefault(u => u.UseIpAuth && u.IpAddress.Equals(ip));
        }

        public static Guest GuestAuth()
        {
            return new Guest("");
        }

        public static User Create(string username, string password, string baseDirectory)
        {
            var info = new UserInfo()
            {
                Name = username,
                Password = CreateNewPasswordString(password),
                BaseDirectory = baseDirectory,
                UsePasswordAuth = true
            };
            
            var user = new User(info);
            _users.Add(user);
            Save();
            return user;
        }

        public static void Save()
        {
            UsersFile.Data = _users.Select(u => (UserInfo)u).ToList();
            UsersFile.Save();
        }

        public static User Get(string name)
        {
            if(name.Equals("guest"))
                return new Guest("");
            return Users.FirstOrDefault(u => u.Name.Equals(name));
        }

        private static string CreateSalt(int length = 12)
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[length];
            random.GetNonZeroBytes(salt);
            return Convert.ToBase64String(salt);
        }

        private static string CreateNewPasswordString(string password, int iterations = 24000)
        {
            var salt = CreateSalt();
            var rfc2898 = new Rfc2898DeriveBytes_sha256(password, Encoding.ASCII.GetBytes(salt), iterations);
            var hash = Convert.ToBase64String(rfc2898.GetBytes(32));

            return $"pbkdf2_sha256${iterations}${salt}${hash}";
        }
    }
}