using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface IMessagePackSerializeHelper
    {
        byte[] Serialize(MessagePacket msgPack);
        MessagePacket Deserialize(byte[] data);
    }
}
