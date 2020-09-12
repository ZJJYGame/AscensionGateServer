using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
namespace AscensionGateServer
{
    //==========================================//
    //消息处理流程：
    //1、接收到 INetworkMessage 消息后，获取ServiceMsg属性，将
    // buffer转换对称解密为明文字段；
    //
    //2、反序列化明文字段到具体对象，查询数据库的对象映射表数据。
    //
    //3、根据查询结果，生成MessagePacket（以下简称mp）。mp的
    //数据实体为字典，根据key-value进行添加。为mp添加数据载荷则
    //调用mp的SetMessages方法即可。另外需要为mp添加返回码--
    //ReturnCode；返回码的载荷为成功、失败、异常等。
    //
    //4、发送数据到客户端。
    //  注：
    //查询结果为真：根据ServiceMsg转换的文明生成token，并
    //为mp添加载荷；
    //查询结果为否：不生成token，mp空数据载荷，但持有返回值载荷。
    //==========================================

    public class NetMessageManager : Module<NetMessageManager>
    {
        public override void OnInitialization()
        {
            MessagePacket.SetHelper(new MessagePacketJsonHelper());
            NetworkMsgEventCore.Instance.AddEventListener(GateOperationCode._MSG, Handler);
        }
        /// <summary>
        ///  处理从系统通讯通道 （_MSG ）接收到的消息，并解包成MessagePack对象；
        ///  解包完成后，通过MessagePackEventCore广播监听MessagePack.OperationCode的方法；
        /// </summary>
        /// <param name="netMsg">数据</param>
        void Handler(INetworkMessage netMsg)
        {
            MessagePacket packet = MessagePacket.Deserialize(netMsg.ServiceMsg);
            MessagePacketEventCore.Instance.Dispatch(packet.OperationCode, packet);
        }
    }
}
