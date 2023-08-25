using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Get Posts 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostResponse>>> GetPosts
        ([FromQuery] int accountId, [FromQuery] PostResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Student)
                //{
                //    return Unauthorized();
                //}
                return await _postService.GetPosts(accountId, filter, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        // <summary>
        /// Get Post By Code 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getByCode")]
        public async Task<ActionResult<BaseResponseViewModel<PostResponse>>> GetPostByCode
        ([FromQuery] int postCode)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Student)
                //{
                //    return Unauthorized();
                //}
                return await _postService.GetPostByCode(postCode);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
