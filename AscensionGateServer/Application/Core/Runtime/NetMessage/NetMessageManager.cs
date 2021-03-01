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
    [Module]
    public class NetMessageManager : Cosmos.Module, INetMessageManager
    {
        INetMessageEncryptHelper netMsgEncryptHelper;
        ConcurrentDictionary<ushort, Queue<MessagePacketHandler>> handlerDict;
        ConcurrentDictionary<ushort, Type> handlerTypeDict;
        public override void OnInitialization()
        {
            NetworkMsgEventCore.Instance.AddEventListener(GateOperationCode._MSG, ProcessCommandMessage);
            handlerDict = new ConcurrentDictionary<ushort, Queue<MessagePacketHandler>>();
            handlerTypeDict = new ConcurrentDictionary<ushort, Type>();
            InitHandler();
            InitHelper();
        }
        public async void SendMessageAsync(long conv, OperationData packet)
        {
            await Task.Run(() =>
            {
                try
                {
                    //加密为密文byte[]；
                    byte[] packetBuffer = Utility.MessagePack.ToByteArray(packet);
                    if (packetBuffer != null)
                    {
                        UdpNetMessage msg = UdpNetMessage.EncodeMessage(conv, GateOperationCode._MSG, packetBuffer);
                        CosmosEntry.NetworkManager.SendNetworkMessage(msg);
                    }
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            });
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
            var handlers = Utility.Assembly.GetDerivedTypeInstances<MessagePacketHandler>();
            var length = handlers.Length;
            for (int i = 0; i < length; i++)
            {
                handlers[i].OnInitialization();
                handlerTypeDict.TryAdd(handlers[i].OpCode, handlers[i].GetType());
            }
        }
        /// <summary>
        ///  处理从系统通讯通道 （_MSG ）接收到的消息，并解包成OperationData对象；
        /// 处理指令消息；
        /// </summary>
        /// <param name="netMsg">数据</param>
        async void ProcessCommandMessage(INetworkMessage netMsg)
        {
            try
            {
                //这里是解码成明文后进行反序列化得到OperationData数据；
                var packet = Utility.MessagePack.ToObject<OperationData>(netMsg.ServiceMsg);
                if (packet == null)
                    return;
                CommandEventCore.Instance.Dispatch((byte)packet.OperationCode, netMsg.Conv, packet);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
    }
}

