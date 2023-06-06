using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Role
{
    public class UpdateRoleRequest
    {
        public string? RoleName { get; set; }
        public string? RoleEmail { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
