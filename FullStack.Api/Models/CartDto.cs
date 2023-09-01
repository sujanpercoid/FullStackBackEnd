namespace FullStack.Api.Models
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int ContactId { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; } // Add Quantity property
        public decimal ItemTotal { get; set; } // Add ItemTotal property
    }
}
