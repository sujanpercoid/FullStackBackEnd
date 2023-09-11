using AutoMapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Services
{
    public class Prod : IProduct
    {
        private readonly FullStackDbContext _prod;
        private readonly IMapper _mapper;
        private List<Product> Products; // You should use the 'Product' class here
        public Prod(FullStackDbContext prod, IMapper mapper)
        {
            _prod = prod;
            _mapper = mapper;
        }

        //To get single prod info
        public async Task<Product> GetProduct(int id)
        {
            var product = await _prod.Products.FirstOrDefaultAsync(x => x.ProductId == id);

            return product;
        }

        //To add Item
        public async Task<string> AddProduct(ProductDto prodRequest)
        {
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


            return "Product Added";

        }

        //Get All Products
        public async Task<List<Product>> GetAllProduct()
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
            return products;
        }
        //Get My Product
        public async Task<List<Product>> MyProd(int id)
        {
            var pid = _prod.UserProducts
           .Where(i => i.ContactId == id)
           .Select(i => i.ProductId)
           .ToList();

            var myProducts = _prod.Products
            .Where(i => pid.Contains(i.ProductId))
            .ToList();

            return myProducts;
        }
        // Product Review
        public async Task<string> Review(Review review)
        {
            await _prod.Reviews.AddAsync(review);
            await _prod.SaveChangesAsync();
            return " Review Added";
        }

        // Get Reviews of indivisual products
        public async Task<List<Review>> GetReview(int id)
        {
            var reviews = _prod.Reviews
               .Where(i => i.ProductId == id)
               .ToList();
            return reviews;
        }
        //Add To Cart
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

        //Get Seller Details
        public List<object> SellerDetails(int id)
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
            return sellerDetails.Select(x => (object)x).ToList(); // we cant directly pass it either we need to convert it to object or create a model ani tesmai halne ho
        }
        // Get Cart Total
        public async Task<object> MyCart (int id)
        {
            var cartItemsForContact = _prod.Carts.Where(item => item.ContactId == id && item.Active == true).ToList();
            if (cartItemsForContact.Count == 0)
            {
                
              
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
            return cartResponse;
        }
        //Get Product info for edit 
        public async Task<Product> UrProdEdit( int id)
        {
            var product = await _prod.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            return product;
        }
        //Delete Your Item
        public async Task<string> DeleteItem(int id)
        {
            var product = await _prod.Products.FindAsync(id);
            if (product == null)
            {
                return "Not Found";

            }
            _prod.Products.Remove(product);
            await _prod.SaveChangesAsync();
            return "Item Deleted";
        }
       //Update Your Item
       public async Task<string> UpdateItem(int id, Product product)
        {
            var update = await _prod.Products.FindAsync(id);
            if (product == null)
            {
                return " Not Found";   
            }
            update.ProductName = product.ProductName;
            update.Price = product.Price;
            update.Category = product.Category;
            update.Description = product.Description;
            await _prod.SaveChangesAsync();
            return "Updated";
        }
    }
}

