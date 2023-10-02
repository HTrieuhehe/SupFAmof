using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreatePostCategoryRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "PostTitleDescription cannot exceed 50 characters.")]
        public string? PostTitleDescription { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "PostTitleType cannot exceed 10 characters.")]
        public string? PostTitleType { get; set; }
    }

    public class UpdatePostCategoryRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "PostTitleDescription cannot exceed 50 characters.")]
        public string? PostTitleDescription { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "PostTitleType cannot exceed 10 characters.")]
        public string? PostTitleType { get; set; }

        [Range(typeof(bool), "true", "false", ErrorMessage = "IsActive must be either true or false.")]
        public bool IsActive { get; set; }
    }
}
