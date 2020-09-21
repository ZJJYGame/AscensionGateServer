using NHibernate.Proxy;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using RedisService;
using System.Net.WebSockets;

namespace AscensionGateServer
{
    //=========================================
    //流程说明：
    //1、接收客户端发送的消息；解码token，生成token键值，redis
    //前缀查询验证；
    //2、验证在redis中的token。若存在，
    //则返回服务器列表，且重置Redis的过期时间；若不存在，则返回
    //ItemNotFound；
    //=========================================
    public class TokenHandler : MessagePacketHandler
    {
        public override ushort OpCode { get; protected set; } = GateOperationCode._Token;
        MessagePacket handlerPacket = new MessagePacket((byte)GateOperationCode._Token);
        Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
        public TokenHandler()
        {
            handlerPacket.Messages = messageDict;
        }
        public override MessagePacket Handle(MessagePacket packet)
        {
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return null;
            messageDict.Clear();
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.Token, out data);
            if (result)
            {
                string dataStr = null;
                try
                {
                    dataStr = JWTEncoder.DecodeToken(data.ToString());
                }
                catch (Exception)
                {
                    Utility.Debug.LogError("token 解码失败");
                }
                //解码token
                if (string.IsNullOrEmpty(dataStr))
                {
                    this.handlerPacket.ReturnCode = (short)GateReturnCode.Fail;
                    Utility.Debug.LogWarning((GateReturnCode)this.handlerPacket.ReturnCode);
                    return this.handlerPacket;
                }
                //反序列化为数据对象
                var userInfoObj = Utility.Json.ToObject<UserInfo>(dataStr);
           
                //组合键值
                var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPrefix;
                //获取对应键值的key
                var tokenContext = RedisHelper.String.StringGetAsync(tokenKey).Result;
                if (string.IsNullOrEmpty(tokenContext))
                {
                    this.handlerPacket.ReturnCode = (byte)GateReturnCode.Empty;
                }
                else
                {
                    if (data.ToString() == tokenContext)
                    {
                        this.handlerPacket.ReturnCode = (byte)GateReturnCode.Success;
                        {
                            //添加服务器列表数据;
                            string dat;
                            var hasDat = ApplicationBuilder.TryGetServerList(out dat);
                            if (hasDat)
                                messageDict.Add((byte)GateParameterCode.ServerInfo, dat);
                        }
                        {
                            TokenExpireData dat;
                            var hasDat = GameManager.CustomeModule<DataManager>().TryGetValue(out dat);
                            //更新过期时间；
                            if (!hasDat)//没数据则默认一周；
                                RedisHelper.KeyExpire(data.ToString(), new TimeSpan(7, 0, 0));
                            else
                            {
                                //有数据则使用数据周期；
                                var srcDat = dat as TokenExpireData;
                                RedisHelper.KeyExpire(data.ToString(), new TimeSpan(srcDat.Days, srcDat.Minutes, srcDat.Seconds));
                            }
                        }
                        NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userInfoObj.Account);
                        NHCriteria nHCriteriaPassword = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userInfoObj.Password);
                        var userObj = NHibernateQuery.CriteriaSelect<User>(nHCriteriaAccount, nHCriteriaPassword);
                        messageDict.Add((byte)GateParameterCode.User, userObj);
                        GameManager.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
                    }
                    else
                    {
                        //验证失败，返回fail
                        this.handlerPacket.ReturnCode = (byte)GateReturnCode.ItemNotFound;
                    }
                }
            }
            else
            {
                //业务数据无效
                this.handlerPacket.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            return this.handlerPacket;
        }
    }
}
