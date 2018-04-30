using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL.DbEngine.Configuration
{
    public class ConfigModel
    {
        public DbEngine dbEngine { get; set; }

        public List<databaseSets> databaseSets { get; set; }

        public List<Connections> connections { get; set; }

        /// <summary>
        /// 日志数据库地址
        /// </summary>
        public List<string> logDataBaseUrl { get; set; }

        /// <summary>
        /// 日志数据库名称
        /// </summary>
        public string logDatabase { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ReadConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 
        /// </summary>
        private static ReadConfigModel _readConfigModel = null;

        /// <summary>
        /// 
        /// </summary>
        private static ConfigModel _configModel = null;

        /// <summary>
        /// 
        /// </summary>
        public ConfigModel configModel
        {
            get { return _configModel; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ReadConfigModel GetInstance()
        {
            if (_readConfigModel == null)
            {
                lock (_lock)
                {
                    if (_readConfigModel == null)
                    {
                        _readConfigModel = new ReadConfigModel();
                    }
                }
            }
            return _readConfigModel;
        }

        private ReadConfigModel()
        {
            init();
        }


        public void init()
        {
            try
            {
                var path = Environment.CurrentDirectory;
                using (var read = File.OpenText(string.Concat(path, "\\conf.json")))
                {
                    string content = read.ReadToEnd();
                    _configModel = JsonConvert.DeserializeObject<ConfigModel>(content);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class DbEngine
    {
        /// <summary>
        /// sqlProvider
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 数据库提供者实现
        /// </summary>
        public string typeName { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class databaseSets
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string provider { get; set; }

        /// <summary>
        /// 分片规则
        /// </summary>
        public string shardingStrategy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<databaseSet> databaseSet { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class databaseSet
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Master 
        /// </summary>
        public string databaseType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string sharding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string connectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; } 

    }



    /// <summary>
    /// 实际连接字符串
    /// </summary>
    public class Connections
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string connection { get; set; }

    }



}
