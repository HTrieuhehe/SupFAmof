using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreatePostTitleRequest
    {
        //public int Id { get; set; }
        public string? PostTitleDescription { get; set; }
        public string? PostTitleType { get; set; }
        //public bool IsActive { get; set; }
        //public DateTime CreatAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }

    public class UpdatePostTitleRequest
    {
        //public int Id { get; set; }
        public string? PostTitleDescription { get; set; }
        public string? PostTitleType { get; set; }
        public bool IsActive { get; set; }
        //public DateTime CreatAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }
}
