using DA;
using DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Yuce.MulakatApi.Controllers
{
    public class JWTAuthenticationController : Controller
    {
        private IConfiguration _configuration;

        public JWTAuthenticationController(IConfiguration config)
        {
            _configuration = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromForm] UserProfile login)
        {
            IActionResult result = Unauthorized();

            var user = new MovieDA(_configuration).LoginCheck(login.UserName, login.Password);

            if (user != null)
            {
                var tokenStr = GenerateJSONWebToken(user);
                result = Ok(new { token = tokenStr });
            }

            return result;
        }

        private string GenerateJSONWebToken(UserProfile user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.UserProfilePK.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }
    }
}
