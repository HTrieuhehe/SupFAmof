﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.DTO.Request
{
    public class CreateNotificationHistoryRequest
    {
        //public int Id { get; set; }

        [Required(ErrorMessage = "Recipient Id is required")]
        [Range(0, int.MaxValue, ErrorMessage = "RecipientId must be a non-negative number")]
        public int RecipientId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Text is required")]
        [MaxLength(500, ErrorMessage = "Text cannot exceed 500 characters.")]
        public string? Text { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(0, int.MaxValue, ErrorMessage = "RecipientId must be a non-negative number")]
        public int Status { get; set; }

        [JsonIgnore]
        public DateTime CreateAt { get; set; } = GetCurrentDatetime();
    }
}
