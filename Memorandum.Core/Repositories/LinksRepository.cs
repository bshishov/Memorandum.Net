using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    public class LinksRepository : DatabaseRepository<Link, int>
    {
        public LinksRepository() : base()
        {
        }

        public LinksRepository(ISession session) : base(session)
        {
        }
    }
}