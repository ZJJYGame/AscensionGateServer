using System.Collections.Generic;
using System;
namespace AscensionGateServer
{
    [Serializable]
    public class User
    {
        public virtual string Account { get; set; }
        public virtual string Password { get; set; }
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
