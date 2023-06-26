using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PersonalId { get; set; }
        public string? Address { get; set; }
        public DateTime? PersonalIdDate { get; set; }
        public string? PersonalIdDestination { get; set; }
        public byte[]? PersonalIdImgFront { get; set; }
        public byte[]? PersonalIdImgBack { get; set; }
        public string? TaxNumber { get; set; }
        public string? IdStudent { get; set; }
        public string? FbUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImgUrl { get; set; }
        public bool? IsPremium { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
