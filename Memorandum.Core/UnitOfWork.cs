﻿using System;
using Memorandum.Core.Repositories;
using NHibernate;

namespace Memorandum.Core
{
    public class UnitOfWork : IDisposable
    {
        private ITransaction _transaction;
        private readonly ISession _session;
        private NodeRepository _nodes;
        private TextNodeRepository _text;
        private UrlNodeRepository _urls;
        private LinksRepository _links;
        private FileNodeRepository _files;
        private UserRepository _users;

        public TextNodeRepository Text => _text ?? (_text = new TextNodeRepository(_session));

        public UrlNodeRepository URL => _urls ?? (_urls = new UrlNodeRepository(_session));

        public LinksRepository Links => _links ?? (_links = new LinksRepository(_session));

        public FileNodeRepository Files => _files ?? (_files = new FileNodeRepository());

        public NodeRepository Nodes => _nodes ?? (_nodes = new NodeRepository(Text, URL, Files));

        public UserRepository Users => _users ?? (_users = new UserRepository(_session));

        public UnitOfWork()
        {
            _session = Database.OpenSession();
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _session.Close();
            }
        }

        public void Dispose()
        {
            if (_session != null)
            {
                _session.Flush(); // commit session transactions
                _session.Close();
            }
        }
    }
}