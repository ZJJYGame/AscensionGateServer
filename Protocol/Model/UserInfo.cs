using System;
namespace Protocol
{
    [Serializable]
    public class UserInfo : IDisposable
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string IPAddress { get; set; }
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
