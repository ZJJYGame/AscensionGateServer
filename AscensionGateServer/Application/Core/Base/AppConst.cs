using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public class AppConst
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
        static byte[] kcpIV;
        /// <summary>
        /// kcp初始向量
        /// </summary>
        public static byte[] KcpIV
        {
            get
            {
                if (kcpIV == null)
                    kcpIV = Utility.Encryption.GenerateIV("ATID-318");
                return kcpIV;
            }
        }
    }
}
