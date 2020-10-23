using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    [ImplementProvider]
    /// <summary>
    /// 消息加密；
    /// </summary>
    public class GateNetMsgEncryptHelper : INetMessageEncryptHelper
    {
        //TODO  GateNetMsgEncryptHelper 消息未加密
        public bool Serialize(MessagePacket packet, out byte[] messageBuffer)
        {
            messageBuffer = Utility.MessagePack.ToByteArray(packet);
            if (messageBuffer != null)
                return true;
            else
                return false;
        }
        public bool Deserialize(byte[] messageBuffer, out MessagePacket packet)
        {
            packet= Utility.MessagePack.ToObject<MessagePacket>(messageBuffer);
            if (packet != null)
                return true;
            else
                return false;
        }
    }
}
