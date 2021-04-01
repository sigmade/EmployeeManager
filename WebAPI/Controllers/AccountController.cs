using DomainLayer;
using DomainLayer.Dtos;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Authentication;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private EmployeesDBContext _context;
        public AccountController(EmployeesDBContext context)
        {
            _context = context;
        }

        //TDOD Убрать
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);
                if (user != null)
                {
                    return Ok("Авторизован"); 
                }
            }
            return Forbid();
        }
        // JWT with claim
        [HttpPost("/token")]
        public IActionResult Token([FromBody] LoginDto loginDto)
        {
            var email = loginDto.Email;
            var password = loginDto.Password;
            var identity = GetIdentity(email, password);
            if (identity is null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            User user =  _context.Users.Include(u => u.Role)
                .FirstOrDefault(x => x.Email == email && x.Password == password);
            var role = user.Role.Name;

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }

        [HttpGet("/role")]
        public ActionResult<string> GetRole()
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value;
            return role;
        }
    }
}
