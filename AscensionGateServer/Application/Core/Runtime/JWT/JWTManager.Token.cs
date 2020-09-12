using Cosmos;
using System.Threading.Tasks;
using Protocol;
using Ubiety.Dns.Core.Common;

namespace AscensionGateServer
{
    public sealed partial class JWTManager : Module<JWTManager>
    {
        public override void OnInitialization()
        {
            MessagePacketEventCore.Instance.AddEventListener((byte)GateOperationCode._Token, MessagePackHandler);
            Utility.Debug.LogInfo("JWT OnInitialization");
        }
        /// <summary>
        /// 处理数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        void MessagePackHandler(MessagePacket packet)
        {
            var data= packet[(byte)GateOperationCode._Token];
        }
        async Task<UdpNetMessage> SendMessageAsync(INetworkMessage netMsg, MessagePacket mp)
        {
            return await Task.Run<UdpNetMessage>(() =>
            {
                var data = MessagePacket.Serialize(mp);
                UdpNetMessage msg = UdpNetMessage.EncodeMessageAsync(netMsg.Conv, netMsg.OperationCode, data).Result;
                return msg;
            });
        }
    }
}