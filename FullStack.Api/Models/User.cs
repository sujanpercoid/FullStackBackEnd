using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStack.Api.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }
        public Guid  Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string fullname { get; set; } = string.Empty;
        public long phone { get; set; }
        public string email { get; set; }
        public ICollection<UserProduct> UserProducts { get; set; }


    }
}
