using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class UpdateAccountRequest
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }

        public UpdateAccountInformationRequest? UpdateAccountInformation { get; set; }
    }
}
