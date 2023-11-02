using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreateAdmissionContractRequest
    {
        //public int Id { get; set; }
        //public int CreatePersonId { get; set; }

        [Required (ErrorMessage = "Contract Name is required.")]
        [MaxLength(100, ErrorMessage = "Contract Name cannot exceed 100 characters.")]
        public string? ContractName { get; set; }

        [Required (ErrorMessage = "Contract Description is required.")]
        [MaxLength(225, ErrorMessage = "Contract Description cannot exceed 225 characters.")]
        public string? ContractDescription { get; set; }

        [Required(ErrorMessage = "Sample File cannot empty")]
        public string? SampleFile { get; set; }

        [Required (ErrorMessage = "Contract Name is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid signing date format.")]
        public DateTime SigningDate { get; set; }

        [Required (ErrorMessage = "Start Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid start date format.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid start date format.")]
        public DateTime EndDate { get; set; }

        [Required (ErrorMessage = "Total Salary is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Salary must be a non-negative number.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Invalid numeric format.")]
        public double TotalSalary { get; set; }

        
        //public DateTime CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }

    public class UpdateAdmissionContractRequest
    {
        [Required]
        [MaxLength(100, ErrorMessage = "ContractName cannot exceed 100 characters.")]
        public string? ContractName { get; set; }

        [Required]
        [MaxLength(225, ErrorMessage = "ContractDescription cannot exceed 225 characters.")]
        public string? ContractDescription { get; set; }

        [Required(ErrorMessage = "Sample File cannot empty")]
        public string? SampleFile { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid signing date format.")]
        public DateTime SigningDate { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid start date format.")]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "TotalSalary must be a non-negative number.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Invalid numeric format.")]
        public double TotalSalary { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "IsActive must be either true or false.")]
        public bool IsActive { get; set; }

        //chỉ cho sửa khi chưa gởi cho bất kì ai
    }
}
