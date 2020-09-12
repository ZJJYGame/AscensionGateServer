using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    public class JWTTokenHelper : IJWTMessageHelper
    {
        public MessagePacket DecodeMessage(byte[] buffer)
        {
            return MessagePacket.Deserialize(buffer);
        }
        /// <summary>
        /// 若验证成功，则返回token，若失败，则返回ReturnCode false
        /// </summary>
        /// <param name="netMsg">派发进来的网络消息体</param>
        public async Task<MessagePacket> EncodeMessage(INetworkMessage netMsg)
        {
            var plaintext = Encryptor.Decrypt(netMsg.ServiceMsg);
            User userObj = Utility.Json.ToObject<User>(plaintext);
            NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            NHCriteria nHCriteriaPassword = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userObj.Password);
            bool verified = await GameManager.External.GetModule<NHManager>().VerifyAsync<User>(nHCriteriaAccount, nHCriteriaPassword);
            MessagePacket mp = new MessagePacket();
            mp.OperationCode = Convert.ToByte(GateOperationCode._Login);
            if (verified)
            {
                Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
                var tokenPlaintxt = await GameManager.External.GetModule<JWTManager>().EncodeTokenAsync(userObj);
                dataDict.Add(Convert.ToByte(GateParameterCode.Token), Encryptor.EncryptToken(tokenPlaintxt));
                mp.SetMessages(dataDict);
                mp.ReturnCode = Convert.ToInt16(GateReturnCode.Success);
            }
            else
            {
                mp.OperationCode = Convert.ToByte(GateOperationCode._Login);
                mp.ReturnCode = Convert.ToInt16(GateReturnCode.Fail);
                UdpNetMessage udpNetMsg = netMsg as UdpNetMessage;
            }
            return mp;
        }
    }
}
