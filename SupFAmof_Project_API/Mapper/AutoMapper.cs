using AutoMapper;
using System.Text;
using Expo.Server.Models;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Response.Admin;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;

namespace SupFAmof.API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Role
            CreateMap<Role, RoleResponse>().ReverseMap();
            CreateMap<CreateRoleRequest, Role>();
            CreateMap<UpdateRoleRequest, Role>();
            #endregion

            #region Account
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<Account, CollaboratorAccountReponse>().ReverseMap();
            CreateMap<AccountInformation, AccountResponse>().ReverseMap();
            CreateMap<AccountInformation, AccountInformationResponse>()
                .ReverseMap();

            CreateMap<AccountReactivation, AccountReactivationResponse>().ReverseMap();
            CreateMap<CreateAccountReactivationRequest, AccountReactivation>();

            CreateMap<CreateAccountRequest, Account>();
            CreateMap<CreateAccountInformationRequest, AccountInformation>().ReverseMap();
            CreateMap<UpdateAccountInformationRequest, AccountInformation>();
            CreateMap<UpdateAccountRequest, Account>();
            CreateMap<UpdateAccountAvatar, Account>();
            #endregion

            #region Account Report
            CreateMap<AccountReport, AccountReportResponse>().ReverseMap();

            CreateMap<CreateAccountReportRequest, AccountReport>();
            #endregion

            #region Admin Account
            CreateMap<Admin, AdminAccountResponse>().ReverseMap();
            CreateMap<CreateAdminAccountRequest, Admin>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.Password)));
            CreateMap<UpdateAdminAccountRequest, Admin>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.Password)));
            #endregion

            #region AccountBanking
            CreateMap<AccountBanking, AccountBankingResponse>().ReverseMap();
            CreateMap<CreateAccountBankingRequest, AccountBanking>();
            CreateMap<UpdateAccountBankingRequest, AccountBanking>();

            #endregion

            #region AdmissionAccount
            CreateMap<Account, AdmissionAccountResponse>()
                .ReverseMap();
            CreateMap<UpdateAdmissionAccountRequest, Account>();

            #endregion

            #region PostRegistration
            CreateMap<PostRegistration, PostRegistrationResponse>()
              .ForMember(dest => dest.PostRegistrationDetails, opt =>
              {
                  opt.MapFrom(src => src.PostRegistrationDetails);
              })
              .ReverseMap();

            CreateMap<PostRegistrationDetail, PostRegistrationDetailResponse>()
                .ReverseMap();

            CreateMap<Post, AdmissionPostsResponse>()
                .ForMember(dest => dest.PostCategoryName, opt=> opt.MapFrom(src=>src.PostCategory.PostCategoryType))
                .ForMember(dest=>dest.CollabRequest , opt=>opt.MapFrom(src=>src.PostRegistrationDetails))
                .ReverseMap();

            CreateMap<PostRegistration, CollabRegistrationResponse>()
                .ForMember(dest => dest.PostRegistrationDetail, opt=>opt.MapFrom(src=>src.PostRegistrationDetails.FirstOrDefault()))
                .ReverseMap();
            CreateMap<PostRegistrationDetail, CollabRegistrationDetailResponse>()
                .ForMember(dest => dest.PostPosition, opt => opt.MapFrom(src => src.Post.PostPositions.FirstOrDefault(x => x.Id == src.PositionId)))
                .ReverseMap();
            CreateMap<Post, CollabPostResponse>()
    .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account))
    .ForMember(dest => dest.PostCategory, opt => opt.MapFrom(src => src.PostCategory));
            


            CreateMap<PostRegistrationDetail, CollabRegistrationFormResponse>()
                .ForMember(dest=>dest.Id ,opt=>opt.MapFrom(src=>src.PostRegistrationId))
                .ForMember(dest => dest.RegistrationCode, opt => opt.MapFrom(src => src.PostRegistration.RegistrationCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PostRegistration.Status))
                .ForMember(dest => dest.SchoolBusOption, opt => opt.MapFrom(src => src.PostRegistration.SchoolBusOption))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.PostRegistration.CreateAt))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.PostRegistration.UpdateAt))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.PostRegistration.AccountId))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.PostRegistration.Account.Name))
                .ForMember(dest => dest.AccountEmail, opt => opt.MapFrom(src => src.PostRegistration.Account.Email))
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Post.PostPositions.FirstOrDefault(x=>x.Id == src.PositionId).PositionName))
                .ReverseMap();
            CreateMap<PostRegistrationRequest, PostRegistration>()
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.SchoolBusOption, opt => opt.MapFrom(src => src.SchoolBusOption)).ReverseMap();
            CreateMap<PostRegistrationUpdateRequest, PostRegistration>()
                .ReverseMap();

            CreateMap<PostRgupdateHistory, PostRgupdateHistoryResponse>().ReverseMap();

            CreateMap<PostRegistration, ReportPostRegistrationResponse>()
                .ForMember(dest => dest.PostRegistrationDetails, opt =>
                {
                    opt.MapFrom(src => src.PostRegistrationDetails);
                })
              .ReverseMap();
            CreateMap<PostRegistrationDetail, ReportPostRegistrationDetailResponse>().ReverseMap();

            #endregion

            #region Post Attendee
            CreateMap<PostRegistration, PostAttendeeRequest>()
                  .ForMember(dest => dest.PositionId, opt =>
                  {
                      opt.MapFrom(src => src.PostRegistrationDetails.First().PositionId);
                  })
                  .ForMember(dest => dest.PostId, opt =>
                  {
                      opt.MapFrom(src => src.PostRegistrationDetails.First().PostId);
                  })

                .ReverseMap();
            CreateMap<PostAttendee, PostAttendeeRequest>().ReverseMap();
            #endregion

            #region Admission Post Category
            CreateMap<PostCategory, PostCategoryResponse>()
                .ReverseMap();
            CreateMap<CreatePostCategoryRequest, PostCategory>();
            CreateMap<UpdatePostCategoryRequest, PostCategory>();
            #endregion

            #region Admission Training Certificate
            CreateMap<TrainingCertificate, TrainingCertificateResponse>().ReverseMap();
            CreateMap<CreateTrainingCertificateRequest, TrainingCertificate>();
            CreateMap<UpdateTrainingCertificateRequest, TrainingCertificate>();
            #endregion

            #region Admission Account Certificate
            CreateMap<AccountCertificate, AccountCertificateResponse>().ReverseMap();
            CreateMap<CreateAccountCertificateRequest, AccountCertificate>();
            #endregion

            #region Admission Post
            CreateMap<Post, AdmissionPostResponse>().ReverseMap();
            CreateMap<Post, PostPositionResponse>().ReverseMap();

            CreateMap<PostPosition, PostPositionResponse>().ReverseMap();

            CreateMap<CreatePostRequest, Post>();

            CreateMap<CreatePostPositionRequest, PostPosition>();

            CreateMap<UpdatePostRequest, Post>()
              .ForMember(dest => dest.PostPositions, opt => opt.MapFrom(src => src.PostPositions))
              .ReverseMap();

            CreateMap<UpdatePostPositionRequest, PostPosition>()
              .ReverseMap();

            #endregion

            #region Admission Contract

            CreateMap<Contract, AdmissionContractResponse>().ReverseMap();
            CreateMap<Contract, ContractResponse>().ReverseMap();
            CreateMap<CreateAdmissionContractRequest, Contract>().ReverseMap();
            CreateMap<UpdateAdmissionContractRequest, Contract>().ReverseMap();

            #endregion

            #region Admission Account Report Problem

            CreateMap<Complaint, AdmissionComplaintResponse>().ReverseMap();
            CreateMap<UpdateAdmissionAccountReportProblemRequest, Complaint>().ReverseMap();

            #endregion

            #region Contract
            CreateMap<Contract, ContractResponse>().ReverseMap();
            #endregion

            #region Post
            CreateMap<Post, PostResponse>().ReverseMap();
            CreateMap<Post, ReportPostResponse>().ReverseMap();
            CreateMap<PostPosition, ReportPostPositionResponse>().ReverseMap();
            #endregion

            #region DocumentTemplate
            CreateMap<AdmissionDocumentResponse,DocumentTemplate>().ReverseMap();
            CreateMap<DocumentRequest, DocumentTemplate>().ReverseMap();
            CreateMap<DocumentUpdateRequest,DocumentTemplate>().ReverseMap();
            #endregion

            #region Profile Reactivtion
            CreateMap<AccountReactivation, AccountReactivationResponse>().ReverseMap();
            #endregion

            #region CheckIn - CheckOut
            CreateMap<CheckInRequest, CheckAttendance>().ReverseMap();
            CreateMap<CheckOutRequest, CheckAttendance>().ReverseMap();
            #endregion

            #region Mail
                CreateMap<PostRegistration, PostAttendee>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PostRegistrationDetails.FirstOrDefault().Post.PostPositions.FirstOrDefault().Id))
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostRegistrationDetails.FirstOrDefault().Post.Id))
            .ForMember(dest => dest.ConfirmAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.PostRegistrationDetails.FirstOrDefault().Post.PostPositions.FirstOrDefault()))
            .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.PostRegistrationDetails.FirstOrDefault().Post))
            .ReverseMap();
            #endregion

            #region Account Banned
            CreateMap<AccountBanned, AccountBannedResponse>().ReverseMap();
            CreateMap<CreateAccountBannedRequest, AccountBanned>().ReverseMap();
            CreateMap<UpdateAccountBannedRequest, AccountBanned>().ReverseMap();
            #endregion

            #region Account Contract
            CreateMap<CreateAccountContractRequest, AccountContract>().ReverseMap();
            CreateMap<AccountContract, AccountContractResponse>().ReverseMap();
            #endregion

            #region Account Report Problem

            CreateMap<Complaint, CompaintResponse>().ReverseMap();
            CreateMap<CreateAccountReportProblemRequest, Complaint>();

            #endregion

            #region Notification History

            CreateMap<NotificationHistory, NotificationHistoryResponse>().ReverseMap();
            CreateMap<CreateNotificationHistoryRequest, NotificationHistory>();
            CreateMap<PushTicketResponse, NotificationHistoryResponse>()
                .ReverseMap();

            #endregion
        }
    }
}
