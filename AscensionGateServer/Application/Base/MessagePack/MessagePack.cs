using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionGateServer
{
    /// <summary>
    ///序列化顺序：OpCode-> Parameters->ReturnCode
    /// </summary>
    public class MessagePacket
    {
        public object this[byte parameterKey]
        {
            get
            {
                object varlue;
                Parameters.TryGetValue(parameterKey, out varlue);
                return varlue;
            }
            set
            {
                Parameters.TryAdd(parameterKey, value);
            }
        }
        public string DebugMessage { get; set; }
        public byte OperationCode { get; set; }
        public Dictionary<byte, object> Parameters { get; set; }
        public short ReturnCode { get; set; }
        object dataContract;
        static IMessagePackSerializeProvider serializationProvider;
        public MessagePacket() { }
        public MessagePacket(byte operationCode)
        {
            this.OperationCode = operationCode;
        }
        public MessagePacket(byte operationCode, object dataContract) : this(operationCode)
        {
            this.dataContract = dataContract;
        }
        public MessagePacket(byte operationCode, Dictionary<byte, object> parameters) : this(operationCode)
        {
            this.Parameters = parameters;
        }
        public void SetParameters(object dataContract)
        {
            this.dataContract = dataContract;
        }
        public void SetParameters(Dictionary<byte, object> parameters)
        {
            this.Parameters = parameters;
        }
        public static void SetProvider(IMessagePackSerializeProvider provider)
        {
            serializationProvider = provider;
        }
        public static byte[] Serialize(MessagePacket msgPack)
        {
            return serializationProvider.Serialize(msgPack);
        }
        public static MessagePacket Deserialize(byte[] data)
        {
            return serializationProvider.Deserialize(data);
        }
    }
}
