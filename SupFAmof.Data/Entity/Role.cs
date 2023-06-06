using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
            staff = new HashSet<staff>();
        }

        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string RoleEmail { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<staff> staff { get; set; }
    }
}
