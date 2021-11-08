using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Model;
using Microsoft.Extensions.Configuration;
using System;

namespace NPlatform.Infrastructure.Config
{
    /// <summary>
    /// apollo配置获取
    /// </summary>
    public  class ApolloConfiguration
    {
        public ConfigChangeEvent OnChangeConfig;

        IConfiguration _config;
        /// <summary>
        /// Initializes a new instance of the <see cref="ApolloConfiguration"/> class.
        /// </summary>
        /// <param name="config"></param>
        public ApolloConfiguration(IConfiguration config)
        {
            //var serveConfig = config.GetServiceConfig().ServiceID;
            //_config = config;
            //_config = ApolloConfigurationManager.GetConfig($"{serveConfig}.{ConfigConsts.NamespaceApplication}.json",
            //    ConfigConsts.NamespaceApplication).GetAwaiter().GetResult();
            //_config.ConfigChanged += OnChanged;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="defaultValue">如果没找到key,则返回默认值</param>
        /// <returns></returns>
        public  string GetConfig(string pre,string key,string defaultValue)
        {
            key = $"{pre}-{key}";
            if (_config == null) return defaultValue;
            var result = _config.GetProperty(key, defaultValue);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="defaultValue">如果没找到key,则返回默认值</param>
        /// <returns></returns>
        public  bool GetConfig(string pre, string key, bool defaultValue)
        {
            key = $"{pre}-{key}";
            if (_config == null) return defaultValue;
            IConfig outCfg;
            var result = this._config.GetProperty(key);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result.HasValue ? result.Value : defaultValue;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="defaultValue">如果没找到key,则返回默认值</param>
        /// <returns></returns>
        public  int GetConfig(string pre, string key, int defaultValue)
        {
            key = $"{pre}-{key}";
            if (_config == null) return defaultValue;
            var result = _config.GetProperty(key, defaultValue);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result.HasValue ? result.Value : defaultValue;
        }

        /// <summary>
        /// 动态监听配置改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="changeEvent"></param>
        private static void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            if(OnChangeConfig!=null)
            {
                OnChangeConfig(null, changeEvent);
            }
            foreach (var kv in changeEvent.Changes)
            {
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}",
                    kv.Value.PropertyName, kv.Value.OldValue, kv.Value.NewValue, kv.Value.ChangeType);
            }
        }
    }
}
