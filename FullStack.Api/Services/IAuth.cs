using FullStack.Api.Models;

namespace FullStack.Api.Services
{
    public interface IAuth
    {
        Task<string> Register(UserDto request);
        Task<object> Login(LoginDto request);
        Task<UserDto> GetUser(Guid id);
        Task<string> Update(Guid id, UserDto request);
        Task<string> Delete (Guid id);
    }
        
}
