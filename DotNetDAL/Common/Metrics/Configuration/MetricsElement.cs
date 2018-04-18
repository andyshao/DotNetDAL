using System;
using System.Configuration;

namespace Arch.Data.Common.Metrics.Configuration
{
    public sealed class MetricsElement 
    {
        /// <summary>
        /// 名称
        /// </summary>
        private const String name = "name";

        public MetricsElement() { }

        /// <summary>
        /// 名称,关键字
        /// </summary>
        public String Name
        {
            get;set;
        }

    }
}