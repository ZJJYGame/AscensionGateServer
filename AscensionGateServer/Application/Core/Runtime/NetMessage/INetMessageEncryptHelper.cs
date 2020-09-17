using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    public interface INetMessageEncryptHelper
    {
        /// <summary>
        /// 将packet序列化后加密
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="messageBuffer">缓冲数据</param>
        /// <returns>是否序列化成功</returns>
        bool Serialize(MessagePacket packet, out byte[] messageBuffer);
        /// <summary>
        /// 将buffer反序列化成MessagePacket 对象
        /// </summary>
        /// <param name="messageBuffer">缓冲数据</param>
        /// <param name="packet">返回的数据包</param>
        /// <returns>是否反序列化成功</returns>
        bool Deserialize(byte[] messageBuffer, out MessagePacket packet);
    }
}
