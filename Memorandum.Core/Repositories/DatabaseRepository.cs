using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Memorandum.Core.Repositories
{
    public class DatabaseRepository<T, TKey> : IRepository<T, TKey>, IDisposable
        where T : class
    {
        protected ISession Session = null;
        protected ITransaction Transaction = null;

        public DatabaseRepository()
        {
            Session = Database.OpenSession();
        }

        public DatabaseRepository(ISession session)
        {
            Session = session;
        }

        public void BeginTransaction()
        {
            Transaction = Session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            // _transaction will be replaced with a new transaction            
            // by NHibernate, but we will close to keep a consistent state.
            Transaction.Commit();
            CloseTransaction();
        }

        public void RollbackTransaction()
        {
            // _session must be closed and disposed after a transaction            
            // rollback to keep a consistent state.
            Transaction.Rollback();
            CloseTransaction();
            CloseSession();
        }

        private void CloseTransaction()
        {
            Transaction.Dispose();
            Transaction = null;
        }

        private void CloseSession()
        {
            Session.Close();
            Session.Dispose();
            Session = null;
        }

        public IEnumerable<T> GetAll()
        {
            return Session.CreateCriteria<T>().List<T>();
        }

        public void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public void Save(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public T FindById(TKey id)
        {
            return Session.Load(typeof(T), id) as T;
        }

        public IEnumerable<T> ByIds(TKey[] ids)
        {
            return Session.CreateCriteria<T>().Add(Restrictions.In(Projections.Id(), ids)).List<T>();
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> query)
        {
            return Session.Query<T>().Where(query).ToList();
        }

        public IQueryable<T> Query()
        {
            return Session.Query<T>();
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                // Commit transaction by default, unless user explicitly rolls it back.
                // To rollback transaction by default, unless user explicitly commits,                // comment out the line below.
                CommitTransaction();
            }

            if (Session != null)
            {
                Session.Flush(); // commit session transactions
                CloseSession();
            }
        }
    }
}