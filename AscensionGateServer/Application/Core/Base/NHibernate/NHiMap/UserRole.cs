using System;
using System.Collections.Generic;
using Cosmos;
using MessagePack;

namespace AscensionGateServer
{
    [Serializable]
    [MessagePackObject]
    public class UserRole
    {
        [Key(0)]
        public virtual string UUID { get; set; }
        [Key(1)]
        public virtual string RoleIDArray { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is UserRole))
                return false;
            var tmpRole = obj as UserRole;
            if (this.RoleIDArray.Equals(tmpRole.RoleIDArray) && this.UUID.Equals(tmpRole.UUID))
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return $"UUID:{UUID};RoleIDArray:{RoleIDArray}";
        }
    }
}
