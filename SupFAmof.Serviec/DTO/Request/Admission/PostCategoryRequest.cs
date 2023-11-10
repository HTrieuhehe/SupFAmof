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
        [MaxLength(50, ErrorMessage = "Post Category Description cannot exceed 50 characters.")]
        public string? PostCategoryDescription { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Post Category Type cannot exceed 10 characters.")]
        public string? PostCategoryType { get; set; }
    }

    public class UpdatePostCategoryRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Post Category Description cannot exceed 50 characters.")]
        public string? PostCategoryDescription { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Post Category Type cannot exceed 10 characters.")]
        public string? PostCategoryType { get; set; }
    }
}
