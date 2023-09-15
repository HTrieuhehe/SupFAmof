using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class PostPositionResponse
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? PositionName { get; set; }
        public int Amount { get; set; }
        public double Salary { get; set; }

        private int? registerAmount;
        public int? RegisterAmount { get => registerAmount; set => registerAmount = value; }

    }
}
