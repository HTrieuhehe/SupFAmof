using AutoMapper;
using System.Text;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;
using SupFAmof.Service.DTO.Request.Admission;

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
            CreateMap<Account, AccountResponse>()
                .ReverseMap();
            CreateMap<AccountInformation, AccountResponse>().ReverseMap();
            CreateMap<AccountInformation, AccountInformationResponse>()
                .ReverseMap();
           
            CreateMap<CreateAccountRequest, Account>();
            CreateMap<CreateAccountInformationRequest, AccountInformation>().ReverseMap();
            CreateMap<UpdateAccountRequest, Account>();
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







            #endregion

            #region Admission PostTitle
            CreateMap<PostTitle, PostTitleResponse>()
                .ReverseMap();
            CreateMap<CreatePostTitleRequest, PostTitle>();
            CreateMap<UpdatePostTitleRequest, PostTitle>();
            #endregion

            #region Admission Training Certificate
            CreateMap<TranningCertificate, TrainingCertificateResponse>().ReverseMap();
            CreateMap<CreateTrainingCertificateRequest, TranningCertificate>();
            CreateMap<UpdateTrainingCertificateRequest, TranningCertificate>();
            #endregion
        }
    }
}
