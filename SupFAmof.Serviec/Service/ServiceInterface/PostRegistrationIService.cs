﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IPostRegistrationService
    {
        Task<BaseResponsePagingViewModel<CollabRegistrationResponse>> GetPostRegistrationByAccountId(int accountId, PagingRequest paging, CollabRegistrationResponse filter);
        Task<BaseResponseViewModel<CollabRegistrationResponse>> CreatePostRegistration(int accountId,PostRegistrationRequest request);
        Task<BaseResponseViewModel<dynamic>> CancelPostregistration(int accountId,int postRegistrationId);
        Task<BaseResponseViewModel<dynamic>> UpdatePostRegistration(int accountId,int PostRegistrationId, PostRegistrationUpdateRequest request);
        Task<BaseResponsePagingViewModel<AdmissionPostsResponse>> AdmssionPostRegistrations(int admissionAccountId, PagingRequest paging);
        Task<BaseResponseViewModel<List<PostRegistrationResponse>>> ApproveUpdateRequest(List<int> Ids, bool approve);
        Task<BaseResponseViewModel<dynamic>> ApprovePostRegistrationRequest(int accountId,List<int> postRegistrationIds, bool approve);

        Task<BaseResponsePagingViewModel<PostRegistrationResponse>> GetAccountByPostPositionId(int accountId, int positionId, string searchEmail, PostRegistrationResponse filter,  PagingRequest paging);
        Task<BaseResponsePagingViewModel<PostRgupdateHistoryResponse>> GetUpdateRequestByAccountId(int accountId, PagingRequest paging);

        Task<BaseResponsePagingViewModel<CollabRegistrationResponse>> GetPostRegistrationCheckIn(int accountId, PagingRequest paging);

    }

}
