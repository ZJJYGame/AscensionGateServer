using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public class MessagePacketEventCore:ConcurrentEventCore<byte,MessagePacket,MessagePacketEventCore>
    {
    }
}
