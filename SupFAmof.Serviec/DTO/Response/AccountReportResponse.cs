using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountReportResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PositionId { get; set; }
        public double? Salary { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual AccountResponse? Account { get; set; }
        public virtual AccountReportPostPositionResponse? Position { get; set; }
    }

    public class AccountReportFilter
    {
        public DateTime? CreateAtStart { get; set; }
        public DateTime? CreateAtEnd { get; set; }
    }

    public class AccountReportPostPositionResponse
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string PositionName { get; set; } = null!;
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool? IsBusService { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public double Salary { get; set; }
    }

    public class ReportPostRegistrationResponse
    {
        public int? Id { get; set; }
        public string? RegistrationCode { get; set; }
        public int PositionId { get; set; }
        public string? Note { get; set; }
        public double Salary { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }

        public virtual CollabReportPostResponse? Post { get; set; }
        public virtual ReportPostPositionResponse? PostPosition { get; set; }
        public virtual ICollection<CheckAttendanceResponse>? CheckAttendances { get; set; }
    }

    public class CollabReportPostResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostCode { get; set; }
        public string? PostImg { get; set; }
        public string? PostDescription { get; set; }
        public int? Priority { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsPremium { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual AccountResponse? Account { get; set; }

    }

    public class ReportPostResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostCode { get; set; }
        public string? PostImg { get; set; }
        public string? PostDescription { get; set; }
        public int? Priority { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsPremium { get; set; }
        public int? Status { get; set; }
        public bool? AttendanceComplete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public ICollection<ReportPostPositionResponse>? PostPositions { get; set; }
    }

    public class ReportPostPositionResponse
    {
        public int? Id { get; set; }
        public int? PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string? PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Status { get; set; }
        public bool? IsBusService { get; set; }
        public int? Amount { get; set; }
        public double? Salary { get; set; }
    }

    /*
     
     {
  "status": {
    "success": true,
    "message": "Success",
    "errorCode": 0
  },
  "data": {
    "id": 96,
    "registrationCode": "9330561378",
    "positionId": 100,
    "note": null,
    "salary": 200000,
    "status": 6,
    "schoolBusOption": false,
    "createAt": "12/11/2023 12:57:09 AM",
    "updateAt": null,
    "post": {
      "id": 48,
      "accountId": 10,
      "postCategoryId": 1,
      "postCode": "5630411447",
      "postImg": "https://firebasestorage.googleapis.com/v0/b/supfamof-c8c84.appspot.com/o/images%2Fadmission%2Feventeef3e1b9-18b4-46e9-81dc-8527f0fa13df?alt=media&token=f2a4fe66-355a-43ee-9a95-1f7ccbe43405",
      "postDescription": "<p>test report anh duy</p>",
      "priority": 1,
      "dateFrom": "2023-12-11T00:00:00",
      "dateTo": "2023-12-11T00:00:00",
      "isPremium": false,
      "status": 3,
      "createAt": "2023-12-10T00:48:25.217",
      "updateAt": "2023-12-11T21:30:00.767"
    },
    "position": {
      "id": 100,
      "postId": 48,
      "trainingCertificateId": null,
      "documentId": null,
      "positionName": "test report ",
      "positionDescription": "test report ",
      "schoolName": "test report ",
      "location": "Trường Đại học Ngân hàng TP.HCM, 56, Hoang Dieu 2, Linh Chieu Ward, Thu Duc City, 00848, Vietnam",
      "latitude": "10.857199",
      "longtitude": null,
      "timeFrom": "01:00:00",
      "timeTo": "01:01:00",
      "status": 1,
      "isBusService": false,
      "amount": 1,
      "salary": 200000
    },
    "checkAttendances": [
      {
        "id": 9,
        "postId": null,
        "positionId": null,
        "checkInTime": "2023-12-11T00:59:46.733",
        "checkOutTime": "2023-12-11T01:01:06.86",
        "post": null
      }
    ]
  }
}
     
     */
}
