using Arch.Data.Common.Logging;
using Arch.Data.Common.Logging.Configuration;
using Arch.Data.Common.Metrics.Configuration;
using Arch.Data.Common.Tracing.Configuration;
using DAL.DbEngine.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Arch.Data.DbEngine.Configuration
{
    /// <summary>
    /// 数据库引擎配置节
    /// </summary>
    public sealed class DbEngineConfigurationSection
    {
        #region construction

        /// <summary>
        /// 构造方法
        /// </summary>
        public DbEngineConfigurationSection()
        {
            DatabaseSets = new List<DatabaseSetElement>();
            DatabaseSets = DatabaseProviderElementCollection.DatabaseSetElement();
            DatabaseProviders = new DatabaseProviderElementCollection();
            ConnectionLocator = new ConnectionLocatorElement();
            DefaultLogger defaultLogger = new DefaultLogger();
            System.Threading.Tasks.Task.Run(() =>
            {
                bool bo = true;
                while (bo)
                {
                    bo = false;
                    try
                    {
                        defaultLogger.RunLog();
                    }
                    catch (Exception)
                    {
                        bo = true;
                    }
                }
            });

            //logListeners = new ConfigurationProperty("logListeners", typeof(ListenerElementCollection), null, ConfigurationPropertyOptions.None);
            //metrics = new ConfigurationProperty("metrics", typeof(MetricsElement), null, ConfigurationPropertyOptions.None);
            //tracing = new ConfigurationProperty("tracing", typeof(TracingElement), null, ConfigurationPropertyOptions.None);
            //log = new ConfigurationProperty("log", typeof(LoggingElement), null, ConfigurationPropertyOptions.None);
            //connectionLocator = new ConfigurationProperty("connectionLocator", typeof(ConnectionLocatorElement), null, ConfigurationPropertyOptions.None);
            //rw = new ConfigurationProperty("rw", typeof(RWSplittingElement), null, ConfigurationPropertyOptions.None);
            //executor = new ConfigurationProperty("executor", typeof(ExecutorElement), null, ConfigurationPropertyOptions.None);
        }

        #endregion

        #region get configuration

        /// <summary>
        /// Db引擎配置节名称
        /// </summary>
        private const String SectionName = "dal";

        /// <summary>
        /// 获取Db引擎配置节配置节
        /// </summary>
        /// <returns></returns>
        public static DbEngineConfigurationSection GetConfig()
        {
            var db_eng = new DbEngineConfigurationSection();
            return db_eng;
        }

        /// <summary>
        /// 名称,关键字
        /// </summary>
        public String Name
        {
            get; set;
        }

        #endregion

        #region public properties

        /// <summary>
        /// 数据库集配置数组
        /// </summary>
        public List<DatabaseSetElement> DatabaseSets
        {
            get; set;
        }

        /// <summary>
        /// 数据库提供者配置数组
        /// </summary>
        public DatabaseProviderElementCollection DatabaseProviders
        {
            get; set;
        }

        /// <summary>
        /// 数据库提供者配置数组
        /// </summary>
        public ListenerElementCollection LogListeners
        {
            get; set;
        }

        /// <summary>
        /// 数据库提供者配置数组
        /// </summary>
        public MetricsElement Metrics
        {
            get; set;
        }

        /// <summary>
        /// 数据库提供者配置数组
        /// </summary>
        public TracingElement Tracing
        {
            get; set;
        }

        /// <summary>
        /// Connection String
        /// </summary>
        public ConnectionLocatorElement ConnectionLocator
        {
            get; set;
        }

        /// <summary>
        /// Log
        /// </summary>
        public LoggingElement Log
        {
            get; set;
        }

        /// <summary>
        /// RW
        /// </summary>
        public RWSplittingElement RWSplitting
        {
            get; set;
        }

        public ExecutorElement Executor
        {
            get; set;
        }

        #endregion
    }
}
