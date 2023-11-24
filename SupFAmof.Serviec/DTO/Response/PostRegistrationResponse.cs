using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.DTO.Response
{
    public class PostRegistrationResponse
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
        public virtual CollaboratorAccountReponse? Account { get; set; }
    }
    public class PostRgupdateHistoryResponse
    {
        public int? Id { get; set; }
        public int? PostRegistrationId { get; set; }
        public int? PositionId { get; set; }
        public int? OriginalId { get; set; }
        public bool? BusOption { get; set; }
        public int? Status { get; set; }
        public string? CreateAt { get; set; }

        public virtual PostRGUpdatePostRegistrationResponse? PostRegistration { get; set; }
        public virtual PostRGUpdatePostResponse? Post { get; set; }
        public virtual PostRGUpdatePositionResponse? PostPositionNeedToBeUpdated { get; set; }
        public virtual PostRGUpdateOriginalPositionResponse? PostPositionOriginal { get; set; }

    }

    public class PostRGUpdateOriginalPositionResponse
    {
        public string? PositionName { get; set; }
    }

    public class PostRGUpdatePostRegistrationResponse
    {
        public int Id { get; set; }
        public string RegistrationCode { get; set; } = null!;
        public int AccountId { get; set; }
        public int PositionId { get; set; }
        public string? Note { get; set; }
        public double Salary { get; set; }
        public int Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual PostPositionResponse? Position { get; set; }
    }

    public class PostRGUpdatePositionResponse
    {
        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string? PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public string? Date { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? TimeFrom { get; set; }
        public string? TimeTo { get; set; }
        public int? Status { get; set; }
        public bool? IsBusService { get; set; }
        public int? Amount { get; set; }
        public double? Salary { get; set; }
    }

    public class PostRGUpdatePostResponse
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
        public int? Id { get; set; }
        public string? RegistrationCode { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }
        public int? PositionId { get; set; }
        public string? Note { get; set; }
        public DateTime? PositionDate { get; set; }
        public int? PostCategoryId { get; set; }

        public virtual CollabPostResponse? Post { get; set; }
        public virtual PostPositionResponse? PostPosition { get; set; }
    }

    public class CollabRegistrationUpdateViewResponse
    {
        public int? Id { get; set; }
        public string? RegistrationCode { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }
        public int? PositionId { get; set; }
        public string? Note { get; set; }
        public DateTime? PositionDate { get; set; }
        public int? PostCategoryId { get; set; }
        public virtual ICollection<PostPositionResponse>? PostPositionsUnregistereds { get; set; }

        public bool? IsUpdated { get; set; }
        public virtual CollabRegistrationUpdatePostResponse? Post { get; set; }
        public virtual PostPositionResponse? PostPosition { get; set; }
    }

    public class CollabRegistrationUpdatePostResponse
    {
        public CollabRegistrationUpdatePostResponse()
        {
            RegisterAmount = 0;
            TotalAmountPosition = 0;
        }

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

        public int? RegisterAmount;
        //public  int? RegisterAmount { get => registerAmount ?? 0; set => registerAmount = value; }

        public int? TotalAmountPosition;
        //public  int? TotalAmountPosition { get => totalAmountPosition ?? 0; set => totalAmountPosition = value; }
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

    public class FilterPostRegistrationResponse
    {
        public List<int>? RegistrationStatus { get; set; }
    }
    public class AdmissionUpdateRequestResponse
    {
        public int? Id { get; set; }
        public int? PostRegistrationId { get; set; }
        public int? PositionId { get; set; }
        public bool? BusOption { get; set; }
        public int? Status { get; set; }
        public DateTime CreateAt { get; set; }
        public virtual PostPositionResponse? PostPositionNeedToBeUpdated { get; set; }

        public virtual PostPositionResponse? PostPositionOriginal { get; set; }
        public virtual CollabPostResponse? Post { get; set; }


    }


}
