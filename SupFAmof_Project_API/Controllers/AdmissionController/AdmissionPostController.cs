﻿using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<BaseResponseViewModel<AdmissionPostResponse>>> CreateAccountCertificate
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
                return await _postService.GetAdmissionPosts(filter, paging);
            }
            catch(ErrorResponse ex)
            {
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
        ([FromQuery] int postCode)
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
        ([FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postService.GetPostByAccountId(account.Id, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}