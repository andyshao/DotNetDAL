using System;
using System.Data.Common;
using System.Data.SqlClient;
//using Npgsql;
using Raven.Client.Documents.Operations.ETL.SQL;

namespace Raven.Server.Documents.ETL.Providers.SQL.RelationalWriters
{
    public class DbProviderFactories
    {
        public static DbProviderFactory GetFactory(string factoryName)
        {
            switch (SqlProviderParser.GetSupportedProvider(factoryName))
            {
                case SqlProvider.SqlClient:
                    return SqlClientFactory.Instance;
                case SqlProvider.Npgsql:
                    return null;//NpgsqlFactory.Instance;
                default:
                    throw new NotSupportedException($"Factory '{factoryName}' is not supported");
            }
        }
    }

    public static class DbProviderFactoryExtensions
    {
        public static DbCommandBuilder InitializeCommandBuilder(this DbProviderFactory factory)
        {
            if (factory is SqlClientFactory)
                return new DbCommandBuilder
                {
                    Start = "[",
                    End = "]"
                };
            //if (factory is NpgsqlFactory)
            //    return new DbCommandBuilder
            //    {
            //        Start = "\"",
            //        End = "\""
            //    };
            return new DbCommandBuilder();
        }
    }
}
