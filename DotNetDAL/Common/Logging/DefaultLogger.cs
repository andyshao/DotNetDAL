using Arch.Data.Common.Enums;
using Arch.Data.DbEngine;
using Arch.Data.DbEngine.MarkDown;
using DAL.DbEngine.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System.Linq;
using Raven.Client.Documents.BulkInsert;

using System.Collections.Concurrent;
using System.Threading;

namespace Arch.Data.Common.Logging
{
    class DefaultLogger : ILogger
    {

        private static ConcurrentQueue<log> _queueLog = new ConcurrentQueue<log>();

        public DefaultLogger()
        {

        }


        public void Prepare(String logName, object collection, out LogLevel level)
        {
            level = LogLevel.Information;
        }

        public void Init(ILogEntry entry, Statement statement, String databaseName, String message)
        {

        }

        public void Start(ILogEntry entry, Stopwatch watch)
        {
            watch.Start();

        }

        public void Success(ILogEntry entry, Statement statement, Stopwatch watch, Func<Int32> func)
        {
            watch.Stop();
            Task.Run(() =>
            {
                AddLogDataBase(statement, watch);
            });


        }

        public void Error(Exception ex, ILogEntry entry, Statement statement, Stopwatch watch)
        {


        }

        public void Complete(ILogEntry entry)
        {

        }

        public void Log(ILogEntry entry)
        {

        }

        public void LogMarkdown(LogLevel level, String message, String errorCode) { }

        public void StartTracing()
        {


        }

        public void StopTracing()
        {

        }

        public void Next() { }

        public void TracingError(Exception ex)
        {

        }

        public void MetricsLog(String databaseSet, DatabaseType dbType, OperationType optType)
        {


        }

        public void MetricsMarkdown(MarkDownMetrics metrics)
        {



        }

        public void MetricsMarkup(MarkUpMetrics metrics)
        {

        }

        public void MetricsRW(String databaseSet, String databaseName, Boolean success)
        {



        }

        public void MetricsFailover(String databaseSet, String allInOneKey)
        {

        }



        #region logDatabase 

        public void RunLog()
        {
            using (BulkInsertOperation bulkInsert = LogDataBase.GetInit()._store.BulkInsert())
            {
                while (true)
                {
                    if (_queueLog.Count == 0)
                        Thread.Sleep(1000 * 5);
                    var bo = _queueLog.TryDequeue(out log _log);
                    if (bo)
                        try
                        {
                            bulkInsert.Store(_log);
                        }
                        catch (Exception ex)
                        {
                            bulkInsert.Dispose();
                            Thread.Sleep(1000 * 10);
                            _queueLog.Enqueue(_log);
                            break;
                        }
                }
            }
        }

        private void AddLogDataBase(Statement statement, Stopwatch stopwatch)
        {
            switch (statement.SqlOperationType)
            {
                case SqlStatementType.UNKNOWN:
                    break;
                case SqlStatementType.SELECT:
                    break;
                case SqlStatementType.INSERT:
                    AddLogQueue(statement, stopwatch, "INSERT");
                    break;
                case SqlStatementType.UPDATE:
                    AddLogQueue(statement, stopwatch, "UPDATE");
                    break;
                case SqlStatementType.DELETE:
                    AddLogQueue(statement, stopwatch, "DELETE");
                    break;
                case SqlStatementType.SP:
                    break;
                default:
                    break;
            }

        }

        private void AddLogQueue(Statement statement, Stopwatch stopwatch, string operationType)
        {
            string sql = statement.StatementText;
            string DatabaseSet = statement.DatabaseSet;
            foreach (var item in statement.Parameters)
            {
                if (item.DbType == System.Data.DbType.Int16
                    || item.DbType == System.Data.DbType.Int32
                    || item.DbType == System.Data.DbType.Int64
                    || item.DbType == System.Data.DbType.UInt16
                    || item.DbType == System.Data.DbType.UInt32
                    || item.DbType == System.Data.DbType.UInt64
                    || item.DbType == System.Data.DbType.Decimal
                    || item.DbType == System.Data.DbType.Boolean
                    || item.DbType == System.Data.DbType.Double)
                    sql = sql.Replace(item.Name, item.Value.ToString());
                sql = sql.Replace(item.Name, $"'{item.Value.ToString()}'");
            }
            _queueLog.Enqueue(new log()
            {
                databaseSet = DatabaseSet,
                rollBackSql = "",
                sql = sql,
                milliseconds = stopwatch.ElapsedMilliseconds,
                operationType = operationType
            });
        }



        #endregion

    }


    internal class LogDataBase
    {

        private static readonly object _lock = new object();

        private static IDocumentStore documentStore { get; set; }

        public IDocumentStore _store
        {
            get { return documentStore; }
        }


        private static LogDataBase _logDataBase { get; set; }


        public static LogDataBase GetInit()
        {
            if (_logDataBase == null)
            {
                lock (_lock)
                {
                    if (_logDataBase == null)
                    {
                        _logDataBase = new LogDataBase();
                    }
                }
            }
            return _logDataBase;
        }


        private LogDataBase()
        {
            var config = ReadConfigModel.GetInstance().configModel;
            documentStore = new DocumentStore()
            {
                Urls = config.logDataBaseUrl.ToArray(),
                Database = config.logDatabase,
                Conventions = { }
            };
            documentStore.Initialize();
        }


    }



    public class log
    {
        public string sql { get; set; }

        public string databaseSet { get; set; }

        public string operationType { get; set; }

        public long milliseconds { get; set; }

        public string rollBackSql { get; set; }
    }
}
