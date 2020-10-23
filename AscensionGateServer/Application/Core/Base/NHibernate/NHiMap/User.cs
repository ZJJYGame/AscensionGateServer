using System.Collections.Generic;
using System;
using MessagePack;

namespace AscensionGateServer
{
    [Serializable]
    [MessagePackObject]
    public class User
    {
        [Key(0)]
        public virtual string Account { get; set; }
        [Key(1)]
        public virtual string Password { get; set; }
        [Key(2)]
        public virtual string UUID { get; set; }
        public override bool Equals(object obj)
        {
            User user = obj as User;
            return user.Account == this.Account && user.Password == this.Password ;
        }
        public override string ToString()
        {
            return $"Account : {Account};Password : {Password};UUID: {UUID}";
        }
    }
}
