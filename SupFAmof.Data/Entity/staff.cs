using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class staff
    {
        public staff()
        {
            AccessTokens = new HashSet<AccessToken>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public byte[] Password { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<AccessToken> AccessTokens { get; set; }
    }
}
