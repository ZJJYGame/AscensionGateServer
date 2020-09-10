using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public class AppConst
    {
        public static readonly string _TokenSecretKey = "ws123456";
        public static readonly string _KcpSecretKey = "JYGameEpoch";
        public static readonly string _TokenInfo = "TokenInfo";
        static byte[] kcpIV;
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
