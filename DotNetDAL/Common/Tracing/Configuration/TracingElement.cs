using System;
using System.Configuration;

namespace Arch.Data.Common.Tracing.Configuration
{
    public sealed class TracingElement 
    {
        /// <summary>
        /// 名称
        /// </summary>
        private const String name = "name";

        /// <summary>
        /// 开关
        /// </summary>
        private const String turn = "turn";

        /// <summary>
        /// 名称,关键字
        /// </summary>
        public String Name
        {
            get;set;
        }

        /// <summary>
        /// 开关
        /// </summary>
        public String Turn
        {
            get;set;
        }

    }
}