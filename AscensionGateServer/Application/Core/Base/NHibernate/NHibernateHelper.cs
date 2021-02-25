using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using Cosmos;

namespace AscensionGateServer
{
    public class NHibernateHelper
    {
        static MySqlData sqlData;
        static NHibernateHelper()
        {
            var result = ServerEntry.DataManager.TryGetValue(out sqlData);
            if (!result)
                Utility.Debug.LogError("Get MySqlData fail，check your config file !");
        }
        private static ISessionFactory _sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure().
                        Database(MySQLConfiguration.Standard.
                        ConnectionString(db => db.Server(sqlData.Address).
                        Database(sqlData.Database).Username(sqlData.Username).
                        Password(sqlData.Password))).
                        Mappings(x => { x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>(); }).
                        BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();//打开一个跟数据库的会话
        }
    }

}
