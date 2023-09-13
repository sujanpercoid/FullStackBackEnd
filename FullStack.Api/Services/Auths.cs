using AutoMapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FullStack.Api.Services
{
    public class Auths:IAuth
    {
        private readonly FullStackDbContext _login;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public Auths(FullStackDbContext login, IMapper mapper, IWebHostEnvironment environment)
        {
            _login = login;
            _mapper = mapper;
            _environment = environment;
        }
        public static User user = new User();

        // For User Registration 
        public async Task<string> Register(UserDto request)
        {
            if (await _login.Users.AnyAsync(u => u.Username == request.Username))
            {
                var resultMessage = new { message = "User Already Exist !!" };
                return (JsonConvert.SerializeObject(resultMessage));

            }
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = _mapper.Map<User>(request);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.Id = Guid.NewGuid();
            _login.Users.Add(user);
            await _login.SaveChangesAsync();
            var msg = new { message = "User Added" };
            return (JsonConvert.SerializeObject(msg));
        }
        public async Task<object> Login(LoginDto request)
        {
            var user = await _login.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                var msg = new { message = "User Not Found" };
                return (JsonConvert.SerializeObject(msg));
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                var msgg = new { message = "Wrong Password" };
                return (JsonConvert.SerializeObject(msgg));
            }
            var active = user.Active;
            if (active == false)
            {
                var resultMessage = new { message = "User NO Longer Available" };
                return (JsonConvert.SerializeObject(resultMessage));
            }

            string token = CreateToken(user);
            

            // Map User to UserDto before returning the token

            return (new
            {
                Token = token,
                User = new
                {
                    user.Username,
                    user.Id,
                    user.ContactId

                }
            });
        }

        //To get for edit
        public async Task <UserDto> GetUser (Guid id)
        {
            var user = await _login.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                //var resultMessage = new { message = "User Not Found" };
                //return (JsonConvert.SerializeObject(resultMessage));

            }
            // returning from the dto
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }
        //New edited 
        public async Task<string> Update(Guid id, UserDto request)
        {
            var profile = await _login.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (profile == null)
            {
                var msg = new { message = "not found" };
                return (JsonConvert.SerializeObject(msg));

            }
            profile.Username = request.Username;
            profile.email = request.email;
            profile.phone = request.phone;
            profile.fullname = request.fullname;
            
            await _login.SaveChangesAsync();
            var resultMessage = new { message = "Edited" };
            return (JsonConvert.SerializeObject(resultMessage));
        }
        public async Task<string> Delete(Guid id)
        {
            var profile = await _login.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (profile == null)
            {
                var msg = new { message = "Not found" };
                return (JsonConvert.SerializeObject(msg));

            }
            profile.Active = false;
            var contactIds = await _login.Users
                        .Where(u => u.Id == id)
                        .Select(u => u.ContactId)
                        .ToListAsync();
            var prodid = await _login.UserProducts
                        .Where(userProduct => contactIds.Contains(userProduct.ContactId))
                        .Select(userProduct => userProduct.ProductId)
                        .ToListAsync();
            var cartItemsToDeactivate = await _login.Carts
                       .Where(cartItem => prodid.Contains(cartItem.ProductId))
                       .ToListAsync();

            foreach (var cartItem in cartItemsToDeactivate)
            {
                cartItem.Active = false;
            }

            //_login.Users.Remove(profile);
            await _login.SaveChangesAsync();
            var resultMessage = new { message = "User Deleted" };
            return (JsonConvert.SerializeObject(resultMessage));
        }












        // METHODS FOR TOKEN GENERATIONS 
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
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
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
