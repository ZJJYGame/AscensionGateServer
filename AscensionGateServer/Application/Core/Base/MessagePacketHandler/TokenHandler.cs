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
        MessagePacket messagePacket = new MessagePacket((byte)GateOperationCode._Token);
        string serverCfgFileName = "GameServerSetData.json";
        public override MessagePacket Handle(MessagePacket packet)
        {
            var packetMsg = packet.Messages;
            if (packetMsg == null)
                return null;
            messagePacket.Messages = null;
            object data;
            var result = packetMsg.TryGetValue((byte)GateParameterCode.Token, out data);
            if (result)
            {
                var keyExist = RedisHelper.KeyExistsAsync(data.ToString()).Result;
                if (keyExist)
                {
                    messagePacket.ReturnCode = (byte)GateReturnCode.Success;
                    messagePacket.Messages = new Dictionary<byte, object>();
                    {
                        string dat;
                        var hasDat= GameManager.OuterModule<ResourceManager>().TryGetValue(serverCfgFileName,out dat);
                        if (hasDat)
                        {
                            try
                            {
                                messagePacket.Messages.Add((byte)GateParameterCode.ServerInfo, dat);
                            }
                            catch (Exception e)
                            {
                                Utility.Debug.LogError($"序列化失败 : {e}");
                            }
                        }
                    }
                    {
                        TokenExpireData dat;
                        var hasDat = GameManager.OuterModule<DataManager>().TryGetValue(out dat);
                        //更新过期时间；
                        if (!hasDat)
                            //没数据则默认一周；
                            RedisHelper.KeyExpire(data.ToString(), new TimeSpan(7, 0, 0));
                        else
                        {
                            //有数据则使用数据周期；
                            var srcDat= dat as TokenExpireData;
                            RedisHelper.KeyExpire(data.ToString(), new TimeSpan(srcDat.Days, srcDat.Minutes, srcDat.Seconds));
                        }
                    }
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
