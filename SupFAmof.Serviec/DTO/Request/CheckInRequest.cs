using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.Utilities;
using System.Text.Json.Serialization;
using static ServiceStack.LicenseUtils;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request
{
    public class CheckInRequest
    {
        [Required(ErrorMessage = "PostId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than 0.")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "PositionId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PositionId must be greater than 0.")]
        public int PositionId { get; set; }

        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
    }

    public class CheckOutRequest
    {
        [Required(ErrorMessage = "PostId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than 0.")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "PositionId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PositionId must be greater than 0.")]
        public int PositionId { get; set; }
    }
}
