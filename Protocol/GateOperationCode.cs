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
        public const ushort _Login = 1;
        public const ushort _Logoff =2 ;
        public const ushort _Signup = 3;
        /// <summary>
        /// 当前账号下的所有角色信息；
        /// </summary>
        public const ushort _AccountRoles = 4;
        public const ushort _Token= 243;
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
