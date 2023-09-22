using AutoMapper;
using System.Text;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Request.Staff;
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
            
            #region Staff
            CreateMap<staff, StaffResponse>().ReverseMap();
            CreateMap<CreateStaffRequest, staff>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.Password)));
            CreateMap<UpdateStaffRequest, staff>()
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

            CreateMap<PostRegistrationDetailRequest, PostRegistrationDetail>().ReverseMap();
            CreateMap<PostRegistrationRequest, PostRegistration>()
              .ForMember(dest => dest.PostRegistrationDetails, opt =>
              {
                  opt.MapFrom(src => src.PostRegistrationDetails);
              })
              .ReverseMap();

            CreateMap<PostRegistrationUpdateRequest,PostRegistration>()
                 .ForMember(dest => dest.PostRegistrationDetails, opt =>
                 {
                     opt.MapFrom(src => src.PostRegistrationDetails);
                 })
                .ReverseMap();
            CreateMap<PostRegistrationDetailUpdateRequest,PostRegistrationDetail>().ReverseMap();

            CreateMap<PostRgupdateHistory, PostRgupdateHistoryResponse>().ReverseMap();

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
            CreateMap<Post, TrainingPositionResponse>().ReverseMap();

            CreateMap<PostPosition, PostPositionResponse>().ReverseMap();
            CreateMap<TrainingPosition, TrainingPositionResponse>().ReverseMap();

            CreateMap<CreatePostRequest, Post>();

            CreateMap<CreatePostPositionRequest, PostPosition>();
            CreateMap<CreatePostTrainingPositionRequest, TrainingPosition>();

            #endregion

            #region Post
            CreateMap<Post, PostResponse>().ReverseMap();
            #endregion
        }
    }
}
