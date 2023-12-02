using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class CreateAccountInformationRequest
    {
        public string? IdentityFrontImg { get; set; }
    }
}
