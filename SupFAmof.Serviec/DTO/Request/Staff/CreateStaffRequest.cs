﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Staff
{
    public class CreateStaffRequest
    {
        public string Name { get; set; } = null!;

        [MinLength(6,
        ErrorMessage = "Value for {0} must longer than {1} characters.")]
        [MaxLength(30,
        ErrorMessage = "Value for {0} must shorter than {1} characters.")]
        [Required]
        public string Username { get; set; } = null!;

        [MinLength(6,
        ErrorMessage = "Value for {0} must longer than {1} characters.")]
        [MaxLength(10,
        ErrorMessage = "Value for {0} must shorter than {1} characters.")]
        [Required]
        public String Password { get; set; } = null!;
    }
}
