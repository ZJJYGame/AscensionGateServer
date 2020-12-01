using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionGateServer
{
    public abstract class MessagePacketHandler:IBehaviour,IReference
    {
        public abstract ushort OpCode { get; protected set; }
        public virtual void Clear(){}
        /// <summary>
        /// 处理消息；
        /// 此方法在外部执行时为异步；
        /// </summary>
        /// <param name="conv">传入的会话Id</param>
        /// <param name="packet">消息体</param>
        public virtual async Task HandleAsync (long conv,MessagePacket packet){}
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
