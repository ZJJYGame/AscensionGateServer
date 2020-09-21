using Cosmos;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;

namespace AscensionGateServer
{
    public class ApplicationBuilder:IApplicationBuilder
    {
        /// <summary>
        /// token的8位密钥
        /// </summary>
        public static readonly string _TokenSecretKey = "ws123456";
        /// <summary>
        /// kcp通讯的密钥
        /// </summary>
        public static readonly string _KcpSecretKey = "JYGameEpoch";
        /// <summary>
        /// JWT的token key；
        /// </summary>
        public static readonly string _JWTTokenKey = "TokenInfo";
        /// <summary>
        /// kcp初始向量
        /// </summary>
        public static readonly byte[] _KcpIV;
        #region Prefix
        /// <summary>
        /// redis的key
        /// </summary>
        public static readonly string _TokenPrefix = "_TOKEN";
        #endregion
        static readonly string srvCfgFileName = "GameServerSetData.json";
        static ApplicationBuilder()
        {
            _KcpIV = Utility.Encryption.GenerateIV("ATID-318");
        }
        public static bool TryGetServerList(out string data)
        {
            var hasData = GameManager.CustomeModule<ResourceManager>().TryGetValue(srvCfgFileName, out data);
            return hasData;
        }
    }
}
