using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class CollabInfoReportResponse
    {
        public string? Name { get; set; }
        public string? IdStudent { get; set; }
        public string? IdentityNumber { get; set; }
        public string? TaxNumber { get; set; }

        // Final Amaount sum by all salary in Working Content
        public string? FinalAmount { get; set; }
    
        public ICollection<WorkingContentResponse>? WorkingContents { get; set; }
    }

    public class WorkingContentResponse
    {
        public string? PostCategoryDescription { get; set; }
        public string? DateFrom { get; set; }

        // salary sum by all positions are registed in one post
        public string? Salary { get; set; }

        //total amount must pay for each Post
        public string? TotalAmount { get; set; }
    }
}
