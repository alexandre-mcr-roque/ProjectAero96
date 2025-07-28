using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectAero96.Helpers;
using ProjectAero96.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectAero96.Controllers.API
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper userHelper;
        private readonly IConfiguration configuration;

        public AccountController(IUserHelper userHelper, IConfiguration configuration)
        {
            this.userHelper = userHelper;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("api/account/createtoken")]
        public async Task<IActionResult> CreateToken([FromBody, Bind("Email,Password")]SignInViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid sign in data." });
            }

            var user = await userHelper.FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid sign in data." });
            }

            var result = await userHelper.ValidatePasswordAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Invalid sign in data." });
            }

            var claims = new[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Tokens:Issuer"],
                configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(15),
                signingCredentials: credentials);

            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
            return Created(string.Empty, new { message = "Signed in successfully.", results.token, results.expiration });
        }
    }
}
