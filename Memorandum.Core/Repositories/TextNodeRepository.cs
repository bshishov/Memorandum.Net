using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class TextNodeRepository : DatabaseRepository<TextNode, int>
    {
        public TextNodeRepository(ISession session)
            : base(session)
        {
        }

        public override void Delete(TextNode entity)
        {
            base.Delete(entity);
            Search.SearchManager.TextNodeIndex.ClearLuceneIndexRecord(entity);
        }

        public override void Save(TextNode entity)
        {
            base.Save(entity);
            Search.SearchManager.TextNodeIndex.AddUpdateLuceneIndex(entity);
        }
    }
}