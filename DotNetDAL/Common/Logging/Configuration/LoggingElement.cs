using System;
using System.Configuration;

namespace Arch.Data.Common.Logging.Configuration
{
    public sealed class LoggingElement 
    {
        /// <summary>
        /// 类型
        /// </summary>
        private const String type = "type";

        private const String logEntryType = "logEntryType";

        /// <summary>
        /// 类型
        /// </summary>
        public String TypeName
        {
            get;set;
        }

        public String LogEntryTypeName
        {
            get;set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(TypeName);
                }
                catch
                {
                    return null;
                }

            }
            set { TypeName = value.AssemblyQualifiedName; }
        }

        public Type LogEntryType
        {
            get
            {
                try
                {
                    return Type.GetType(LogEntryTypeName);
                }
                catch
                {
                    return null;
                }
            }
            set { LogEntryTypeName = value.AssemblyQualifiedName; }
        }

    }
}
