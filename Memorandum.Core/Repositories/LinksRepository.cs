using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class LinksRepository : DatabaseRepository<Link, int>
    {
        public LinksRepository(ISession session) : base(session)
        {
        }

        public override void Delete(Link entity)
        {
            base.Delete(entity);
            Search.SearchManager.LinkIndex.ClearLuceneIndexRecord(entity);
        }

        public override void Save(Link entity)
        {
            base.Save(entity);
            Search.SearchManager.LinkIndex.AddUpdateLuceneIndex(entity);
        }
    }
}