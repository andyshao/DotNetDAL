using System;
using System.Configuration;

namespace Arch.Data.Common.Logging.Configuration
{
    /// <summary>
    /// 日志侦听器集合
    /// </summary>
    public sealed class ListenerElementCollection 
    {
        #region collection operator


        #region Sampling

        private const String SamplingProperty = "sampling";

        public String Sampling
        {
            get;set;
        }

        #endregion

        #region Encrypt

        private const String EncryptProperty = "encrypt";

        public String Encrypt
        {
            get;set;
        }

        #endregion

        #region Switch

        private const String SwitchProperty = "switch";
        private const String SwitchDefaultValue = "on";

        public String Switch
        {
            get;set;
        }

        #endregion

        #region ConcurrentCapacity

        private const String ConcurrentCapacityProperty = "concurrentCapacity";
        public const Int32 ConcurrentCapacityDefaultValue = 1000;

        private String concurrentCapacity
        {
            get;set;
        }

        public Int32 ConcurrentCapacity
        {
            get
            {
                Int32 value = ConcurrentCapacityDefaultValue;
                String capacity = concurrentCapacity;

                if (!String.IsNullOrEmpty(capacity))
                {
                    if (!Int32.TryParse(capacity, out value))
                        value = ConcurrentCapacityDefaultValue;
                }

                return value;
            }
        }

        #endregion

        #region Cat Switch

        private const String CatProperty = "cat";
        private const String CatDefaultValue = "on";

        public String Cat
        {
            get;set;
        }

        #endregion

        #region Cat Parameter Switch

        private const String SensitiveProperty = "sensitive";
        private const String SensitiveDefaultValue = "off";

        public String Sensitive
        {
            get;set;
        }

        #endregion

        #endregion
    }
}
