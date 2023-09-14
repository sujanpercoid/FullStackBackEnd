using FullStack.Api.Models;

namespace FullStack.Api.Services
{
    public interface ICart
    {
        Task<string> Cart(Cart cartItem);
        Task<string> Increase(Cart cartItem);
        Task<string> Decrease(Cart cartItem);
        Task<List<CartDto>> MyCart(int id);
    }
}
