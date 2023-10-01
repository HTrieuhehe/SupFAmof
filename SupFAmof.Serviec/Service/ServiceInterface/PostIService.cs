using SupFAmof.Service.DTO.Request;
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
        Task<BaseResponsePagingViewModel<CollaboratorAccountReponse>> GetAccountByPostPositionId(int positionId, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> GetPostByPostcode(string postCode);
        Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetPostByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> CreateAdmissionPost(int accountId, CreatePostRequest request);
        Task<BaseResponseViewModel<AdmissionPostResponse>> UpdateAdmissionPost(int accountId, int postId, UpdatePostRequest request);
        Task<BaseResponseViewModel<AdmissionPostResponse>> ClosePost(int accountId, int postId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> EndPost(int accountId, int postId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> AdmissionSearchPost(int accountId, string searchPost);


        #endregion

        #region Collaborator Post IService
        Task<BaseResponsePagingViewModel<PostResponse>> GetPosts(int accountId, PostResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<PostResponse>> SearchPost(string searchPost, PagingRequest paging);
        #endregion
    }
}
