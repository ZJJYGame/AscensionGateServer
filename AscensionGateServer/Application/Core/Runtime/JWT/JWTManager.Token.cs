using Cosmos;
using System.Threading.Tasks;
using Protocol;
namespace AscensionGateServer
{
    public sealed partial class JWTManager : Module<JWTManager>
    {
        public override void OnInitialization()
        {
            NetworkMsgEventCore.Instance.AddEventListener(GateOperationCode._Token, TokenHandler);
            Utility.Debug.LogInfo("JWT OnInitialization");
        }
         void TokenHandler(INetworkMessage netMsg)
        {
            string str = "锟斤拷666";
            var data = Utility.Encode.ConvertToByte(str);
            UdpNetMessage msg = UdpNetMessage.EncodeMessage(netMsg.Conv, GateOperationCode._Token, data);
            GameManager.NetworkManager.SendNetworkMessage(msg);
        }
        async Task TokenHandlerAsync(INetworkMessage netMsg)
        {
             await Task.Run(() => { 



             });
        }
    }
}