using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionAttendanceResponse
    {
        public int Id { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public virtual AttendanceAccountResponse? Account { get; set; }
    }

    public class AttendanceAccountResponse
    {

    }
}
