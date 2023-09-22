
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class CreateAccountRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
        public bool PostPermission { get; set; }
        public bool? IsPremium { get; set; }
        public bool IsActive { get; set; }

    }

    public class CreateAccountReactivationRequest
    {
        public int AccountId { get; set; }
        public string? Email { get; set; }
    }
}
