using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Role
{
    public class CreateRoleRequest
    {
        [Required]
        [MaxLength(20, ErrorMessage = "RoleName cannot exceed 20 characters.")]
        public string? RoleName { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "RoleEmail cannot exceed 20 characters.")]
        public string? RoleEmail { get; set; }
    }
}
