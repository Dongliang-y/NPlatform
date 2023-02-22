using NPlatform.Infrastructure.Config;
using StackExchange.Redis;

namespace NPlatform.Infrastructure.Redis
{
    /// <summary>
    /// ConnectionMultiplexer对象管理帮助类
    /// </summary>
    public static class RedisConnection
    {
        /// <summary>
        /// 集群或者哨兵模式时，必须实现的委托。
        /// </summary>
        public static Action<RedisChannel, RedisValue> Switch_master = null;

        private static ISubscriber subscriber = null;
        private static readonly object Locker = new object();
        private static ConnectionMultiplexer instance;

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer CreateInstance(IRedisConfig Config)
        {
            if (instance == null)
            {
                lock (Locker)
                {
                    if (instance == null || !instance.IsConnected)
                    {
                        instance = GetManager(Config);
                    }
                }
            }

            return instance;
        }

        private static ConnectionMultiplexer GetManager(IRedisConfig Config)
        {
            if (Config.Connections == null || Config.Connections.Length == 0)
            {
                throw new Exception("Redis连接配置错误，无任何连接信息。");
            }

            ConfigurationOptions option = new ConfigurationOptions();
            option.DefaultDatabase = Config.dbNum;
            switch (Config.RedisType.ToLower().Trim())
            {
                case "twemproxy":
                    if (Switch_master == null)
                    {
                        throw new Exception($"集群或者哨兵模式时，必须实现RedisConnectionManager.Switch_master委托！");
                    }

                    foreach (var connStr in Config.Connections)
                    {
                        option.EndPoints.Add(connStr);
                    }

                    option.Proxy = Proxy.Twemproxy; //代理的类型

                    break;
                case "sentinel":
                    if (Switch_master == null)
                    {
                        throw new Exception($"集群或者哨兵模式时，必须实现RedisConnectionManager.Switch_master委托！");
                    }

                    option.ServiceName = "master1";
                    foreach (var connStr in Config.Connections)
                    {
                        option.EndPoints.Add(connStr);
                    }
                    option.TieBreaker = ""; //这行在sentinel模式必须加上
                    option.CommandMap = CommandMap.Sentinel;
                    // Need Version 3.0 for the INFO command?
                    option.DefaultVersion = new Version(3, 0);
                    break;
                default:
                    option.EndPoints.Add(Config.Connections[0]);
                    break;
            }

            option.Password = Config.Password;
            option.AllowAdmin = Config.AllowAdmin;
            ConnectionMultiplexer connect = ConnectionMultiplexer.Connect(option);
            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            if (Switch_master != null)
            {
                subscriber = connect.GetSubscriber();
                subscriber.Subscribe("+switch-master", Switch_master);
            }

            return connect;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redis: Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redis: ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redis: ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redis: 重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType +
                              (e.Exception == null ? "" : ", " + e.Exception.Message));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redis: HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redis: InternalError:Message" + e.Exception.Message);
        }

        #endregion 事件
    }
}

