using System.ComponentModel.DataAnnotations;

namespace FullStack.Api.Models
{
    public class UserProduct
    {
        [Key]
        public int UserProductId { get; set; }

        public int ContactId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

}
