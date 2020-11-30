using NHibernate.Engine.Query;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    //=========================================
    //流程说明：
    //
    //=========================================
    public class LogoffHandler: MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Logoff;
        MessagePacket handlerPacket = new MessagePacket((byte)GateOperationCode._Logoff);
    }
}
