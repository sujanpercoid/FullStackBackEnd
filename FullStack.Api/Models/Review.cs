using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStack.Api.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public string Reviews { get; set; }
        public string Username { get; set; }
        public virtual Product Product { get; set; }
    }
}
