using System;
namespace Protocol
{
    [Serializable]
    public class User : IDisposable
    {
        public virtual string Account { get; set; }
        public virtual string Password { get; set; }
        public virtual string UUID { get; set; }
        public void Dispose()
        {
            Account = null;
            Password = null;
            UUID = null;
        }
        public override string ToString()
        {
            return $"Account : {Account} ; Password : {Password}; UUID: {UUID} ; ";
        }
    }
}
