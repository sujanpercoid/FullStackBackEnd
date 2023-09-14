namespace FullStack.Api.Models
{
    public class CartDto
    {
       
        public int ProductId { get; set; }
        
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public float Total { get; set; }
        public float GrandTotal { get; set; }
    }
}
