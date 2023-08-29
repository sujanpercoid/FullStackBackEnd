using System.ComponentModel.DataAnnotations.Schema;

namespace FullStack.Api.Models
{
    public class Cart
    {
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public int ContactId { get; set; }

    }
}
