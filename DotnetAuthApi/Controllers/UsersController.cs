using DotnetAuthApi.Dtos;
using DotnetAuthApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DotnetAuthApi.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody]UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User { Email = userDto.Email, UserName = userDto.UserName };
                var result = await _userManager.CreateAsync(newUser, userDto.Password);

                if (result.Succeeded)
                    return Ok();
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(error.Description, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            var foundUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (foundUser == null)
                return NotFound();

            if (await _userManager.CheckPasswordAsync(foundUser, userDto.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("thisisthelongsecret");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString())
                    }),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Id = foundUser.Id, Token = tokenString });
            }

            return BadRequest();
        }
    }
}
