using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    public interface IJWTMessageHelper
    {
        Task<MessagePacket> EncodeMessage(INetworkMessage netMsg);
        MessagePacket DecodeMessage(byte[] buffer);
    }
}
