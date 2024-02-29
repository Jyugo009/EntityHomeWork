using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Services
{
    internal class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtInfo:JwtKey"] ?? throw new ArgumentNullException("TokenKey")));
        }

        public string GetToken(object userParam)
        {
            var claims = new List<Claim>();

            if (userParam is Librarian librarian)
            {
                claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.NameId, librarian.Login));
            }
            else if (userParam is Reader reader)
            {
                claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.NameId, reader.Login));
            }

            var signature = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var descr = new SecurityTokenDescriptor
            {
                SigningCredentials = signature,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descr);

            return handler.WriteToken(token);
        }
    }
}
