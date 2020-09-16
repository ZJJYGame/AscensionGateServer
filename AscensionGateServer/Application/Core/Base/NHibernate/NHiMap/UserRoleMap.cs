using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionGateServer
{
    public class UserRoleMap:ClassMap<UserRole>
    {
        public UserRoleMap()
        {
            Id(x => x.UUID).GeneratedBy.Assigned().Column("uuid");
            Map(x => x.RoleIDArray).Column("role_id_array");
            Table("user_role");
        }
    }
}
