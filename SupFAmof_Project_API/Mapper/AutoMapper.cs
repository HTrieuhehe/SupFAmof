using AutoMapper;
using System.Text;
using Expo.Server.Models;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Request.Admin;
using SupFAmof.Service.DTO.Response.Admin;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.API.Controllers.AdmissionController;
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

            #region Admin System Management

            CreateMap<Role, AdminSystemManagementResponse>().ReverseMap();
            CreateMap<CreateAdminSystemManagementRequest, Role>();
            CreateMap<UpdateAdminSystemManagementRequest, Role>();

            #endregion

            #endregion

            #region Account

            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<Account, NewCollaboratorResponse>().ReverseMap();
            CreateMap<Account, CollaboratorAccountReponse>().ReverseMap();
            CreateMap<AccountInformation, AccountResponse>().ReverseMap();
            CreateMap<AccountInformation, AccountInformationResponse>()
                .ReverseMap();

            CreateMap<AccountReactivation, AccountReactivationResponse>().ReverseMap();
            CreateMap<CreateAccountReactivationRequest, AccountReactivation>();

            CreateMap<CreateAccountRequest, Account>();
            CreateMap<CreateAccountInformationRequest, AccountInformation>().ReverseMap();
            CreateMap<UpdateAccountInformationRequest, AccountInformation>();
            CreateMap<UpdateAccountInformationRequestTest, AccountInformation>();
            CreateMap<UpdateAccountRequest, Account>();
            CreateMap<UpdateAccountAvatar, Account>();

            CreateMap<Account,ManageCollabAccountResponse>()
                .ForMember(x=>x.certificates,opt=>opt.MapFrom(src=>src.AccountCertificateAccounts))
                .ForMember(x => x.IdStudent, opt => opt.MapFrom(src => src.AccountInformation.IdStudent))
                .ReverseMap();
            CreateMap<Account, ContractCollaboratorResponse>().ReverseMap();
            CreateMap<AccountCertificate, CertificateResponse>()
                .ForMember(x=>x.CertificateName,opt=>opt.MapFrom(src=>src.TrainingCertificate.CertificateName))
                .ReverseMap();

            CreateMap<Account, AttendanceAccountResponse>();


            CreateMap<UpdateCitizenIdentification, AccountInformation>().ReverseMap();
            CreateMap<UpdateCitizenIdentification2, AccountInformation>().ReverseMap();
            CreateMap<UpdateCitizenIdentificationFrontImg, AccountInformation>().ReverseMap();
            CreateMap<UpdateCitizenIdentificationBackImg, AccountInformation>().ReverseMap();

            #region Admission Manage Admission Account 

            CreateMap<Account, AdminAccountAdmissionResponse>().ReverseMap();
            CreateMap<UpdateAdminAccountAdmissionRequest, Account>();

            #endregion

            #endregion

            #region Account Report
            CreateMap<AccountReport, AccountReportResponse>().ReverseMap();

            CreateMap<CreateAccountReportRequest, AccountReport>();
            CreateMap<Account, CollabReportResponse>()
                .ForMember(x=>x.IdStudent , opt=>opt.MapFrom(src=>src.AccountInformation.IdStudent))
                .ForMember(x => x.IdentityNumber, opt => opt.MapFrom(src => src.AccountInformation.IdentityNumber))
                .ForMember(x => x.TaxNumber, opt => opt.MapFrom(src => src.AccountInformation.TaxNumber))
                .ForMember(x => x.Beneficiary, opt => opt.MapFrom(src => src.AccountBankings.First().Beneficiary))
                .ForMember(x => x.AccountNumber, opt => opt.MapFrom(src => src.AccountBankings.First().AccountNumber))
                .ForMember(x => x.BankName, opt => opt.MapFrom(src => src.AccountBankings.First().BankName))
                .ForMember(x => x.Branch, opt => opt.MapFrom(src => src.AccountBankings.First().Branch))
                .ReverseMap();
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
              .ReverseMap();
            
            CreateMap<PostRegistration, PostRGUpdatePostRegistrationResponse>()
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
              .ReverseMap();

            CreateMap<Post, AdmissionPostsResponse>()
                .ForMember(dest => dest.PostCategoryName, opt=> opt.MapFrom(src=>src.PostCategory.PostCategoryType))
                .ForMember(dest=>dest.Positions,opt=>opt.MapFrom(src=>src.PostPositions))
                .ReverseMap();

            CreateMap<PostRegistration, CollabRegistrationResponse>()
                .ForMember(dest=>dest.Post,opt=>opt.MapFrom(src=>src.Position.Post))
                .ForMember(dest => dest.PostPosition, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.PositionDate, opt => opt.MapFrom(src => src.Position.Date))
                .ForMember(dest => dest.PostCategoryId, opt => opt.MapFrom(src => src.Position.Post.PostCategoryId))
                .ReverseMap();

            CreateMap<Post, CollabPostResponse>()
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account))
                .ForMember(dest => dest.PostCategory, opt => opt.MapFrom(src => src.PostCategory))
                .ReverseMap();
            
            CreateMap<Post, CollabRegistrationUpdatePostResponse>()
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account))
                .ForMember(dest => dest.PostCategory, opt => opt.MapFrom(src => src.PostCategory))
                .ReverseMap();

            CreateMap<Post, PostRGUpdatePostResponse>().ReverseMap();

            CreateMap<PostRegistration, CollabRegistrationUpdateViewResponse>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Position.Post))
                .ForMember(dest => dest.PostPosition, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.PositionDate, opt => opt.MapFrom(src => src.Position.Date))
                .ForMember(dest => dest.PostCategoryId, opt => opt.MapFrom(src => src.Position.Post.PostCategoryId))
                .ForMember(dest => dest.PostPositionsUnregistereds, opt => opt.MapFrom(src => src.Position.Post.PostPositions))
                .ForMember(dest => dest.IsUpdated, opt => opt.MapFrom(src => src.UpdateAt.HasValue))
                .ReverseMap();

            CreateMap<PostRegistration, RegistrationMapResponse>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Position.Post))
                .ForMember(dest => dest.PostPosition, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.PositionDate, opt => opt.MapFrom(src => src.Position.Date))
                .ForMember(dest => dest.PostCategoryId, opt => opt.MapFrom(src => src.Position.Post.PostCategoryId))
                .ForMember(dest => dest.IsUpdated, opt => opt.MapFrom(src => src.UpdateAt.HasValue))
                .ReverseMap();

            CreateMap<PostPosition, AdmissionPostPositionResponse>()
                .ForMember(dest=>dest.CollabRequest ,opt=>opt.MapFrom(src=>src.PostRegistrations))
                .ReverseMap();

            CreateMap<PostPosition, PostRGUpdateOriginalPositionResponse>().ReverseMap();
          CreateMap<PostRegistration, CollabRegistrationFormResponse>()
                .ReverseMap();
            CreateMap<PostRegistrationRequest, PostRegistration>()
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.SchoolBusOption, opt => opt.MapFrom(src => src.SchoolBusOption)).ReverseMap();
            CreateMap<PostRegistrationUpdateRequest, PostRegistration>()
                .ReverseMap();
            CreateMap<UpdateSchoolBusRequest, PostRegistration>();

            CreateMap<PostRgupdateHistory, PostRgupdateHistoryResponse>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Position.Post))
                .ForMember(dest => dest.PostPositionNeedToBeUpdated, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.PostPositionOriginal, opt => opt.MapFrom(src => src.OriginalPosition))
                .ReverseMap();

            CreateMap<PostRegistration, ReportPostRegistrationResponse>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Position.Post))
                .ForMember(dest => dest.PostPosition, opt => opt.MapFrom(src => src.Position))
              .ReverseMap();


            CreateMap<PostRgupdateHistory, AdmissionUpdateRequestResponse>()
                .ForMember(dest => dest.OriginalPosition, opt => opt.MapFrom(src => src.OriginalPosition))
                .ForMember(dest => dest.PostPositionNeedToBeUpdated, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.PostRegistration.Position.Post));
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
            CreateMap<EventDaysCertificate, TrainingEventDay>().ReverseMap();
            #endregion
            CreateMap<TrainingEventDay, ViewCollabInterviewClassResponse>()
                .ReverseMap();
            CreateMap<TrainingRegistration, TrainingRegistrationResponse>().ReverseMap();
            CreateMap<Account, AccountCertificateRegistrationResponse>()
                .ForMember(dest=>dest.IdStudent ,opt=>opt.MapFrom(src=>src.AccountInformation.IdStudent))
                .ReverseMap();
            CreateMap<TrainingCertificate, TrainingCertificateRegistrationResponse>().ReverseMap();
            #region Admission Account Certificate
            CreateMap<AccountCertificate, AccountCertificateResponse>().ReverseMap();
            CreateMap<CreateAccountCertificateRequest, AccountCertificate>();
            CreateMap<UpdateAccountCertificateRequest, AccountCertificate>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AccountCertificateId))
                .ReverseMap(); 

            CreateMap<TrainingCertificateRegistration, TrainingRegistration>().ReverseMap();
            CreateMap<TrainingCertificate, AdmissionGetCertificateRegistrationResponse>()
                .ForMember(dest=>dest.Registrations,opt=>opt.MapFrom(src=>src.TrainingRegistrations))
                .ForMember(dest=>dest.RegisterAmount,opt=>opt.MapFrom(src=>src.TrainingRegistrations.Count()))
                .ReverseMap();
            CreateMap<TrainingEventDay, TrainingEventDayResponse>().ReverseMap();
            CreateMap<TrainingRegistration, CollabRegistrationsResponse>()
                .ForMember(x=>x.ConfirmAt,opt=>opt.MapFrom(src=>src.ConfirmedAt));
            CreateMap<TrainingRegistration, AccountCertificateRegistrationResponse>()
                .ForMember(x=>x.Email,opt=>opt.MapFrom(src=>src.Account.Email))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Account.Name))
                .ForMember(x => x.ImgUrl, opt => opt.MapFrom(src => src.Account.ImgUrl))
                .ForMember(x => x.IsPremium, opt => opt.MapFrom(src => src.Account.IsPremium))
                .ForMember(x => x.Phone, opt => opt.MapFrom(src => src.Account.Phone))
                .ForMember(x => x.IdStudent, opt => opt.MapFrom(src => src.Account.AccountInformation.IdStudent))

                ;
            #endregion

            #region Admission Post
            CreateMap<Post, AdmissionPostResponse>().ReverseMap();
            CreateMap<Post, PostPositionResponse>().ReverseMap();

            CreateMap<PostPosition, PostPositionResponse>()
                .ForMember(x => x.CertificateName, opt => opt.MapFrom(src => src.TrainingCertificate.CertificateName))
                .ReverseMap();

            CreateMap<CreatePostRequest, Post>();

            CreateMap<CreatePostPositionRequest, PostPosition>();

            CreateMap<UpdatePostRequest, Post>()
              .ForMember(dest => dest.PostPositions, opt => opt.MapFrom(src => src.PostPositions))
              .ReverseMap();

            CreateMap<UpdatePostPositionRequest, PostPosition>()
              .ReverseMap();

            #endregion

            #region Admission Contract

            CreateMap<Contract, AdmissionAccountContractResponse>().ReverseMap();
            CreateMap<AccountContract, AdmissionAccountContractCompleteResponse>().ReverseMap();
            CreateMap<Contract, ContractResponse>().ReverseMap();
            CreateMap<CreateAdmissionContractRequest, Contract>().ReverseMap();
            CreateMap<UpdateAdmissionContractRequest, Contract>().ReverseMap();

            #endregion

            #region Admission Account Report Problem

            CreateMap<Application, AdmissionApplicationResponse>().ReverseMap();
            CreateMap<UpdateAdmissionAccountApplicationRequest, Application>().ReverseMap();

            #endregion

            #region Contract
            CreateMap<Contract, ContractResponse>().ReverseMap();
            #endregion

            #region Post
            CreateMap<Post, PostResponse>().ReverseMap();
            CreateMap<Post, ReportPostResponse>().ReverseMap();
            CreateMap<Post, CollabReportPostResponse>().ReverseMap();
            CreateMap<PostPosition, ReportPostPositionResponse>()
                .ReverseMap();
            CreateMap<PostPosition, PostRGUpdatePositionResponse>().ReverseMap();
            CreateMap<PostPosition, AccountReportPostPositionResponse>().ReverseMap();
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
            CreateMap<CheckAttendance, CheckAttendanceResponse>().ReverseMap();

            CreateMap<Post, CheckAttendancePostResponse>()
                .ReverseMap();
            CreateMap<CheckAttendance, CheckAttendanceAdmission>()
                .ReverseMap();

            CreateMap<AdmissionConfirmAttendanceRequest, CheckAttendance>().ReverseMap();
            CreateMap<CheckAttendance, AdmissionAttendanceResponse>()
                .ForMember(x=>x.PostRegistration,opt=>opt.MapFrom(src=>src.PostRegistration))
                .ReverseMap();
            CreateMap<PostRegistration, AttendancePostRegistrationResponse>()
                .ForMember(x => x.Post, opt => opt.MapFrom(src => src.Position.Post))
                .ReverseMap();

            #endregion

            #region Mail
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

            CreateMap<Application, ApplicationResponse>().ReverseMap();
            CreateMap<CreateAccountApplicationRequest, Application>();

            #endregion

            #region Notification History

            CreateMap<NotificationHistory, NotificationHistoryResponse>().ReverseMap();
            CreateMap<CreateNotificationHistoryRequest, NotificationHistory>();
            CreateMap<PushTicketResponse, NotificationHistoryResponse>()
                .ReverseMap();

            #endregion

            #region Attendance History

            CreateMap<CheckAttendance, AdmissionAttendanceResponse>()
                // ban đầu luồng là CheckAttendance -> PostRegistration -> Account
                // bước này để nhảy map luồng PostRegistration -> Account vào cái AccountResponse luôn
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.PostRegistration.Account))
                .ReverseMap();
            CreateMap<CheckAttendance, AttendanceResponse>().ReverseMap();

            #endregion

        }
    }
}
