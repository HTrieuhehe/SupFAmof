using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SupFAmof.Service.Helpers
{
    public class Enum
    {
        public enum SystemRoleEnum
        {
            [Display(Name = "Những con báo kỳ 9")]
            SystemAdmin = 0,
            [Display(Name = "Cán bộ tuyển sinh")]
            AdmissionManager = 1,
        }
    }
}
