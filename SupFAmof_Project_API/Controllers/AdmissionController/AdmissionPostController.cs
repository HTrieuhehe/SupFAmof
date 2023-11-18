using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Web;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using System.Net.NetworkInformation;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionPostController : ControllerBase
    {
        private readonly IPostService _postService;

        public AdmissionPostController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Create Post 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> CreatePost
        ([FromBody] CreatePostRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.CreateAdmissionPost(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Posts 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdmissionPostResponse>>> GetPosts
        ([FromQuery] AdmissionPostResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.GetAdmissionPosts(account.Id, filter, paging);
            }
            catch(ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Post by PostCode
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getByPostCode")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> GetPostByPostCode
        ([FromQuery] string postCode)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.GetPostByPostcode(postCode);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Post by Account Id
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getByAccountId")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdmissionPostResponse>>> GetPostByAccountId
        ([FromQuery] AdmissionPostResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.GetPostByAccountId(account.Id, filter, paging);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Confirm ending Post
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("confirmEndPost")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> ConfirmEndingPost
            ([FromQuery] int postId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.EndPost(account.Id, postId);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Re Open Post Registration
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("Re-openPostRegistration")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> ReOpenPost
            ([FromQuery] int postId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.ReOpenPostRegistration(account.Id, postId);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Confirm running Post
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("confirmRunningPost")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> ClosePostRegistration
            ([FromQuery] int postId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.ClosePostRegistration(account.Id, postId);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Update Post
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> UpdatePost
            ([FromQuery] int postId, [FromBody] UpdatePostRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.UpdateAdmissionPost(account.Id, postId, request);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        //[HttpPut("confirmPost")]
        //public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> ConfirmPost
        //    ([FromQuery] int postId)
        //{
        //    try
        //    {
        //        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //        var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
        //        if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
        //        {
        //            return Unauthorized();
        //        }
        //        return await _postService.(account.Id, postId);
        //    }
        //    catch (ErrorResponse ex)
        //    {
        //        return BadRequest(ex.Error);
        //    }
        //}

        /// <summary>
        /// Get Post By Code 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> SearchPost
        ([FromQuery] string searchPost, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.AdmissionSearchPost(account.Id, searchPost, paging);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Delete Post Position 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpDelete("position/delete")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> DeletePosition
        ([FromQuery] int positionId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.DeletePostPosition(account.Id, positionId);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Delete Post 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpDelete("post/delete")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> DeletePost
        ([FromQuery] int postId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.DeletePost(account.Id, postId);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create new Post Position
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("updateNewPosition")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> AddNewPosition
            ([FromQuery] int postId, [FromBody] CreatePostPositionRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.CreatePostPosition(account.Id, postId, request);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }
    }
}
