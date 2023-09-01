using AutoMapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly FullStackDbContext _prod;
        private readonly IMapper _mapper;

        public ProductController(FullStackDbContext prod, IMapper mapper)
        {
            _prod = prod;
            _mapper = mapper;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto prodRequest)
        {
            // Map the DTO to the Product model
            var productModel = _mapper.Map<ProductDto, Product>(prodRequest);

            // Check if the product with the same name already exists in the Products table
            var existingProduct = await _prod.Products.FirstOrDefaultAsync(p => p.ProductId == prodRequest.ProductId);

            if (existingProduct == null)
            {
                // Product doesn't exist, add it to the Products table
                await _prod.Products.AddAsync(productModel);
                await _prod.SaveChangesAsync();
            }
            else
            {
                // Product already exists, use the existing product's ID
                productModel.ProductId = existingProduct.ProductId;
            }

            // Create the UserProduct entry
            var userProductModel = new UserProduct
            {
                ContactId = prodRequest.UserId,
                ProductId = productModel.ProductId
            };

            await _prod.UserProducts.AddAsync(userProductModel);
            await _prod.SaveChangesAsync();

            var response = new { message = "Product Added" };
            return Ok(response);
        }








        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {


            var contactIds = await _prod.Users
                        .Where(user => user.Active == true)
                        .Select(user => user.ContactId)
                        .ToListAsync();

            var userProductIds = await _prod.UserProducts
                                    .Where(userProduct => contactIds.Contains(userProduct.ContactId))
                                    .Select(userProduct => userProduct.ProductId)
                                    .ToListAsync();

            var products = await _prod.Products
                                .Where(product => userProductIds.Contains(product.ProductId))
                                .ToListAsync();




            //var contactid = await _prod.UserProducts
            //       .Select(p => p.ContactId )
            //       .Where(p=>p.ProductId==productIds)
            //       .ToListAsync();




            return Ok(products);
        }



        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _prod.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (product == null)
            {
                return NotFound();

            }
            return Ok(product);
        }

        //Get My Product
        [HttpGet("myprod/{id:int}")]
        public async Task <IActionResult> MyProd([FromRoute] int id)
        {
            var pid = _prod.UserProducts
           .Where(i => i.ContactId == id)
           .Select(i => i.ProductId)
           .ToList();

            var myProducts = _prod.Products
            .Where(i => pid.Contains(i.ProductId))
            .ToList();
           
            return Ok(myProducts);

        }

        //Product review 
        [HttpPost("review")]
        public async Task<IActionResult> Review([FromBody] Review review)
        {

            await _prod.Reviews.AddAsync(review);
            await _prod.SaveChangesAsync();
            return Ok(review);
           
        }
        [HttpGet("review/{id:int}")] // Different route template for review retrieval
        public async Task<IActionResult> GetReview([FromRoute] int id)
        {
            var reviews = _prod.Reviews
                .Where(i => i.ProductId == id)
                .ToList();

            return Ok(reviews);
        }

        

        //item in Cart
        [HttpPost("cart")]
        public async Task<IActionResult> Cart([FromBody] Cart cartItem)
        {
            // First, check if the user exists based on the provided UserId
            var existingUser = await _prod.Carts.FirstOrDefaultAsync(u => u.ContactId == cartItem.ContactId);

            if (existingUser == null)
            {
                await _prod.Carts.AddAsync(cartItem);
                await _prod.SaveChangesAsync();
                var response = new { message = "Item Added To Cart" };
                return Ok(response);
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

                var response = new { message = "Item Added To Cart" };
                return Ok(response);
            }
        }



        //Cart Increase count 
        [HttpPost("inc")]
        public async Task<IActionResult> Increase ([FromBody] Cart cartItem)
        {
            var existingUser = await _prod.Carts.FirstOrDefaultAsync(u => u.ContactId == cartItem.ContactId);
            if (existingUser == null)
            {
                return BadRequest();
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

                var response = new { message = "Count Increased" };
                return Ok(response);

            }

                return Ok();
        }

        //To decrease count in Cart
        [HttpPost("dec")]
        public async Task<IActionResult> Decrease([FromBody] Cart cartItem)
        {
            var existingUser = await _prod.Carts.FirstOrDefaultAsync(u => u.ContactId == cartItem.ContactId);
            if (existingUser == null)
            {
                return BadRequest();
            }
            else
            {
                // Find the cart item with the given product ID and matching ContactId
                var existingCartItem = await _prod.Carts.FirstOrDefaultAsync(c => c.ProductId == cartItem.ProductId && c.ContactId == cartItem.ContactId);

                if (existingCartItem == null)
                {
                    // If the product is not in the cart, return an error
                    return BadRequest();
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
                    return Ok(response);
                }
            }
        }

        
        [HttpGet("seller/{id:int}")]
        public IActionResult SellerDetails(int id)
        {
            var sellerIds = _prod.UserProducts
                                  .Where(p => p.ProductId == id)
                                  .Select(p => p.ContactId);

            var sellerDetails = _prod.Users
                                    .Where(u => sellerIds.Contains(u.ContactId))
                                    .Select(u => new
                                    {
                                        u.Username,
                                        u.fullname,
                                        u.phone,
                                        u.email
                                    })
                                    .ToList();

            return Ok(sellerDetails);
        }








        //Diplsy MyCard and details like price and  totoal price
        [HttpGet("cart/{id:int}")]
        public async Task<IActionResult> MyCart([FromRoute] int id)
        {
            var cartItemsForContact = _prod.Carts.Where(item => item.ContactId == id && item.Active==true).ToList();
            if (cartItemsForContact.Count == 0)
            {
                var emptyCartResponse = new
                {
                    Message = "No item in cart"
                };
                return Ok(emptyCartResponse);
            }

            var productInfo = cartItemsForContact
                      
                       .GroupBy(item => item.ProductId)
                       .Select(group => new
                     {
                       ProductId = group.Key,
                      ProductName = group.First().ProductName,
                      Price = group.First().Price,
                      Count = group.First().Count,
                      Total = group.Sum(item => item.Count * item.Price)
                     })
                    .ToList();


            long grandTotal = productInfo.Sum(item => item.Total);

            var cartResponse = new
            {
                ProductInfo = productInfo.Select(item => new
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Count = item.Count,
                    Total = item.Total
                }),
                GrandTotal = grandTotal
            };

            return Ok(cartResponse);
        }








    }
}
