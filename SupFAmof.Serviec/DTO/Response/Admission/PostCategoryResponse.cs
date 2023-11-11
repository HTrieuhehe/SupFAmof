using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class PostCategoryResponse
    {
        public int? Id { get; set; }
        public string? PostCategoryDescription { get; set; }
        public string? PostCategoryType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        //public virtual ICollection<Contract> Contracts { get; set; }
        //public virtual ICollection<Post> Posts { get; set; }
    }
}
