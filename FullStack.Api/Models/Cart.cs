using System.ComponentModel.DataAnnotations.Schema;

namespace FullStack.Api.Models
{
    public class Cart
    {
        public int CartId { get; set; } // Primary Key

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ContactId { get; set; }
        public long Price { get; set; }
        public long Count { get; set; }
        public bool Active { get; set; }= true;

        // Navigation properties
        public Product Product { get; set; } // Navigation property for Product
        public User User { get; set; } // Navigation property for Contact
    }

}
