﻿using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;
using SupFAmof.Data.Entity;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountService
    {
        Task<BaseResponsePagingViewModel<AccountResponse>> GetAccounts(AccountResponse request, PagingRequest paging);
        Task<BaseResponseViewModel<AccountResponse>> GetAccountById(int accountId);
        Task<BaseResponsePagingViewModel<AccountResponse>> SearchCollaboratorByEmail(string email, PagingRequest paging);
        Task<BaseResponseViewModel<AccountResponse>> GetAccountByEmail(string email);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccount(CreateAccountRequest request);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccountInformation(int accountId, Account account);
        Task<BaseResponseViewModel<AccountResponse>> UpdateAccount(int accountId, UpdateAccountRequest request);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAdmissionAccount(int accountId, UpdateAdmissionAccountRequest request);
        Task<BaseResponseViewModel<AccountInformationResponse>> UpdateAccountInforamtion(int accountId, UpdateAccountInformationRequest request);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAdmissionAccountAvatart(int accountId, UpdateAccountAvatar request);
        Task<BaseResponseViewModel<AccountResponse>> DisableAccount(int accountId);
        Task Logout(ExpoTokenLogoutRequest expoToken, int accountId, int status);
        Task<BaseResponseViewModel<LoginResponse>> AdmissionLogin(ExternalAuthRequest data);
        Task<BaseResponseViewModel<dynamic>> InputVerifycationCode(int accountId, int code, int roleId);
        Task<BaseResponseViewModel<LoginResponse>> Login(ExternalAuthRequest data);
        Task<BaseResponseViewModel<AccountReactivationResponse>> EnableAccount(int accountId);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountAdmissionById(int accountId);
        Task<BaseResponseViewModel<AccountResponse>> UpdateAccountAvatar(int accountId, UpdateAccountAvatar request);
        Task<BaseResponseViewModel<TotalAccountResponse>> ViewCollaborator(int accountId);
        Task<BaseResponsePagingViewModel<ManageCollabAccountResponse>> GetAllCollabAccount(int accountId,PagingRequest paging);

        #region interface dùng để quét CCCD

        Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationFrontImg(int accountId, UpdateCitizenIdentificationFrontImg request);
        Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationBackImg(int accountId, UpdateCitizenIdentificationBackImg request);
        Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationFrontImgInformation(int accountId, UpdateCitizenIdentification request);
        Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationBackImgInformation(int accountId, UpdateCitizenIdentification2 request);

        #endregion

        Task<BaseResponseViewModel<AccountInformationResponse>> UpdateAccountInformationTest(int accountId, UpdateAccountInformationRequestTest request);


        #region update Collaborator Credential

        Task<BaseResponseViewModel<AccountResponse>> UpdateCollaboratorCredential(int accountId, int collaboratorAccountId);
        Task<BaseResponseViewModel<AccountResponse>> DisableCollaboratorCredential(int accountId, int collaboratorAccountId);

        #endregion
    }
}
