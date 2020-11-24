using Cosmos;
using Protocol;
using RedisService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    //=========================================
    //流程说明：
    //1、接收客户端发送的消息；解析UserInfo，先验证账号在数据库
    //是否重复，若重复，则返回ItemAlreadyExists，不重复则进入下
    //一个逻辑；
    //2、在数据库存储好账号数据后，为UserRole开辟一条数据空间；
    //3、Success部分，返回值带有验证成功后的Token、服务器列表；
    //=========================================
    /// <summary>
    /// 注册
    /// </summary>
    public class SignupHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Signup;
        //MessagePacket handlerPacket = new MessagePacket((byte)GateOperationCode._Signup);
        public async override Task<MessagePacket> HandleAsync(MessagePacket packet)
        {
            return await Task.Run(() =>
            {
                MessagePacket handlerPacket = GameManager.ReferencePoolManager.Spawn<MessagePacket>();
                handlerPacket.OperationCode = (byte)GateOperationCode._Signup;
                var packetMsg = packet.Messages;
                if (packetMsg == null)
                    return null;
                Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
                handlerPacket.Messages = messageDict;
                messageDict.Clear();
                object data;
                var result = packetMsg.TryGetValue((byte)GateParameterCode.UserInfo, out data);
                if (result)
                {
                    var userInfoObj = Utility.Json.ToObject<UserInfo>(Convert.ToString(data));
                    NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userInfoObj.Account);
                    User userObj = new User() { Account = userInfoObj.Account, Password = userInfoObj.Password };
                    bool isExist = NHibernateQuerier.Verify<User>(nHCriteriaAccount);
                    if (!isExist)
                    {
                        userObj = NHibernateQuerier.Insert(userObj);
                        NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);
                        bool userRoleExist = NHibernateQuerier.Verify<UserRole>(nHCriteriaUUID);
                        if (!userRoleExist)
                        {
                            var userRole = new UserRole() { UUID = userObj.UUID };
                            NHibernateQuerier.Insert(userRole);
                        }
                        var token = JWTEncoder.EncodeToken(userInfoObj);
                        //获取对应键值的key
                        var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPrefix;
                        {
                            TokenExpireData dat;
                            var hasDat = GameManager.CustomeModule<DataManager>().TryGetValue(out dat);
                            //更新过期时间；
                            if (!hasDat)//没数据则默认一周；
                            {
                                var t = RedisHelper.String.StringSetAsync(tokenKey, token, new TimeSpan(7, 0, 0, 0));
                            }
                            else
                            {
                                //有数据则使用数据周期；
                                var t = RedisHelper.String.StringSetAsync(tokenKey, token, new TimeSpan(dat.Days, dat.Hours, dat.Minutes, dat.Seconds));
                            }
                        }
                        {
                            messageDict.TryAdd((byte)GateParameterCode.Token, token);
                            string dat;
                            var hasDat = ApplicationBuilder.TryGetServerList(out dat);
                            if (hasDat)
                                packet.Messages.Add((byte)GateParameterCode.ServerInfo, dat);
                            handlerPacket.ReturnCode = (byte)GateReturnCode.Success;
                            messageDict.TryAdd((byte)GateParameterCode.User, Utility.Json.ToJson(userObj));
                        }
                        GameManager.ReferencePoolManager.Despawn(nHCriteriaUUID);
                        Utility.Debug.LogInfo($"Register user: {userObj}");
                    }
                    else
                    {
                        //账号存在
                        handlerPacket.ReturnCode = (byte)GateReturnCode.ItemAlreadyExists;
                    }
                    GameManager.ReferencePoolManager.Despawn(nHCriteriaAccount);
                }
                else
                {
                    //业务数据无效
                    handlerPacket.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
                }
                return handlerPacket;
            });
        }
    }
}
