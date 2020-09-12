using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class GateOperationCode
    {
        #region MessagePack Opcode
        public static readonly ushort _Login = 1;
        public static readonly ushort _LoginOff =2 ;
        public static readonly ushort _Token= 243;
        #endregion



        #region System Opcode
        //系统通道，请勿修改！
        /// <summary>
        /// 系统通讯通道opcode号
        /// </summary>
        public static readonly ushort _MSG= 4319;
        #endregion
    }
}
