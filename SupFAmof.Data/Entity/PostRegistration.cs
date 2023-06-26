using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Test
{
    public partial class PostRegistration
    {
        public int Id { get; set; }
        public string? RegistationCode { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
