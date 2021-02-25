using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using Protocol;
using RedisService;

namespace AscensionGateServer
{
    [Module]
    public class LoginManager:Module, ILoginManager
    {
        public override void OnInitialization()
        {
            CommandEventCore.Instance.AddEventListener((byte)GateOperationCode._Login,ProcessLoginC2S);
            CommandEventCore.Instance.AddEventListener((byte)GateOperationCode._Signup,ProcessSignUpC2S);
            CommandEventCore.Instance.AddEventListener((byte)GateOperationCode._Token,ProcessTokenC2S);
            CommandEventCore.Instance.AddEventListener((byte)GateOperationCode._Logoff, ProcessLogoffC2S);
        }
        //=========================================
        //流程说明
        //1、接收客户端发送的消息；解析UserInfo，先验证账号。若验证
        //失败，则返回ItemNotFound。验证成功则进入下一个逻辑；
        //2、根据UserInfo生成token；
        //3、Success部分，返回值带有验证成功后的Token、服务器列表；
        //=========================================
        void ProcessLoginC2S(long conv, OperationData packet)
        {
            Utility.Debug.LogInfo($"LoginHandler Conv:{conv}尝试登陆");
            var opData = new OperationData() { OperationCode = GateOperationCode._Login };
            var packetMsg = Utility.Json.ToObject<Dictionary<byte,object>> (Convert.ToString( packet.DataMessage));
            if (packetMsg == null)
                return;
            Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
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
                    opData.ReturnCode = (byte)GateReturnCode.ItemNotFound;
                    Utility.Debug.LogWarning($"LoginHandler Conv:{conv}登陆失败，账号无效！");
                }
                else
                {
                    var token = JWTEncoder.EncodeToken(userInfoObj);
                    //获取对应键值的key
                    var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPostfix;
                    {
                        TokenExpireData dat;
                        var hasDat = ServerEntry.DataManager.TryGetValue(out dat);
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
                    opData.ReturnCode = (byte)GateReturnCode.Success;
                    Utility.Debug.LogInfo($"Conv{conv} : {userInfoObj}");
                    CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
                }
            }
            else
            {
                //业务数据无效
                opData.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            opData.DataMessage = Utility.Json.ToJson(messageDict);
            ServerEntry.NetMessageManager.SendMessageAsync(conv, opData);
        }
        //=========================================
        //流程说明：
        //1、接收客户端发送的消息；解码token，生成token键值，redis
        //前缀查询验证；
        //2、验证在redis中的token。若存在，
        //则返回服务器列表，且重置Redis的过期时间；若不存在，则返回
        //ItemNotFound；
        //=========================================
        void ProcessTokenC2S(long conv,OperationData packet)
        {
            Utility.Debug.LogInfo($"TokenHandler Conv:{conv}尝试Token");
            var opData = new OperationData() { OperationCode = GateOperationCode._Token};
            opData.OperationCode = (byte)GateOperationCode._Token;
            var packetMsg = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(packet.DataMessage));
            //var packetMsg = (Dictionary<byte,object>)packet.DataMessage;
            if (packetMsg == null)
                return;
            Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
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
                    Utility.Debug.LogError($"Conv:{conv} token 解码失败");
                }
                //解码token
                if (string.IsNullOrEmpty(dataStr))
                {
                    opData.ReturnCode = (short)GateReturnCode.Fail;
                    Utility.Debug.LogWarning($"Conv:{conv}  {(GateReturnCode)opData.ReturnCode}");
                    return;
                }
                //反序列化为数据对象
                var userInfoObj = Utility.Json.ToObject<UserInfo>(dataStr);
                Utility.Debug.LogInfo($"TokenHandler Conv:{conv} UserInfo:{userInfoObj}");
                //组合键值
                var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPostfix;
                //获取对应键值的key
                var tokenContext = RedisHelper.String.StringGet(tokenKey);
                if (string.IsNullOrEmpty(tokenContext))
                {
                    opData.ReturnCode = (byte)GateReturnCode.Empty;
                }
                else
                {
                    if (data.ToString() == tokenContext)
                    {
                        opData.ReturnCode = (byte)GateReturnCode.Success;
                        {
                            //添加服务器列表数据;
                            string dat;
                            var hasDat = ApplicationBuilder.TryGetServerList(out dat);
                            if (hasDat)
                                messageDict.TryAdd((byte)GateParameterCode.ServerInfo, dat);
                        }
                        {
                            TokenExpireData dat;
                            var hasDat = ServerEntry.DataManager.TryGetValue(out dat);
                            //更新过期时间；
                            if (!hasDat)//没数据则默认一周；
                                RedisHelper.KeyExpire(data.ToString(), new TimeSpan(7, 0, 0, 0));
                            else
                            {
                                //有数据则使用数据周期；
                                var srcDat = dat as TokenExpireData;
                                RedisHelper.KeyExpire(data.ToString(), new TimeSpan(srcDat.Days, srcDat.Hours, srcDat.Minutes, srcDat.Seconds));
                            }
                        }
                        NHCriteria nHCriteriaAccount = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userInfoObj.Account);
                        NHCriteria nHCriteriaPassword = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userInfoObj.Password);
                        var userObj = NHibernateQuerier.CriteriaSelect<User>(nHCriteriaAccount, nHCriteriaPassword);
                        messageDict.TryAdd((byte)GateParameterCode.User, Utility.Json.ToJson(userObj));
                        CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
                        Utility.Debug.LogInfo($"Conv:{conv} Token decoded message success {userObj}");
                    }
                    else
                    {
                        //验证失败，返回fail
                        opData.ReturnCode = (byte)GateReturnCode.ItemNotFound;
                    }
                }
            }
            else
            {
                //业务数据无效
                opData.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            opData.DataMessage = Utility.Json.ToJson( messageDict);
            ServerEntry.NetMessageManager.SendMessageAsync(conv, opData);
        }
        //=========================================
        //流程说明：
        //1、接收客户端发送的消息；解析UserInfo，先验证账号在数据库
        //是否重复，若重复，则返回ItemAlreadyExists，不重复则进入下
        //一个逻辑；
        //2、在数据库存储好账号数据后，为UserRole开辟一条数据空间；
        //3、Success部分，返回值带有验证成功后的Token、服务器列表；
        //=========================================
        void ProcessSignUpC2S(long conv, OperationData packet)
        {
            Utility.Debug.LogInfo($"SignupHandler Conv:{conv}尝试注册");
            var opData = new OperationData() { OperationCode =  GateOperationCode._Signup};
            opData.OperationCode = (byte)GateOperationCode._Signup;
            var packetMsg = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(packet.DataMessage));
            //var packetMsg = (Dictionary<byte, object>)packet.DataMessage;
            if (packetMsg == null)
                return;
            Dictionary<byte, object> messageDict = new Dictionary<byte, object>();
            //opData.DataMessage = messageDict;
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.UserInfo, out data);
            if (result)
            {
                var userInfoObj = Utility.Json.ToObject<UserInfo>(Convert.ToString(data));
                Utility.Debug.LogInfo($"SignupHandler Conv:{conv} UserInfo:{userInfoObj}");
                NHCriteria nHCriteriaAccount = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userInfoObj.Account);
                User userObj = new User() { Account = userInfoObj.Account, Password = userInfoObj.Password };
                bool isExist = NHibernateQuerier.Verify<User>(nHCriteriaAccount);
                if (!isExist)
                {
                    userObj = NHibernateQuerier.Insert(userObj);
                    NHCriteria nHCriteriaUUID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);
                    bool userRoleExist = NHibernateQuerier.Verify<UserRole>(nHCriteriaUUID);
                    if (!userRoleExist)
                    {
                        var userRole = new UserRole() { UUID = userObj.UUID };
                        NHibernateQuerier.Insert(userRole);
                    }
                    var token = JWTEncoder.EncodeToken(userInfoObj);
                    //获取对应键值的key
                    var tokenKey = userInfoObj.Account + ApplicationBuilder._TokenPostfix;
                    {
                        TokenExpireData dat;
                        var hasDat = ServerEntry.DataManager.TryGetValue(out dat);
                        //更新过期时间；
                        if (!hasDat)//没数据则默认一周；
                        {
                            var t = RedisHelper.String.StringSet(tokenKey, token, new TimeSpan(7, 0, 0, 0));
                        }
                        else
                        {
                            //有数据则使用数据周期；
                            var t = RedisHelper.String.StringSet(tokenKey, token, new TimeSpan(dat.Days, dat.Hours, dat.Minutes, dat.Seconds));
                        }
                    }
                    {
                        messageDict.TryAdd((byte)GateParameterCode.Token, token);
                        string dat;
                        var hasDat = ApplicationBuilder.TryGetServerList(out dat);
                        if (hasDat)
                            messageDict.Add((byte)GateParameterCode.ServerInfo, dat);
                        opData.ReturnCode = (byte)GateReturnCode.Success;
                        messageDict.TryAdd((byte)GateParameterCode.User, Utility.Json.ToJson(userObj));
                    }
                    CosmosEntry.ReferencePoolManager.Despawn(nHCriteriaUUID);
                    Utility.Debug.LogInfo($"Conv:{conv} Register user: {userObj}");
                }
                else
                {
                    //账号存在
                    opData.ReturnCode = (byte)GateReturnCode.ItemAlreadyExists;
                }
                CosmosEntry.ReferencePoolManager.Despawn(nHCriteriaAccount);
            }
            else
            {
                //业务数据无效
                opData.ReturnCode = (byte)GateReturnCode.InvalidOperationParameter;
            }
            opData.DataMessage = Utility.Json.ToJson(messageDict);
            ServerEntry.NetMessageManager.SendMessageAsync(conv, opData);
        }
        //=========================================
        //流程说明：
        //暂定
        //=========================================
        void ProcessLogoffC2S(long conv, OperationData packet) { }
    }
}
