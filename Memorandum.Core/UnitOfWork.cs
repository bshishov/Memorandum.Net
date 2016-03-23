using System;
using Memorandum.Core.Repositories;
using NHibernate;

namespace Memorandum.Core
{
    public class UnitOfWork : IDisposable
    {
        private readonly ISession _session;
        private NodeRepository _nodes;
        private TextNodeRepository _text;
        private UrlNodeRepository _urls;
        private LinksRepository _links;
        private FileNodeRepository _files;
        private UserRepository _users;
        private DatabaseRepository<Domain.Session, string> _sessions;

        public TextNodeRepository Text => 
            _text ?? (_text = new TextNodeRepository(_session));

        public UrlNodeRepository URL => 
            _urls ?? (_urls = new UrlNodeRepository(_session));

        public LinksRepository Links => 
            _links ?? (_links = new LinksRepository(_session));

        public FileNodeRepository Files => 
            _files ?? (_files = new FileNodeRepository());

        public NodeRepository Nodes => 
            _nodes ?? (_nodes = new NodeRepository(Text, URL, Files));

        public UserRepository Users => 
            _users ?? (_users = new UserRepository(_session));

        public DatabaseRepository<Domain.Session, string> Sessions => 
            _sessions ?? (_sessions = new DatabaseRepository<Domain.Session, string>(_session));

        public UnitOfWork()
        {
            _session = Database.OpenSession();
        }
        

        public void Dispose()
        {
            if (_session != null)
            {
                _session.Flush(); // commit session transactions
                _session.Close();
            }
        }

        public T UnProxy<T>(object o) where T : class 
        {
            if (!NHibernateUtil.IsInitialized(o))
            {
                NHibernateUtil.Initialize(o);
            }

            return (T)_session.GetSessionImplementation().PersistenceContext.Unproxy(o);
        }
    }
}