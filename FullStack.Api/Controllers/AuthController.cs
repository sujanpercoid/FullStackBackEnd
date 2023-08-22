﻿using AutoMapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
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

        

        public AuthController(FullStackDbContext login, IMapper mapper)
        {
            _login = login;
            _mapper = mapper;
        }

        public static User user = new User();

         [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            if (await _login.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("User Already Exists");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = _mapper.Map<User>(request);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _login.Users.Add(user);
            await _login.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var user = await _login.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return BadRequest("User Not Found");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(user);

            // Map User to UserDto before returning the token
            
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Username,
                    user.Id
                    
                }
            });
        }
        [HttpGet("{id:Guid}")]
        
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var user = await _login.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();

            }
            // returning from the dto
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
            //returing from main database
            //return Ok(employee);
        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateProfile([FromRoute] Guid id, UserDto request)
        {
            var profile = await _login.Users.FindAsync(id);
            if (profile == null)
            {
                return NotFound();

            }
            profile.Username = request.Username;
            profile.email = request.email;
            profile.phone = request.phone;
            profile.fullname = request.fullname;
            
            await _login.SaveChangesAsync();
            return Ok(profile);

        }



        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteProfile([FromRoute] Guid id)
        {
            var profile = await _login.Users.FindAsync(id);
            if (profile == null)
            {
                return NotFound();

            }
            _login.Users.Remove(profile);
            await _login.SaveChangesAsync();
            return Ok(profile);


        }



        //Methods
        private string CreateToken(User user)
        {
            var key = GenerateSecurityKey();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private SymmetricSecurityKey GenerateSecurityKey()
        {
            byte[] keyBytes = new byte[128];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            return new SymmetricSecurityKey(keyBytes);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
