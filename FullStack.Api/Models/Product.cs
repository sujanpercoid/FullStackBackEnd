using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStack.Api.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        
        
        public string ProductName { get; set; }
        public long Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
      


    }
}
