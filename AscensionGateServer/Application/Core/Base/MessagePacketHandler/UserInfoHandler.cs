﻿using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using RedisService;

namespace AscensionGateServer
{
    public class UserInfoHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._UserInfo;
        MessagePacket messagePacket = new MessagePacket((byte)GateOperationCode._UserInfo);
        public override MessagePacket Handle(MessagePacket packet)
        {
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return null;
            messagePacket.Messages = null;
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.UserInfo, out data);
            if (result)
            {
                var obj = Utility.Json.ToObject<UserInfo>(Convert.ToString(data));
                var token = GameManager.External.GetModule<JWTManager>().EncodeToken(obj);
                messagePacket.Messages = new Dictionary<byte, object>() { { (byte)GateOperationCode._UserInfo, token} };
                messagePacket.ReturnCode = (byte)GateReturnCode.Success;
                Utility.Debug.LogWarning(obj.ToString());
            }
            else
            {
                messagePacket.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            return messagePacket;
        }
    }
}