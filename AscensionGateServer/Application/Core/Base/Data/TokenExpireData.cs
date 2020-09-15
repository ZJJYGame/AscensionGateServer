using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    /// <summary>
    /// token过期配置数据
    /// </summary>
    [Serializable]
    public class TokenExpireData : IData
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public void SetData(object data)
        {
            var rtcd = data as TokenExpireData;
            Days = rtcd.Days;
            Hours= rtcd.Hours;
            Minutes= rtcd.Minutes;
            Seconds= rtcd.Seconds;
        }
    }
}
