﻿using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class TotalAccountResponse
    {
        public int TotalCollaborator { get; set; }
        public virtual List<NewCollaboratorResponse>? NewCollaborators { get; set; }
    }

    public class NewCollaboratorResponse
    {
        public string? ImgUrl { get; set; }
    }

    public class AccountResponse
    {
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public int? AccountInformationId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
        public bool? PostPermission { get; set; }
        public bool? IsPremium { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBanned { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public AccountInformationResponse AccountInformation { get; set; }
        //public virtual ICollection<AccountCertificateResponse> AccountCertificateAccounts { get; set; }
    }

    public class AccountInformationResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public string? IdentityNumber { get; set; }
        public string? IdStudent { get; set; }
        public string? FbUrl { get; set; }
        public string? Address { get; set; }
        public DateTime? IdentityIssueDate { get; set; }
        public string? PlaceOfIssue { get; set; }
        public string? IdentityFrontImg { get; set; }
        public string? IdentityBackImg { get; set; }
        public string? TaxNumber { get; set; }
    }

    public class AccountReactivationResponse
    {
        public DateTime ExpirationDate { get; set; }
    }
}
