﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class CreateAccountInformationRequest
    {
        public string? PersonalId { get; set; }
        public string? IdStudent { get; set; }
        public string? FbUrl { get; set; }
        public string? Address { get; set; }
        public DateTime? PersonalIdDate { get; set; }
        public string? PlaceOfIssue { get; set; }
        public string? PersonalIdFrontImg { get; set; }
        public string? PersonalIdBackImg { get; set; }
        public string? TaxNumber { get; set; }
    }
}
