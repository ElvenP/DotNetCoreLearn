using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Admin.Core.Common.Configs;
using Microsoft.IdentityModel.Tokens;

namespace Admin.Core.Common.Auth
{
    public class UserToken:IUserToken
    {

        private readonly JwtConfig _jwtConfig;

        public UserToken(JwtConfig jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }
        public string Build(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecurityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var refreshExpires = DateTime.Now.AddMinutes(_jwtConfig.RefreshExpires).ToString();
            claims = claims.Append(new Claim(ClaimAttributes.RefreshExpires, refreshExpires)).ToArray();

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_jwtConfig.Expires),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Claim[] Decode(string jwtToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
            return jwtSecurityToken?.Claims?.ToArray();
        }
    }
}