using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
namespace AscensionGateServer
{
    //==========================================//
    //消息处理流程：
    //1、消息从4319口进入此管理类，由MessageHandler进行解码；
    //2、消息解码为MessagePacket对象后，根据opCode派发到具体
    // 消息处理者。处理者本质为异步；
    //3、消息处理者完成消息处理后，则返回处理完成的消息；
    //4、发送处理好的消息；
    //==========================================
    [OuterModule]
    public class NetMessageManager : Module<NetMessageManager>
    {
        INetMessageEncryptHelper netMsgEncryptHelper;
        Dictionary<ushort, MessagePacketHandler> handlerDict = new Dictionary<ushort, MessagePacketHandler>();
        public override void OnInitialization()
        {
            MessagePacket.SetHelper(new MessagePacketJsonHelper());
            NetworkMsgEventCore.Instance.AddEventListener(GateOperationCode._MSG, HandleMessage);
            InitHandler();
            InitHelper();
        }
        void InitHelper()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<TargetHelperAttribute>(typeof(INetMessageEncryptHelper));
            netMsgEncryptHelper = obj as INetMessageEncryptHelper;
        }
        /// <summary>
        /// 初始化消息处理者；
        /// </summary>
        void InitHandler()
        {
            var handlerType = typeof(MessagePacketHandler);
            Type[] types = Assembly.GetAssembly(handlerType).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (handlerType.IsAssignableFrom(types[i]))
                {
                    if (types[i].IsClass && !types[i].IsAbstract)
                    {
                        var handler = Utility.Assembly.GetTypeInstance(types[i]) as MessagePacketHandler;
                        handler.OnInitialization();
                        handlerDict.Add(handler.OpCode, handler);
                    }
                }
            }
        }
        /// <summary>
        ///  处理从系统通讯通道 （_MSG ）接收到的消息，并解包成MessagePack对象；
        ///  解包完成后，派发消息到具体的消息处理者；
        ///  处理者完成处理后，对消息进行发送；
        /// </summary>
        /// <param name="netMsg">数据</param>
        void HandleMessage(INetworkMessage netMsg)
        {
            //MessagePacket packet;
            ////这里是解码成明文后进行反序列化得到packet数据；
            //var result = netMsgEncryptHelper.Deserialize(netMsg.ServiceMsg, out packet);
            //if (!result)
            //    return;
            //MessagePacketHandler handler;
            //var exist = handlerDict.TryGetValue(packet.OperationCode, out handler);
            //if (exist)
            //{
            //    var mp = handler.Handle(packet);
            //    if (mp != null)
            //        SendMessage(netMsg, mp);
            //}
            var t= HandleMessageAsync(netMsg);
        }
        async Task HandleMessageAsync(INetworkMessage netMsg)
        {
            await Task.Run(() => 
            {
                MessagePacket packet;
                //这里是解码成明文后进行反序列化得到packet数据；
                var result = netMsgEncryptHelper.Deserialize(netMsg.ServiceMsg, out packet);
                if (!result)
                    return;
                MessagePacketHandler handler;
                var exist = handlerDict.TryGetValue(packet.OperationCode, out handler);
                if (exist)
                {
                    var mp = handler.Handle(packet);
                    if (mp != null)
                        SendMessage(netMsg, mp);
                }
            });
        }
        void SendMessage(INetworkMessage netMsg, MessagePacket packet)
        {
            byte[] packetBuffer;
            //加密为密文byte[]；
            var result = netMsgEncryptHelper.Serialize(packet, out packetBuffer);
            if (result)
            {
                UdpNetMessage msg = UdpNetMessage.EncodeMessageAsync(netMsg.Conv, netMsg.OperationCode, packetBuffer).Result;
                GameManager.NetworkManager.SendNetworkMessage(msg);
            }
        }
    }
}
