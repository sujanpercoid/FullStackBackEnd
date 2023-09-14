using AutoMapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using FullStack.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly FullStackDbContext _prod;
        private readonly IMapper _mapper;
        private readonly ICart _cart;

        public CartController(FullStackDbContext prod, IMapper mapper, ICart cart)
        {
            _prod = prod;
            _mapper = mapper;
            _cart = cart;
        }

        //Put item in Cart
        [HttpPost("cart")]
        public async Task<IActionResult> Cart([FromBody] Cart cartItem)
        {
            var cart = await _cart.Cart(cartItem);
            return Ok();

        }

        //Cart Increase count 
        [HttpPost("inc")]
        public async Task<IActionResult> Increase([FromBody] Cart cartItem)
        {
            var count = await _cart.Increase(cartItem);
            return Ok();
        }

        //To decrease count in Cart
        [HttpPost("dec")]
        public async Task<IActionResult> Decrease([FromBody] Cart cartItem)
        {
            var dec = await _cart.Decrease(cartItem);
            return Ok();

        }
        //Diplsy MyCart and details like price and  totoal price
        [HttpGet("cart/{id:int}")]
        public async Task<IActionResult> MyCart([FromRoute] int id)
        {
            var total = await _cart.MyCart(id);
            return Ok(total);
        }
    }
}
