﻿using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class Application
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int? AccountReplyId { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string ProblemNote { get; set; } = null!;
        public string? ReplyNote { get; set; }
        public int Status { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Account? AccountReply { get; set; }
    }
}
