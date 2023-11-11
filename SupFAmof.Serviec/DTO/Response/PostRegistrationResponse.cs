using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.DTO.Response
{
    public class PostRegistrationResponse
    {
        public int Id { get; set; }
        public string RegistrationCode { get; set; } = null!;
        public int Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
        public string? Note { get; set; }
        public double Salary { get; set; }

        public virtual PostPositionResponse? Position { get; set; }  
        public virtual CollaboratorAccountReponse? Account { get; set; }
    }
    public class PostRgupdateHistoryResponse
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int PostRegistrationId { get; set; }
        public int? PositionId { get; set; }
        public bool? BusOption { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

    }

    public class CollaboratorAccountReponse
    {
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ImgUrl { get; set; }
        public bool? IsPremium { get; set; }
    }



    public class AdmissionPostsResponse
    {
        public int? Id { get; set; }

        public string? PostCategoryName { get; set; }
        public string? PostCode { get; set; }
        public string? PostDescription { get; set; }
        public int? Priority { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? PostImg { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }

        public virtual ICollection<AdmissionPostPositionResponse>? Positions { get; set; }
    }

    public class CollabRegistrationFormResponse
    {
        public int? Id { get; set; }
        public string? RegistrationCode { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }
        public int? AccountId { get; set; }
        public string? AccountEmail { get; set; }
        public string?  AccountName  { get; set; }

    }
    public class AdmissionPostPositionResponse
    {
        public int? Id { get; set; }
        public string? PositionName { get; set; }
        public virtual ICollection<CollabRegistrationFormResponse>? CollabRequest { get; set; }
    }

    public class CollabRegistrationResponse
    {
        public string? RegistrationCode { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }
        public int? PositionId { get; set; }
        public string? Note { get; set; }
        public virtual CollabPostResponse? Post { get; set; }
        public virtual PostPositionResponse? PostPosition { get; set; }



    }
    public class CollabPostResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostCode { get; set; }
        public string? PostImg { get; set; }
        public string? PostDescription { get; set; }
        public int? Priority { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public bool? IsPremium { get; set; }
        public int? Status { get; set; }
        public bool? AttendanceComplete { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; } 
        public virtual AccountResponse? Account { get; set; }
        public virtual PostCategoryResponse? PostCategory { get; set; }
    }


}
