using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsParser.Interfaces;
using NewsParser.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewsParser.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private IConfiguration _config;

        public UserController( IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }
        [HttpPost("getToken")]
        [AllowAnonymous]
        public IActionResult GetToken([FromBody] LoginModel loginModel)
        {
            User _user = _userRepository.Login(loginModel.UserName, loginModel.Password);
            _user.Password = null;

            if (_user != null)
            {
                var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _user.Username),
                        new Claim(JwtRegisteredClaimNames.Name, _user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, _user.RoleName)
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(30),
                    signingCredentials: signIn
                );
                var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { Token = encodeToken });
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
        [HttpPost("addUser")]
        [AllowAnonymous]
        public IActionResult AddUser([FromBody] User user)
        {
            _userRepository.AddUser(user);
            return(Ok("User added successfully"));         
        }
    }
}
