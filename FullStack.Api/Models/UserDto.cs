using System.ComponentModel.DataAnnotations;

namespace FullStack.Api.Models
{
    public class UserDto
    {
        
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string fullname { get; set; } = string.Empty;
        public long phone { get; set; }
       public string email { get; set; }
    }
}
