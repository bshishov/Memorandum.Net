using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class TextNodeRepository : DatabaseRepository<TextNode, int>
    {
        public TextNodeRepository() : base()
        {
        }

        public TextNodeRepository(ISession session)
            : base(session)
        {
        }
    }
}