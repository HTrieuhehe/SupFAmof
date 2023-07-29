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
            [Display(Name = "Cán bộ tuyển sinh")]
            AdmissionManager = 1,

            [Display(Name = "Sinh viên")]
            Student = 2,

            [Display(Name = "Những con báo kỳ 9")]
            SystemAdmin = 3,
        }

        public enum StaffErrorEnum
        {
            //404
            [Display(Name = "Staff ID not found!")]
            NOT_FOUND_ID = 4041,

            //400
            [Display(Name = "This staff already exsist!")]
            STAFF_EXSIST = 4001,

            [Display(Name = "Username or password is not correct")]
            LOGIN_FAIL = 4002,
        }
    }
}
