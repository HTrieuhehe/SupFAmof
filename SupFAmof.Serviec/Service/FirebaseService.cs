﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service
{
    public class FireBaseService
    {
        public static int GetUserIdFromHeaderToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(accessToken);
            }
            catch (Exception)
            {
                return -1;
            }
            var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
            var claims = tokenS.Claims;
            var id = Int32.Parse(claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value.ToString());
            return id;
        }
    }
}
