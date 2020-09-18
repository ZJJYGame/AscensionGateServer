using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.IO;

namespace RedisService
{
    public class RedisManager : ConcurrentSingleton<RedisManager>
    {
        /// <summary>
        /// 连接配置
        ///// </summary>
        //readonly string ConnectStr = "192.168.0.117:6379,password=123456,DefaultDatabase=3";
        //readonly string ConnectStr = "121.37.185.220:6379,password=123456,DefaultDatabase=0";
        //readonly string ConnectStr = "127.0.0.1:6379,password=jygame_%Redis,DefaultDatabase=0";
        /// <summary>
        /// Redis保存数据时候key的前缀
        /// </summary>
        internal static readonly string RedisKeyPrefix = "JY_";
        /// <summary>
        /// Redis对象
        /// </summary>
        public ConnectionMultiplexer Redis { get; private set; }
        public IServer[] RedisServers { get; private set; }
        /// <summary>
        /// Redis中的DB
        /// </summary>
        public IDatabase RedisDB { get; private set; }
        string folderPath = Environment.CurrentDirectory + "/RedisConfigData";
        RedisConfig redisConfig;
        public void OnInitialization()
        {
            LoadCfg();
            OnPreparatory();
        }
       void OnPreparatory()
        {
            try
            {
                Redis = ConnectionMultiplexer.Connect(redisConfig.Configuration);
                if (Redis == null)
                {
                    Utility.Debug.LogError(new ArgumentNullException("Redis Connect Fail"));
                    return;
                }
                else
                {
                    Utility.Debug.LogInfo("RedisService Connected");
                    List<IServer> servers = new List<IServer>();
                    foreach (var endPoint in Redis.GetEndPoints())
                    {
                        
                        var server = Redis.GetServer(endPoint);
                        servers.Add(server);
                    }
                    RedisServers = servers.ToArray();
                }
                RedisDB = Redis.GetDatabase();
            }
            catch (Exception)
            {
                Utility.Debug.LogError(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis Connect Fail"));
            }
        }
        void LoadCfg()
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (var f in dir.GetFiles())
            {
                if(f.Name== "RedisConfig.json")
                {
                    var str = Utility.IO.ReadTextFileContent(folderPath, f.Name);
                    try
                    {
                        redisConfig = Utility.Json.ToObject<RedisConfig>(str);
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError($"Redis ToObject fail . Type : {e}");
                    }
                }
            }
        }
    }
}
