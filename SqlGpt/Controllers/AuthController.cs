using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SqlGpt.Dto;
using SqlGpt.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SqlGpt.Services.Interfaces;

namespace SqlGpt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private IJwtService _jwtService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            AppUser user = new AppUser() 
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email

            };
            var result = await _userManager.CreateAsync(user,registerRequestDto.Password);
            if (!result.Succeeded) 
            { return BadRequest("Try again"); }
            return  Ok("Registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto) 
        {
            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            AppUser? getUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            if (getUser == null) {
                return Unauthorized("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                    getUser,
                  loginRequestDto.Password,
                lockoutOnFailure: false);

            if (!result.Succeeded) 
            { 
                return Unauthorized("Invalid email or password"); 
            }
             

            /// return Ok("Login successful"); old return without da add JWT
            string getGeneratedToken = _jwtService.GenerateToken(getUser);
            return Ok(new { getGeneratedToken });

        }

    }
}
