using AutoMapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using FullStack.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FullStack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        
        private readonly FullStackDbContext _login;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuth _iauth;

        

        public AuthController(FullStackDbContext login, IMapper mapper,IWebHostEnvironment environment,IAuth iauth)
        {
            _login = login;
            _mapper = mapper;
            _environment = environment;
            _iauth = iauth;
        }

        public static User user = new User();

         [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var reg = await _iauth.Register(request);
            var response = new { message = "User Added" };
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDto request)
        {
            var log = await _iauth.Login(request);
            return Ok(log);
        }

        //  Get for Edit
        [HttpGet("{id:Guid}")]
        
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var user = await _iauth.GetUser(id);
            return Ok(user);
            
        }

        // Edited Profile

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateProfile([FromRoute] Guid id, UserDto request)
        {
           var update = await _iauth.Update(id,request);
            return Ok(update);

        }



        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteProfile([FromRoute] Guid id)
        {
            var del = await _iauth.Delete(id);
            return Ok(del);
        }

       



       

     }
}
