using MessagePack;
using System;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class UserInfo : IDisposable
    {
        [Key(0)]
        public string Account { get; set; }
        [Key(1)]
        public string Password { get; set; }
        [Key(2)]
        public string IPAddress { get; set; }
        [Key(3)]
        public string DeviceUID { get; set; }
        public void Dispose()
        {
            Account = null;
            Password = null;
            IPAddress = null;
            DeviceUID = null;
        }
        public override string ToString()
        {
            string str = $"Account:{Account};Password:{Password};IPAddress:{IPAddress};DeviceUID:{DeviceUID};";
            return str;
        }
    }
}
