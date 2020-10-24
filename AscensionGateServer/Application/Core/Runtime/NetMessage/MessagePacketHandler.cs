using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionGateServer
{
    public abstract class MessagePacketHandler:IBehaviour
    {
        public abstract ushort OpCode { get; protected set; }
        /// <summary>
        /// 处理消息；
        /// 此方法在外部执行时为异步；
        /// </summary>
        /// <param name="packet">消息体</param>
        /// <returns></returns>
        public  abstract  Task<MessagePacket >HandleAsync (MessagePacket packet);
        /// <summary>
        /// 空虚函数；
        /// </summary>
        public virtual void OnInitialization() { }
        /// <summary>
        /// 空虚函数；
        /// </summary>
        public virtual void OnTermination(){}
    }
}
