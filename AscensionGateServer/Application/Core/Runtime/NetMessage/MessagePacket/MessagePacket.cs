using Cosmos;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AscensionGateServer
{
    [MessagePackObject]
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
        [Key(0)]
        public byte OperationCode { get; set; }
        [Key(1)]
        public Dictionary<byte, object> Messages { get; set; }
        [Key(2)]
        public short ReturnCode { get; set; }
        object dataContract;
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
    }
}
