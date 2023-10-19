using System.Text.Json.Serialization;
using Service.Commons;
using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreatePostRequest
    {
        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "PostCategoryId must contain only numeric characters.")]
        public int PostCategoryId { get; set; }

        [Required]
        public string? PostDescription { get; set; }


        public string? PostImg { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Priority must be greater or equal 0")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "PostCategoryId must contain only numeric characters.")]
        public int Priority { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsPremium { get; set; }

        public List<CreatePostPositionRequest>? PostPositions { get; set; }
    }

    public class CreatePostPositionRequest
    {

        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }

        [Required(ErrorMessage = "Position Name is required")]
        [MaxLength(30, ErrorMessage = "Position Name cannot exceed 30 characters.")]
        public string? PositionName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Position Description cannot exceed 100 characters.")]
        public string? PositionDescription { get; set;}

        [Required]
        [MaxLength(100, ErrorMessage = "School Name cannot exceed 100 characters.")]
        public string? SchoolName { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Location cannot exceed 500 characters.")]
        public string? Location { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Latitude must be a decimal number.")]
        public string? Latitude { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Longitude must be a decimal number.")]
        public string? Longitude { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool? IsBusService { get; set; }

        [JsonIgnore]
        public int Status { get; set; }

        [Range(1, 1000, ErrorMessage = "Amount must be between 1 and 1000")]
        public int Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be a negative number")]
        public double Salary { get; set; }
    }


    public class UpdatePostRequest
    {
        [Required]
        public int PostCategoryId { get; set; }
        [Required]
        public string? PostDescription { get; set; }
        [Required]
        public string? PostImg { get; set; }

        public List<UpdatePostPositionRequest>? PostPositions { get; set; }
    }

    public class UpdatePostPositionRequest
    {
        [Required]
        [Int]
        public int Id { get; set; }

        [Required(ErrorMessage = "Position Name is required")]
        [MaxLength(20, ErrorMessage = "Position Name cannot exceed 20 characters.")]
        public string? PositionName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Position Description cannot exceed 100 characters.")]
        public string? PositionDescription { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "School Name cannot exceed 100 characters.")]
        public string? SchoolName { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Location cannot exceed 500 characters.")]
        public string? Location { get; set; }

        [Required]
        public decimal? Latitude { get; set; }

        [Required]
        public decimal? Longtitude { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be a negative number")]
        public double Salary { get; set; }
    }
}
