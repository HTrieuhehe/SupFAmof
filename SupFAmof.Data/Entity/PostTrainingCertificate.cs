﻿using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class PostTrainingCertificate
    {
        public int Id { get; set; }
        public int PostTitleId { get; set; }
        public int TrainingCertiId { get; set; }

        public virtual PostCategory PostTitle { get; set; } = null!;
        public virtual TrainingCertificate TrainingCerti { get; set; } = null!;
    }
}
