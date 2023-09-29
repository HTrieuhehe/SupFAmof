using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.Utilities;
using System.Text.Json.Serialization;

namespace SupFAmof.Service.DTO.Request
{
    public class CheckInRequest
    {
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        [JsonIgnore]
        public DateTime CheckInTime { get; set; } = Ultils.GetCurrentTime() ;
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
    }

    public class CheckOutRequest
    {
        //public int AccountId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        //public DateTime CheckOutTime { get; set; }
    }
}
