using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class PostCategoryResponse
    {
        public int Id { get; set; }
        public string? PostTitleDescription { get; set; }
        public string? PostTitleType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        //public virtual ICollection<Contract> Contracts { get; set; }
        //public virtual ICollection<Post> Posts { get; set; }
    }
}
