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
    public class ProductController : Controller
    {
        private readonly IProduct _producti;
        private readonly FullStackDbContext _prod;
        private readonly IMapper _mapper;

        public ProductController(FullStackDbContext prod, IMapper mapper, IProduct producti)
        {
            _prod = prod;
            _mapper = mapper;
            _producti = producti;
        }

        //To Add New Product
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto prodRequest)
        {

            // Call the AddProduct method from your repository
            var addedProduct = await _producti.AddProduct(prodRequest);

            // You can return a response based on the result of the operation
            if (addedProduct != null)
            {
                var response = new { message = "Product Added" };
                return Ok(response); // Return the added product as a response
            }
            else
            {
                return BadRequest("Failed to add the product."); // Handle the failure case
            }
        }

        //Get All Products
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _producti.GetAllProduct();
            return Ok(products);
        }



        //Get Single Product for Testing
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _producti.GetProduct(id);
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound();
            }


        }

        //Get My Product
        [HttpGet("myprod/{id:int}")]
        public async Task<IActionResult> MyProd([FromRoute] int id)
        {

            var myProducts = await _producti.MyProd(id);
            return Ok(myProducts);

        }

        //Product review 
        [HttpPost("review")]
        public async Task<IActionResult> Review([FromBody] Review review)
        {
            var reviews = await _producti.Review(review);
            return Ok();

        }

        // Get review 
        [HttpGet("review/{id:int}")] // Different route template for review retrieval
        public async Task<IActionResult> GetReview([FromRoute] int id)
        {
            var reviews = await _producti.GetReview(id);
            return Ok(reviews);
        }



        //item in Cart
        [HttpPost("cart")]
        public async Task<IActionResult> Cart([FromBody] Cart cartItem)
        {
            var cart = await _producti.Cart(cartItem);
            return Ok();
            
        }



        //Cart Increase count 
        [HttpPost("inc")]
        public async Task<IActionResult> Increase([FromBody] Cart cartItem)
        {
            var count = await _producti.Increase(cartItem); 
             return Ok(); 
        }

        //To decrease count in Cart
        [HttpPost("dec")]
        public async Task<IActionResult> Decrease([FromBody] Cart cartItem)
        {
            var dec = await _producti.Decrease(cartItem);
            return Ok();
            
        }

        //Get Seller Details
        [HttpGet("seller/{id:int}")]
        public IActionResult SellerDetails(int id)
        {
            var sd = _producti.SellerDetails(id);
            return Ok(sd);
        }

        //Diplsy MyCard and details like price and  totoal price
        [HttpGet("cart/{id:int}")]
        public async Task<IActionResult> MyCart([FromRoute] int id)
        {
            var total = await _producti.MyCart(id);
            return Ok(total);
        }

        //Get Product For Edit
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> UrProdEdit([FromRoute] int id)
        {
            var product = await _producti.UrProdEdit(id);
            if (product == null)
            {
                return NotFound();

            }
            return Ok(product);
        }


        //Delete the item
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteItem([FromRoute] int id)
        {
            var product = await _producti.DeleteItem(id);
            
            var msg = new
            {
                Message = "Item Deleted"
            };

            return Ok(msg);
        }

        //Update your Item
        [HttpPut("edit/{id:int}")]
        public async Task<IActionResult> UpdateItem([FromRoute] int id, Product product)
        {
            var update = await _producti.UpdateItem(id, product);
           var msg = new
            {
                Message = "Item Updated"
            };
            return Ok(msg);
        }

        //Get review for edit
        [HttpGet("reviewedit/{id:int}")] // Different route template for review retrieval
        public async Task<IActionResult> GetReviewForEdit([FromRoute] int id)
        {
            var reviews = _prod.Reviews
                .Where(i => i.ReviewId == id)
                .ToList();

            return Ok(reviews);
        }








    }
}
