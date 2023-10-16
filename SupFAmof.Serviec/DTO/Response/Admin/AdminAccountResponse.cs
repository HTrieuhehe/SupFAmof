using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admin
{
    public class AdminAccountResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
