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
        public string fullname { get; set; } = string.Empty;
        public long phone { get; set; }
        public string email { get; set; }
    }
}
