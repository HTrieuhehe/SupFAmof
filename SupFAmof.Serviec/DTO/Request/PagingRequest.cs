using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.SortType;

namespace SupFAmof.Service.DTO.Request
{
    public class PagingRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Sort { get; set; }
        public string? Order { get; set; }
        //public string KeySearch { get; set; } = "";
        //public string SearchBy { get; set; } = "";
        //public SortOrder SortType { get; set; } = SortOrder.Descending;
        //public string ColName { get; set; } = "Id";
    }
}
