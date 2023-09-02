﻿using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IPostService
    {
        #region Admission Post IService
        Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetAdmissionPosts(AdmissionPostResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> GetPostByPostcode(int postCode);
        Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetPostByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> CreateAdmissionPost(int accountId, CreatePostRequest request);
        Task<BaseResponseViewModel<AdmissionPostResponse>> UpdateAdmissionPost(int accountId, int postId, UpdatePostRequest request);
        #endregion

        #region Collaborator Post IService
        #endregion
    }
}