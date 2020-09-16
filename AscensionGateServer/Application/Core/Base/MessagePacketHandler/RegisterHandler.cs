using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public class RegisterHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Register;
        MessagePacket messagePacket = new MessagePacket((byte)GateOperationCode._Register);
        public override MessagePacket Handle(MessagePacket packet)
        {
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return null;
            messagePacket.Messages = null;
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.User, out data);
            if (result)
            {
                var userObj = Utility.Json.ToObject<User>(Convert.ToString(data));
                NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
                bool isExist = NHibernateQuery.Verify<User>(nHCriteriaAccount);
                if (!isExist)
                {
                    userObj = NHibernateQuery.Insert(userObj);
                    NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);
                    bool userRoleExist =NHibernateQuery.Verify<UserRole>(nHCriteriaUUID);
                    if (!userRoleExist)
                    {
                        var userRole = new UserRole() { UUID = userObj.UUID };
                        NHibernateQuery.Insert(userRole);
                    }
                    messagePacket.ReturnCode = (byte)GateReturnCode.Success;
                    GameManager.ReferencePoolManager.Despawn(nHCriteriaUUID);
                    Utility.Debug.LogInfo($"Register account: {userObj}");
                }
                GameManager.ReferencePoolManager.Despawn(nHCriteriaAccount);
            }
            else
            {
                messagePacket.ReturnCode = (byte)GateReturnCode.Fail;
            }
            return messagePacket;
        }
    }
}
