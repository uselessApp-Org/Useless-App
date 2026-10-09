using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace UselessApp.Services;

    public class JwtService
    {
        private readonly SymmetricSecurityKey _key;

        public JwtService()
        {
         _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-super-secret-key-I-hope-this-is-enough"));
        }

        public string GenerateToken(string username)
        {
           var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
           var token= new JwtSecurityToken(
                issuer: "UselessApp",
                audience: "UselessApp",
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, username)
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);
                
           return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
