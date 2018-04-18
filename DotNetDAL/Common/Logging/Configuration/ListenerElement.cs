using System;
using System.Configuration;

namespace Arch.Data.Common.Logging.Configuration
{
    public sealed class ListenerElement
    {
        #region private field

        /// <summary>
        /// 名称
        /// </summary>
        private const String c_NameProperty = "name";

        /// <summary>
        /// 类型
        /// </summary>
        private const String c_TypeProperty = "type";

        /// <summary>
        /// 日志级别
        /// </summary>
        private const String c_LevelProperty = "level";

        /// <summary>
        /// 初始化配置
        /// </summary>
        private const String c_SettingProperty = "setting";

        public ListenerElement() { }

        #endregion

        #region public properties

        /// <summary>
        /// 名称,关键字
        /// </summary>
        public String Name
        {
            get;set;
        }

        /// <summary>
        /// 级别
        /// </summary>
        public LogLevel Level
        {
            get;set;
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        public String TypeName
        {
            get;set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type
        {
            get { return Type.GetType(TypeName); }
            set { TypeName = value.AssemblyQualifiedName; }
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public String Setting
        {
            get;set;
        }

        #endregion
    }
}
