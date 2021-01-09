using DO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace DA
{
    public static class DBSession
    {
        static IConfiguration _configuration;

        private static ISessionFactory _MainSessionFactory { get; set; }
        private static ISessionFactory MainSessionFactory
        {
            get
            {
                if (_MainSessionFactory == null) InitializeMainSessionFactory();
                return _MainSessionFactory;
            }
        }

        private static void InitializeMainSessionFactory()
        {
            string conn = _configuration.GetConnectionString("MSSQL");


            _MainSessionFactory = Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012.ConnectionString(conn).ShowSql())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MovieMappings>())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMovieNoteMappings>())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMovieRatingMappings>())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserProfileMappings>())
            .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
            .BuildSessionFactory();
        }

        public static ISession OpenUserSession(IConfiguration configuration)
        {
            _configuration = configuration;
            return MainSessionFactory.OpenSession();
        }

    }
}
