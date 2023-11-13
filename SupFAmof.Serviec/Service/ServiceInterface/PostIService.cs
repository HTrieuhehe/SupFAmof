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
        Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetAdmissionPosts(int accountId, AdmissionPostResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> GetPostByPostcode(string postCode);
        Task<BaseResponsePagingViewModel<AdmissionPostResponse>> GetPostByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> CreateAdmissionPost(int accountId, CreatePostRequest request);
        Task<BaseResponseViewModel<AdmissionPostResponse>> UpdateAdmissionPost(int accountId, int postId, UpdatePostRequest request);
        Task<BaseResponseViewModel<AdmissionPostResponse>> ClosePostRegistration(int accountId, int postId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> ReOpenPostRegistration(int accountId, int postId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> EndPost(int accountId, int postId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> AdmissionSearchPost(int accountId, string searchPost, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionPostResponse>> DeletePostPosition(int accountId, int positionId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> DeletePost(int accountId, int postId);
        Task<BaseResponseViewModel<AdmissionPostResponse>> CreatePostPosition(int accountId, int postId, CreatePostPositionRequest request);

        #endregion

        #region Collaborator Post IService
        Task<BaseResponsePagingViewModel<PostResponse>> GetPosts(int accountId, string search, PostResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<PostResponse>> GetPostReOpen(int accountId, PostResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<PostResponse>> SearchPost(string searchPost, PagingRequest paging);
        #endregion
    }
}
