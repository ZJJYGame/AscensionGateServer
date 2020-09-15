using System;
using System.Collections.Generic;
using System.Text;

namespace RedisService
{
    [Serializable]
    public class RedisConfig
    {
        /// <summary>
        ///192.168.0.117:6379,password=123456,DefaultDatabase=3
        /// </summary>
        public string Configuration { get; set; }
        public override string ToString()
        {
            return "Configuration :" + Configuration;
        }
    }
}
