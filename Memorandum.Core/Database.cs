using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Memorandum.Core
{
    public static class Database
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration _configuration;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _configuration = new Configuration();

                    _configuration.Configure();
                    _configuration.AddAssembly("Memorandum.Core");

                    _sessionFactory = _configuration.BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static void CreateSchema()
        {
            new SchemaExport(_configuration).Create(false, true);   
        }
    }
}
