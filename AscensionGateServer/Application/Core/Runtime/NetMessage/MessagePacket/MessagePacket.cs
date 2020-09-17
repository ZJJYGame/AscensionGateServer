using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    /// <summary>
    ///序列化顺序：OpCode-> Parameters->ReturnCode
    /// </summary>
    public class MessagePacket
    {
        public object this[byte messageKey]
        {
            get
            {
                object varlue;
                Messages.TryGetValue(messageKey, out varlue);
                return varlue;
            }
            set
            {
                Messages.TryAdd(messageKey, value);
            }
        }
        public string DebugMessage { get; set; }
        public byte OperationCode { get; set; }
        public Dictionary<byte, object> Messages { get; set; }
        public short ReturnCode { get; set; }
        object dataContract;
        static IMessagePacketSerializeHelper serializationHelper;
        public MessagePacket() { }
        public MessagePacket(byte operationCode)
        {
            this.OperationCode = operationCode;
        }
        public MessagePacket(byte operationCode, object dataContract) : this(operationCode)
        {
            this.dataContract = dataContract;
        }
        public MessagePacket(byte operationCode, Dictionary<byte, object> messages) : this(operationCode)
        {
            this.Messages = messages;
        }
        public void SetMessages(object dataContract)
        {
            this.dataContract = dataContract;
        }
        public void SetMessages(Dictionary<byte, object> messages)
        {
            this.Messages = messages;
        }
        public static void SetHelper(IMessagePacketSerializeHelper helper)
        {
            serializationHelper = helper;
        }
        public static byte[] Serialize(MessagePacket msgPack)
        {
            return serializationHelper.Serialize(msgPack);
        }
        public static MessagePacket Deserialize(byte[] data)
        {
            return serializationHelper.Deserialize(data);
        }
        public async static Task<byte[]> SerializeAsync(MessagePacket msgPack)
        {
            return await Task.Run(() => { return serializationHelper.Serialize(msgPack); });
        }
        public async static Task<MessagePacket> DeserializeAsync(byte[] data)
        {
            return await Task.Run(() => { return serializationHelper.Deserialize(data); });
        }
    }
}
