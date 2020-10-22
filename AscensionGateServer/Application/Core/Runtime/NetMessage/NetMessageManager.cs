using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    [CustomeModule]
    public class NetMessageManager : Module<NetMessageManager>
    {
        INetMessageEncryptHelper netMsgEncryptHelper;
        ConcurrentDictionary<ushort, MessagePacketHandler> handlerDict = new ConcurrentDictionary<ushort, MessagePacketHandler>();
        public override void OnInitialization()
        {
            NetworkMsgEventCore.Instance.AddEventListener(GateOperationCode._MSG, HandleMessage);
            InitHandler();
            InitHelper();
        }
        void InitHelper()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute>(typeof(INetMessageEncryptHelper));
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
                        handlerDict.TryAdd(handler.OpCode, handler);
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
            HandleMessageAsync(netMsg);
        }
        async void HandleMessageAsync(INetworkMessage netMsg)
        {
            await Task.Run(() =>
            {
                try
                {
                    //这里是解码成明文后进行反序列化得到packet数据；
                    MessagePacket packet = Utility.MessagePack.ToObject<MessagePacket>(netMsg.ServiceMsg);
                    if (packet == null)
                        return;
                    var exist = handlerDict.TryGetValue(packet.OperationCode, out var handler);
                    if (exist)
                    {
                        var mp = handler.Handle(packet);
                        if (mp != null)
                        {
                            SendMessageAsync(netMsg, mp);
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            });
        }
        async void SendMessageAsync(INetworkMessage netMsg, MessagePacket packet)
        {
            await Task.Run(() =>
            {
                //加密为密文byte[]；
                byte[] packetBuffer=Utility.MessagePack.ToByteArray(packet);
                if (packetBuffer!=null)
                {
                    UdpNetMessage msg = UdpNetMessage.EncodeMessage(netMsg.Conv, netMsg.OperationCode, packetBuffer);
                    GameManager.NetworkManager.SendNetworkMessage(msg);
                }
            });

        }
    }
}
