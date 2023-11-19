using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Admin
{
    public class CreateAdminSystemManagementRequest
    {
        [Required(ErrorMessage = "Role Name is requied")]
        [MaxLength(20, ErrorMessage = "Role Name cannot exceed 20 characters.")]
        public string? RoleName { get; set; }

        [Required(ErrorMessage = "Role Email is requied")]
        [MaxLength(500, ErrorMessage = "Role Email cannot exceed 20 characters.")]
        public string? RoleEmail { get; set; }
    }

    public class UpdateAdminSystemManagementRequest
    {
        [Required(ErrorMessage = "Role Name is requied")]
        [MaxLength(20, ErrorMessage = "Role Name cannot exceed 20 characters.")]
        public string? RoleName { get; set; }

        [Required(ErrorMessage = "Role Email is requied")]
        [MaxLength(500, ErrorMessage = "Role Email cannot exceed 20 characters.")]
        public string? RoleEmail { get; set; }
    }
}
