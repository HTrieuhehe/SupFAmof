﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionPostCategoryController : ControllerBase
    {
        private readonly IPostCategoryService _postTitleService;

        public AdmissionPostCategoryController(IPostCategoryService postCategoryService)
        {
            _postTitleService = postCategoryService;
        }

        /// <summary>
        /// Get List Post Categoryies
        /// </summary>    
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostCategoryResponse>>> GetPostCategories
            ([FromQuery] PostCategoryResponse request, [FromQuery] PagingRequest paging)
        {
            try
            {
                return await _postTitleService.GetPostCategories(request, paging);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Get Post Category By Id                        
        /// </summary>
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> GetPostCategoryById
            ([FromQuery] int postCategoryId)
        {
            try
            {
                return await _postTitleService.GetPostCategoryById(postCategoryId);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Create Post Category                      
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> CreatePostCategory
            ([FromBody] CreatePostCategoryRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postTitleService.CreatePostCategory(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Update Post Category                     
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> UpdatePostCategory
            ([FromQuery] int postCategoryId, [FromBody] UpdatePostCategoryRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postTitleService.UpdatePostCategory(account.Id, postCategoryId, request);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Search Post Category by Description or Type                    
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostCategoryResponse>>> SearchPostCategory
            ([FromQuery] string search, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postTitleService.SearchPostCategory(search, paging);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Disable Post Category                     
        /// </summary>
        [HttpPut("disable")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> DisablePostCategory
            ([FromQuery] int postCategoryId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postTitleService.DisablePostCategory(account.Id, postCategoryId);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }
    }
}
