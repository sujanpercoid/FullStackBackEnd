using AutoMapper;
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

        //  WithOut AutoMapper
        //[HttpPost("register")]
        //public async Task<ActionResult<User>> Register(UserDto request)
        //{
        //    if (user.Username == request.Username)
        //            {
        //               return BadRequest("User Already Exist");
        //            }
        //        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        //    user.Username = request.Username;
        //    user.PasswordHash = passwordHash;
        //    user.PasswordSalt = passwordSalt;

        //    return Ok(user);
        //}

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


        //[HttpPost("login")]
        //public async Task<ActionResult<string>> Login(UserDto request)
        //{
        //    if (user.Username != request.Username)
        //   {
        //        return BadRequest("User Not Found");
        //   }

        //    if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        //   {
        //       return BadRequest("Wrong Password");
        //    }

        //   string token = CreateToken(user);
        //    return Ok(token);
        //}
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
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
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(new { Token = token, User = userDto });
        }



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
