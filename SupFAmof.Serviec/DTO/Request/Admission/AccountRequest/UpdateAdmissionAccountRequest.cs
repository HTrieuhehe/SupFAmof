﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission.AccountRequest
{
    public class UpdateAdmissionAccountRequest
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
    }
}