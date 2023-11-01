﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.Service.Service
{
    public class FireBaseService
    {
        public static GetUser GetUserIdFromHeaderToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(accessToken);
            }
            catch (Exception)
            {
                return new GetUser()
                {
                    Id = (int)SystemAuthorize.NotAuthorize
                };
            }
            var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
            var claims = tokenS.Claims;
            var id = Int32.Parse(claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value.ToString());
            var roleId = Int32.Parse(claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value.ToString());
            var postPermission = claims.Where(x => x.Type == "PostPermission").FirstOrDefault().Value.ToString();
            return new GetUser()
            {
                Id = id,
                RoleId = roleId,
                PostPermission = postPermission,
            };
        }

        public class GetUser
        {
            public int Id { get; set; }
            public int RoleId { get; set; }
            public string? PostPermission { get; set; }
        }
    }
}
