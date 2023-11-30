using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionAttendanceResponse
    {
        public int Id { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public virtual AttendanceAccountResponse? Account { get; set; }
        public virtual AttendancePostRegistrationResponse? PostRegistration { get; set; }

    }

    public class AttendanceAccountResponse
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ImgUrl { get; set; }
    }

    public class AttendancePostRegistrationResponse
    {
        public int? Id { get; set; }
        public string? RegistrationCode { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? PositionId { get; set; }
        public string? Note { get; set; }
        public double? Salary { get; set; }

        public virtual CollabPostResponse? Post { get; set; }
        public virtual PostPositionResponse? Position { get; set; }
    }
}
