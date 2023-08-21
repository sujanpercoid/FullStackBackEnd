using System.ComponentModel.DataAnnotations;

namespace FullStack.Api.Models
{
    public class User
    {
        [Key]
        public Guid  Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
