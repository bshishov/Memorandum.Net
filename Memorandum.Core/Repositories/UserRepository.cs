using Memorandum.Core.Domain;
using NHibernate;

namespace Memorandum.Core.Repositories
{
    class UserRepository : DatabaseRepository<User, int>
    {
        public UserRepository() : base()
        {
        }

         public UserRepository(ISession session)
            : base(session)
        {
        }
    }
}
