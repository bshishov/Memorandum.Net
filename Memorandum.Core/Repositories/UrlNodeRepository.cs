using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class UrlNodeRepository : DatabaseRepository<URLNode, int>
    {
        public UrlNodeRepository(ISession session)
            : base(session)
        {
        }

        public override void Delete(URLNode entity)
        {
            base.Delete(entity);
            Search.SearchManager.UrlNodeIndex.ClearLuceneIndexRecord(entity);
        }

        public override void Save(URLNode entity)
        {
            base.Save(entity);
            Search.SearchManager.UrlNodeIndex.AddUpdateLuceneIndex(entity);
        }
    }
}