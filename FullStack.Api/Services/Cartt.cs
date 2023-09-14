using AutoMapper;
using Dapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Services
{
    public class Cartt : ICart
    {
        private readonly FullStackDbContext _prod;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public Cartt(FullStackDbContext prod, IMapper mapper, IConfiguration config)
        {
            _prod = prod;
            _mapper = mapper;
            _config = config;
        }
        //Connection string for sql for whole class
        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_config.GetConnectionString("conn"));
        }
        public async Task<string> Cart(Cart cartItem)
        {
            // First, check if the user exists based on the provided UserId
            var existingUser = await _prod.Carts.FirstOrDefaultAsync(u => u.ContactId == cartItem.ContactId);

            if (existingUser == null)
            {
                await _prod.Carts.AddAsync(cartItem);
                await _prod.SaveChangesAsync();


            }
            else
            {
                // Find the cart item with the given product ID and matching ContactId
                var existingCartItem = await _prod.Carts.FirstOrDefaultAsync(c => c.ProductId == cartItem.ProductId && c.ContactId == cartItem.ContactId);

                if (existingCartItem != null)
                {
                    // If the product is already in the cart, increment the count by 1
                    existingCartItem.Count++;
                }
                else
                {
                    // If the product is not in the cart, add it as a new item
                    await _prod.Carts.AddAsync(cartItem);
                }

                await _prod.SaveChangesAsync();


            }
            return " Item Added To Cart";
        }

        // Cart Item Count increase
        public async Task<string> Increase(Cart cartItem)
        {
            var existingUser = await _prod.Carts.FirstOrDefaultAsync(u => u.ContactId == cartItem.ContactId);
            if (existingUser == null)
            {

            }
            else
            {
                // Find the cart item with the given product ID and matching ContactId
                var existingCartItem = await _prod.Carts.FirstOrDefaultAsync(c => c.ProductId == cartItem.ProductId && c.ContactId == cartItem.ContactId);

                if (existingCartItem != null)
                {
                    // If the product is already in the cart, increment the count by 1
                    existingCartItem.Count++;
                }
                else
                {
                    // If the product is not in the cart, add it as a new item
                    await _prod.Carts.AddAsync(cartItem);
                }

                await _prod.SaveChangesAsync();
            }
            return "count increase";

        }

        // To Decrease Count 
        public async Task<string> Decrease(Cart cartItem)
        {
            var existingUser = await _prod.Carts.FirstOrDefaultAsync(u => u.ContactId == cartItem.ContactId);
            if (existingUser == null)
            {

            }
            else
            {
                // Find the cart item with the given product ID and matching ContactId
                var existingCartItem = await _prod.Carts.FirstOrDefaultAsync(c => c.ProductId == cartItem.ProductId && c.ContactId == cartItem.ContactId);

                if (existingCartItem == null)
                {
                    // If the product is not in the cart, return an error

                }
                else
                {
                    // If the product is already in the cart, decrement the count by 1
                    existingCartItem.Count--;

                    // If count becomes 0, you might want to remove the item from the cart
                    if (existingCartItem.Count <= 0)
                    {
                        _prod.Carts.Remove(existingCartItem);
                    }

                    await _prod.SaveChangesAsync();

                    var response = new { message = "Count Decreased" };

                }
            }
            return "ok";
        }

        // Get Cart Total
        public async Task<List<CartDto>> MyCart(int id)
        {

            var connection = CreateConnection();
            var sql = @"  
              select productid,productname,price,count,(price*count) as total
              from Carts
              where active = 1 and ContactId =@ContactId;
              ";
            var parameter = new { ContactId = id };
            var cartResonse = await connection.QueryAsync<CartDto>(sql, parameter);
            //var grandtotal = await connection.QueryAsync<CartDto>(@"
            //  select sum(price*count) as grand total
            //  from Carts
            //  where active = 1 and ContactId =@ContactId;");


            return cartResonse.ToList();
        }
    }
}
