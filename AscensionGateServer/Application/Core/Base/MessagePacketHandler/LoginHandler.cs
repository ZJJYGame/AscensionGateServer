using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using RedisService;

namespace AscensionGateServer
{
    public class LoginHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Login;
        MessagePacket messagePacket = new MessagePacket((byte)GateOperationCode._Login);
        string srvCfgFileName = "GameServerSetData.json";
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
                NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", obj.Account);
                NHCriteria nHCriteriaPassword = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", obj.Password);
                var verified=NHibernateQuery.Verify<User>(nHCriteriaAccount, nHCriteriaPassword);
                if (!verified)
                {
                    //验证失败则返回空
                    messagePacket.ReturnCode = (byte)GateReturnCode.Empty;
                    return messagePacket;
                }
                var token = JWTEncoder.EncodeToken(obj);
                messagePacket.Messages = new Dictionary<byte, object>() 
                {
                    { (byte)GateParameterCode.Token, token },
                };
                string dat;
                var hasDat = GameManager.OuterModule<ResourceManager>().TryGetValue(srvCfgFileName,out dat);
                if (hasDat)
                    messagePacket.Messages.Add((byte)GateParameterCode.ServerInfo, dat);
                messagePacket.ReturnCode = (byte)GateReturnCode.Success;
                Utility.Debug.LogWarning(obj.ToString());
                GameManager.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
            }
            else
            {
                messagePacket.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            return messagePacket;
        }
    }
}
