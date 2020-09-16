using System;
using System.Collections.Generic;
using Cosmos;
namespace AscensionGateServer
{
    [Serializable]
    public class UserRole
    {
        public virtual string UUID { get; set; }
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
