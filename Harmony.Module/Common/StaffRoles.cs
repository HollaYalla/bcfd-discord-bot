using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony.Module.Attributes;

namespace Harmony.Module.Common
{
    internal class StaffRoles
    {
        public enum Role
        {
            [StringValue("Tow Driver")]
            TOW_DRIVER,
            [StringValue("Intern Mechanic")]
            INTERN_MECHANIC,
            [StringValue("Mechanic")]
            MECHANIC,
            [StringValue("Lead Mechanic")]
            LEAD_MECHANIC,
            [StringValue("Expert Mechanic")]
            EXPERT_MECHANIC,
            [StringValue("Veteran Mechanic")]
            VETERAN_MECHIANIC,
            [StringValue("Manager")]
            MANAGER,
            [StringValue("Veteran Manager")]
            VETERAN_MANAGER,
            [StringValue("Boss")]
            BOSS,
            [StringValue("IT Support")]
            IT_SUPPORT
        }
    }
}
