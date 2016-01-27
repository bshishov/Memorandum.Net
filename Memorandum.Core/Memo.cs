using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memorandum.Core.Domain;
using Memorandum.Core.Repositories;
using Memorandum.Core.Utilities;


namespace Memorandum.Core
{
    public class Memo
    {
        public TextNodeRepository TextNodes { get; private set; }
        public UrlNodeRepository URLNodes { get; private set; }
        public LinksRepository Links { get; private set; }
        public FileNodeRepository Files { get; private set; }
        public DatabaseRepository<Profile, int> Profiles { get; set; }
        private UserRepository Users { get; set; }
        


        public Memo()
        {
            Files = new FileNodeRepository();
            Links = new LinksRepository();
            URLNodes = new UrlNodeRepository();
            TextNodes = new TextNodeRepository();
            Users = new UserRepository();
            Profiles = new DatabaseRepository<Profile, int>();
        }

        public User Auth(string login, string password)
        {
            var user = Users.Where(u => u.Username.Equals(login)).FirstOrDefault();
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
                Users.Save(user);
                return user;
            }

            throw new Exception("Password mismatch");
        }
    }
}
