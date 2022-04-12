using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            //CREATE A LIST OF CLAIMS TO STORE IN A JWT WEB TOKEN
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            /////////////////////////////////////////////////////

            //SIGNING CREDENTIALS FOR A TOKEN BASED ON THE SYMMETRIC SECURITY KEY VALUE
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            /////////////////////////////////////////////////////

            //APPLY SPECIFIC SETTINGS FOR THE GENERATED TOKEN
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = creds,
                NotBefore = DateTime.Now,
                Issuer = _config["appsettings.Development:Issuer"],
                Audience = _config["appsettings.Development:Audience"]
            };
            //////////////////////////////////////////////////////

            //CREATE JWT TOKEN HANDLER
            var tokenHandler = new JwtSecurityTokenHandler();
            //////////////////////////////////////////////////////

            //CREATE THE JWT TOKEN IN VIRTUAL MEMORY USING THE CREATED ITOKENSERVICE INTERFACE
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //////////////////////////////////////////////////////

            //SEND JWT TOKEN VALUE BACK TO THE INSTANCE WHERE IT WAS INVOKED
            return tokenHandler.WriteToken(token);
            //////////////////////////////////////////////////////
        }
    }
}