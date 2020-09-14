using NHibernate.Proxy;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using RedisService;
namespace AscensionGateServer
{
    public class TokenHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Token;
        MessagePacket messagePacket=new MessagePacket((byte)GateOperationCode._Token);
        public override MessagePacket Handle(MessagePacket packet)
        {
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return null;
            messagePacket.Messages = null;
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.Token, out data);
            if(result)
            {
                var exist= RedisHelper.KeyExistsAsync(data.ToString()).Result;
                if (exist)
                {
                    messagePacket.ReturnCode = (byte)GateReturnCode.Success;
                }
                else
                {
                    messagePacket.ReturnCode = (byte)GateReturnCode.Fail;
                }
                Utility.Debug.LogWarning(data);
            }
            else
            {
                messagePacket.ReturnCode = (byte)GateReturnCode.Fail;
            }
            return messagePacket;
        }
    }
}
