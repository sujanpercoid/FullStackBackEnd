using FullStack.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Services
{
    public interface IProduct
    {
       Task<Product> GetProduct (int id);
       Task<string> AddProduct (ProductDto prodRequest);
       Task<List<Product>> GetAllProduct();
       Task <List<Product>> MyProd(int id);
       Task <string> Review(Review review);
       Task<List<Review>> GetReview(int id);
       Task<string> Cart(Cart cartItem);
       Task<string> Increase(Cart cartItem);
       Task <string> Decrease (Cart cartItem);
       List<object> SellerDetails(int id);
       Task<object> MyCart (int id);
       Task<Product> UrProdEdit(int id);
       Task<string> DeleteItem(int id);
       Task<string> UpdateItem(int id, Product product);
    }
}
