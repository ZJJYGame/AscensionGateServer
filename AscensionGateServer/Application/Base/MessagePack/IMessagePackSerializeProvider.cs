using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface IMessagePackSerializeProvider
    {
        byte[] Serialize(MessagePacket msgPack);
        MessagePacket Deserialize(byte[] data);
    }
}
