using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admin
{
    public class AdminSystemManagementResponse
    {
        public int? Id { get; set; }
        public string? RoleName { get; set; }
        public string? RoleEmail { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
