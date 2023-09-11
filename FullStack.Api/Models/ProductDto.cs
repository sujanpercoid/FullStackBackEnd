using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStack.Api.Models
{
    public class ProductDto
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        public int UserId { get; set; }
        public string ProductName { get; set; }
        public long Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        //public int count { get; set; }
        //public ICollection<UserProduct> UserProducts { get; set; }
    }
}
