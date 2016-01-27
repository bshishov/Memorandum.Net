using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class UrlNodeRepository : DatabaseRepository<URLNode, int>
    {
        public UrlNodeRepository() : base()
        {
        }

         public UrlNodeRepository(ISession session)
            : base(session)
        {
        }
    }
}