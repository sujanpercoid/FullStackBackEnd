using AutoMapper;
using Dapper;
using FullStack.Api.Data;
using FullStack.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Services
{
    public class Prod : IProduct
    {
        private readonly FullStackDbContext _prod;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private List<Product> Products; // You should use the 'Product' class here
        public Prod(FullStackDbContext prod, IMapper mapper,IConfiguration config)
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

        //To get single prod info
        public async Task<List<Product>> GetProduct(int id)
        {

            //var product = await _prod.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            using var connection = CreateConnection();
            var sqlQuery = "SELECT * FROM products WHERE ProductId = @ProductId";
            var parameters = new { ProductId = id };
            var products = await connection.QueryAsync<Product>(sqlQuery, parameters);

            return products.ToList();
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
            //var contactIds = await _prod.Users
            //           .Where(user => user.Active == true)
            //           .Select(user => user.ContactId)
            //           .ToListAsync();

            //var userProductIds = await _prod.UserProducts
            //                        .Where(userProduct => contactIds.Contains(userProduct.ContactId))
            //                        .Select(userProduct => userProduct.ProductId)
            //                        .ToListAsync();

            //var products = await _prod.Products
            //                    .Where(product => userProductIds.Contains(product.ProductId))
            //                    .ToListAsync();
            using var connection = CreateConnection();
            var products = await connection.QueryAsync<Product>
                ( @"select 
                  p.ProductId, p.ProductName, p.Price, P.Category, p.Description
                 from Products p
                 inner join UserProducts up on p.ProductId = up.ProductId
                 join Users u on u.ContactId = up.ContactId
                  where u.Active = 1; "
                );
                return products.ToList();
           }
        //Get My Product
        public async Task<IEnumerable<Product>> MyProd(int id)
        {
            // var pid = _prod.UserProducts
            //.Where(i => i.ContactId == id)
            //.Select(i => i.ProductId)
            //.ToList();

            // var myProducts = _prod.Products
            // .Where(i => pid.Contains(i.ProductId))
            // .ToList();
            // return myProducts;
            using var connection = CreateConnection();
            var sql = @" select p.ProductId,
                        p.ProductName,p.Price,p.Category,p.Description
                        from products p
                        inner join userproducts u on p.ProductId = u.ProductId
                        where u.ContactId = @ContactId";
            var parameter = new { ContactId = id };
            var prod = await connection.QueryAsync<Product>(sql, parameter);
            return prod;
            
        }
        // Product Review
        public async Task<string> Review(Review review)
        {
            await _prod.Reviews.AddAsync(review);
            await _prod.SaveChangesAsync();
            return " Review Added";
        }

        // Get Reviews of indivisual products
        public async Task<IEnumerable<Review>> GetReview(int id)
        {
            //var reviews = _prod.Reviews
            //   .Where(i => i.ProductId == id)
            //   .ToList();
            using var connection = CreateConnection();
            var sqlQuery = "SELECT * FROM reviews WHERE ProductId = @ProductId";
            var parameters = new { ProductId = id };
            var review = await connection.QueryAsync<Review>(sqlQuery, parameters);
            return review;
        }





        //Get Seller Details
        public async Task <List<SellerDto>> SellerDetails(int id)
        {
            //var sellerIds = _prod.UserProducts
            //                      .Where(p => p.ProductId == id)
            //                      .Select(p => p.ContactId);

            //var sellerDetails = _prod.Users
            //                        .Where(u => sellerIds.Contains(u.ContactId))
            //                        .Select(u => new
            //                        {
            //                            u.Username,
            //                            u.fullname,
            //                            u.phone,
            //                            u.email
            //                        })
            //                        .ToList();
            //return sellerDetails.Select(x => (object)x).ToList(); // we cant directly pass it either we need to convert it to object or create a model ani tesmai halne ho
            using var connection = CreateConnection();
            var sql = @"select u.Username,u.Fullname,u.Phone,u.Email
                       from users u
                       inner join userproducts up on u.ContactId = up.ContactId
                       where up.ProductId = @ProductId";
            var parameter = new { ProductId = id };
            var sellers = await connection.QueryAsync<SellerDto>(sql, parameter);
            return sellers.ToList();

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

