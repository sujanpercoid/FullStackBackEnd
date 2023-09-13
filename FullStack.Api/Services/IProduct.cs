using FullStack.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Services
{
    public interface IProduct
    {
       Task <List<Product>> GetProduct (int id);
       Task<string> AddProduct (ProductDto prodRequest);
       Task<List<Product>> GetAllProduct();
       Task <IEnumerable<Product>> MyProd(int id);
       Task <string> Review(Review review);
       Task<IEnumerable<Review>> GetReview(int id);
       Task<string> Cart(Cart cartItem);
       Task<string> Increase(Cart cartItem);
       Task <string> Decrease (Cart cartItem);
       Task <List <SellerDto>> SellerDetails(int id);
       Task<object> MyCart (int id);
       Task<Product> UrProdEdit(int id);
       Task<string> DeleteItem(int id);
       Task<string> UpdateItem(int id, Product product);
    }
}
