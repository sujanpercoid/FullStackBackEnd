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
            var products = await _prod.Products.ToListAsync();
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



    }
}
