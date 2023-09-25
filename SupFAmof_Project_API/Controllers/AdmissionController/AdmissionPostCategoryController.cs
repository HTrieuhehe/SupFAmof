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

        public AdmissionPostCategoryController(IPostCategoryService postTitleService)
        {
            _postTitleService = postTitleService;
        }

        /// <summary>
        /// Get List Post Title
        /// </summary>    
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostCategoryResponse>>> GetPostTitles
            ([FromQuery] PostCategoryResponse request, [FromQuery] PagingRequest paging)
        {
            try
            {
                return await _postTitleService.GetPostTitles(request, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Post Title By Id                        
        /// </summary>
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> GetPostTitleById
            ([FromQuery] int postTitleId)
        {
            try
            {
                return await _postTitleService.GetPostTitleById(postTitleId);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create Post                       
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> CreatePostTitle
            ([FromBody] CreatePostCategoryRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postTitleService.CreatePostTitle(request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Update Post                        
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<PostCategoryResponse>>> UpdatePostTitle
            ([FromQuery] int postTitleId, [FromBody] UpdatePostCategoryRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postTitleService.UpdatePostTitle(postTitleId, request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}