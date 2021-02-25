using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using RedisService;
using System.Threading.Tasks;

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
        public async override void HandleAsync(long conv, MessagePacket packet)
        {
            Utility.Debug.LogInfo($"LoginHandler Conv:{conv}尝试登陆");
            MessagePacket handlerPacket = CosmosEntry.ReferencePoolManager.Spawn<MessagePacket>();
            handlerPacket.OperationCode = (byte)GateOperationCode._Login;
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return;
            Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
            handlerPacket.Messages = messageDict;
            messageDict.Clear();
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.UserInfo, out data);
            if (result)
            {
                var userInfoObj = Utility.Json.ToObject<UserInfo>(Convert.ToString(data));
                Utility.Debug.LogInfo($"LoginHandler Conv:{conv} UserInfo:{userInfoObj}");
                NHCriteria nHCriteriaAccount = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userInfoObj.Account);
                NHCriteria nHCriteriaPassword = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userInfoObj.Password);
                var userObj = NHibernateQuerier.CriteriaSelect<User>(nHCriteriaAccount, nHCriteriaPassword);
                var verified = (userObj != null);
                if (!verified)
                {
                    //验证失败则返回空
                    handlerPacket.ReturnCode = (byte)GateReturnCode.ItemNotFound;
                    Utility.Debug.LogWarning($"LoginHandler Conv:{conv}登陆失败，账号无效！");
                }
                else
                {
                    var token = JWTEncoder.EncodeToken(userInfoObj);
                    //获取对应键值的key
                    var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPostfix;
                    {
                        TokenExpireData dat;
                        var hasDat =  ServerEntry.DataManager.TryGetValue(out dat);
                        //更新过期时间；
                        if (!hasDat)//没数据则默认一周；
                        {
                            var t = RedisHelper.String.StringSet(tokenKey, token, new TimeSpan(7, 0, 0, 0));
                        }
                        else
                        {
                            //有数据则使用数据周期；
                            var srcDat = dat as TokenExpireData;
                            var t = RedisHelper.String.StringSet(tokenKey, token, new TimeSpan(srcDat.Days, srcDat.Hours, srcDat.Minutes, srcDat.Seconds));
                        }
                    }
                    messageDict.TryAdd((byte)GateParameterCode.Token, token);
                    {
                        string dat;
                        var hasDat = ApplicationBuilder.TryGetServerList(out dat);
                        if (hasDat)
                        {
                            messageDict.TryAdd((byte)GateParameterCode.ServerInfo, dat);
                        }
                    }
                    messageDict.TryAdd((byte)GateParameterCode.User, Utility.Json.ToJson(userObj));
                    handlerPacket.ReturnCode = (byte)GateReturnCode.Success;
                    Utility.Debug.LogInfo($"Conv{conv} : {userInfoObj}");
                    CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
                }
            }
            else
            {
                //业务数据无效
                handlerPacket.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            //ServerEntry.NetMessageManager.SendMessageAsync(conv, handlerPacket);
        }
    }
}
