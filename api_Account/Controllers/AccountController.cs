using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api_Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginData(string username, string password)
        {
            try
            {
                if (username == "abc" && password == "abc")
                {
                    var token = GenerateToken(username);
                    return Ok(new { Token = token });
                }
                return BadRequest("Invalid Data");
            }
            catch(Exception ex) {
            
                return BadRequest(ex.Message);  
            
            }
        }

        private string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var authClaims = new List<Claim>();
            authClaims.Add(new Claim(ClaimTypes.Name, "user.FirstName + user.LastName"));
            authClaims.Add(new Claim("UserName", "user.FirstName +  + user.LastName"));
            authClaims.Add(new Claim("Email", "user.Email"));
            authClaims.Add(new Claim("UserId", "user.Id"));
            authClaims.Add(new Claim("allowed_routes", "/api/Products/getproductstring"));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
