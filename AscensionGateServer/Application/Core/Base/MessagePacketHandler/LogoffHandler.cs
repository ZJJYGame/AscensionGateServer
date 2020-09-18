using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    //=========================================
    //流程说明：
    //
    //=========================================
    public class LogoffHandler: MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Logoff;
        MessagePacket messagePacket = new MessagePacket((byte)GateOperationCode._Logoff);
        public override MessagePacket Handle(MessagePacket packet)
        {
            return null;
        }
    }
}
