using System.Collections.Generic;

namespace Memorandum.Core.Repositories
{
    public interface IRepository<T, in TKey>
    {
        IEnumerable<T> GetAll();
        void Delete(T entity);
        void Save(T entity);
        T FindById(TKey id);
        IEnumerable<T> ByIds(TKey[] ids);
    }
}