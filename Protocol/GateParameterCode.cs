using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public enum GateParameterCode:byte
    {
        /// <summary>
        /// 包含账户信息以及设备号的数据
        /// </summary>
        UserInfo = 3,
        Token = 4
    }
}
