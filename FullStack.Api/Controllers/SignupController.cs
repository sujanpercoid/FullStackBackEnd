using FullStack.Api.Data;
using FullStack.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignupController : Controller
    {
        private readonly FullStackDbContext _fullStackDbContext;

        public SignupController(FullStackDbContext fullStackDbContext)
        {
            _fullStackDbContext = fullStackDbContext;
        }
        
        [HttpPost]
        public async Task <IActionResult> Singup([FromBody] Signup signupRequest)
        {
            signupRequest.Id = Guid.NewGuid();
            await _fullStackDbContext.Signups.AddAsync(signupRequest);
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(signupRequest);
        } 
    }
}
