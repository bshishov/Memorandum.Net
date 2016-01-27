using System;
using System.Linq;
using System.Text;
using Memorandum.Core.Domain;
using Memorandum.Core.Repositories;
using Memorandum.Core.Utilities;
using NHibernate;

namespace Memorandum.Core
{
    public class Memo : IDisposable
    {
        private readonly ISession _dbSession;
        public TextNodeRepository TextNodes { get; private set; }
        public UrlNodeRepository URLNodes { get; private set; }
        public LinksRepository Links { get; private set; }
        public FileNodeRepository FileNodes { get; private set; }
        public NodeRepository Nodes { get; private set; }
        private UserRepository Users { get; set; }
        
        public Memo()
        {
            _dbSession = Database.OpenSession();

            FileNodes = new FileNodeRepository();
            Links = new LinksRepository(_dbSession);
            URLNodes = new UrlNodeRepository(_dbSession);
            TextNodes = new TextNodeRepository(_dbSession);
            Users = new UserRepository(_dbSession);
            Nodes = new NodeRepository(TextNodes, URLNodes, FileNodes);
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

        public void Dispose()
        {
            if (_dbSession != null)
            {
                _dbSession.Flush(); // commit session transactions
                _dbSession.Close();
            }
        }
    }
}
