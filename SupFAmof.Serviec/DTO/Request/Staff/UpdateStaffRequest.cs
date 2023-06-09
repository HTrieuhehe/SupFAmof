using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Staff
{
    public class UpdateStaffRequest
    {
        public string Name { get; set; } = null!;
        public String Password { get; set; } = null!;
        public bool IsAvailable { get; set; }
    }
}
