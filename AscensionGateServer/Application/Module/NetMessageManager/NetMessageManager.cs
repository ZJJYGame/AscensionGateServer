using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
namespace AscensionGateServer
{
    public class NetMessageManager : Module<NetMessageManager>
    {
        public override void OnInitialization()
        {
            NetworkMsgEventCore.Instance.AddEventListener(GateOperationCode._LoginVerify, UserHandler);
            MessagePacket.SetProvider(new MessagePackJsonProvider());
        }
        /// <summary>
        /// 若验证成功，则返回token，若失败，则返回ReturnCode false
        /// </summary>
        /// <param name="netMsg">派发进来的网络消息体</param>
        async void UserHandler(INetworkMessage netMsg)
        {
            var plaintext = Encryption.Decrypt(netMsg.GetBuffer());
            User userObj = Utility.Json.ToObject<User>(plaintext);
            NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            NHCriteria nHCriteriaPassword = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userObj.Password);
            bool verified = await GameManager.External.GetModule<NHManager>().VerifyAsync<User>(nHCriteriaAccount, nHCriteriaPassword);
            if (verified)
            {
                MessagePacket mp = new MessagePacket();
                mp.OperationCode = Convert.ToByte(GateOperationCode._LoginVerify);
                Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
                dataDict.Add(Convert.ToByte(GateParameterCode.Token), Encryption.Token(plaintext));
                mp.SetParameters(dataDict);
                mp.ReturnCode = Convert.ToInt16(GateReturnCode.Success);
                UdpNetMessage udpNetMsg = netMsg as UdpNetMessage;
                var encodedMsg = await EncodeMessage(udpNetMsg, mp);
                GameManager.NetworkManager.SendNetworkMessage(encodedMsg);
            }
            else
            {
                MessagePacket mp = new MessagePacket();
                mp.OperationCode = Convert.ToByte(GateOperationCode._LoginVerify);
                mp.ReturnCode = Convert.ToInt16(GateReturnCode.Fail);
                UdpNetMessage udpNetMsg = netMsg as UdpNetMessage;
                var encodedMsg = await EncodeMessage(udpNetMsg, mp);
                GameManager.NetworkManager.SendNetworkMessage(encodedMsg);
            }
        }
        async Task<UdpNetMessage> EncodeMessage(UdpNetMessage udpNetMsg, MessagePacket mp)
        {
            return await Task.Run<UdpNetMessage>(() =>
            {
                var data = MessagePacket.Serialize(mp);
                UdpNetMessage msg = new UdpNetMessage(udpNetMsg, data);
                return msg;
            });
        }
    }
}
