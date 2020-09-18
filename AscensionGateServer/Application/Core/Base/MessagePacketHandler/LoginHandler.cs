using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using RedisService;

namespace AscensionGateServer
{
    //=========================================
    //流程说明
    //1、接收客户端发送的消息；解析UserInfo，先验证账号。若验证
    //失败，则返回ItemNotFound。验证成功则进入下一个逻辑；
    //2、根据UserInfo生成token；
    //3、Success部分，返回值带有验证成功后的Token、服务器列表；
    //=========================================
    public class LoginHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Login;
        MessagePacket packet = new MessagePacket((byte)GateOperationCode._Login);
        Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
        public LoginHandler()
        {
            packet.Messages = messageDict;
        }
        public override MessagePacket Handle(MessagePacket packet)
        {
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return null;
            messageDict.Clear();
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.UserInfo, out data);
            if (result)
            {
                var userInfoObj = Utility.Json.ToObject<UserInfo>(Convert.ToString(data));
                NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userInfoObj.Account);
                NHCriteria nHCriteriaPassword = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userInfoObj.Password);
                var verified=NHibernateQuery.Verify<User>(nHCriteriaAccount, nHCriteriaPassword);
                if (!verified)
                {
                    //验证失败则返回空
                    this.packet.ReturnCode = (byte)GateReturnCode.ItemNotFound;
                    return this.packet;
                }
                var token = JWTEncoder.EncodeToken(userInfoObj);
                //获取对应键值的key
                var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPrefix;
                {
                    TokenExpireData dat;
                    var hasDat = GameManager.OuterModule<DataManager>().TryGetValue(out dat);
                    //更新过期时间；
                    if (!hasDat)//没数据则默认一周；
                    {
                        var t= RedisHelper.String.StringSetAsync(tokenKey, token, new TimeSpan(7, 0, 0));
                    }
                    else
                    {
                        //有数据则使用数据周期；
                        var srcDat = dat as TokenExpireData;
                       var t= RedisHelper.String.StringSetAsync(tokenKey, new TimeSpan(srcDat.Days, srcDat.Minutes, srcDat.Seconds));
                    }
                }
                messageDict.Add((byte)GateParameterCode.Token, token);
                {
                    string dat;
                    var hasDat = ApplicationBuilder.TryGetServerList(out dat);
                    if (hasDat)
                    {
                        messageDict.Add((byte)GateParameterCode.ServerInfo, dat);
                    }
                }
                this.packet.ReturnCode = (byte)GateReturnCode.Success;
                Utility.Debug.LogWarning(userInfoObj.ToString());
                GameManager.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
            }
            else
            {
                //业务数据无效
                this.packet.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            return this.packet;
        }
    }
}
