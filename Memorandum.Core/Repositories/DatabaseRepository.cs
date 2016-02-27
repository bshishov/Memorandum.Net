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

        public DatabaseRepository(ISession session)
        {
            Session = session;
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

        public virtual void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public virtual void Save(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public T FindById(TKey id)
        {
            return Session.CreateCriteria<T>().Add(Restrictions.Eq(Projections.Id(), id)).UniqueResult<T>();
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
            if (Session != null)
            {
                Session.Flush(); // commit session transactions
                CloseSession();
            }
        }
    }
}