using ASM_api.DB;
using ASM_api.Models;

namespace ASM_api.Services
{
    public class ProductSvc : IProductSvc
    {
        private readonly net105AsmDBContext _context;
        public ProductSvc(net105AsmDBContext context)
        {
            _context = context;
        }

        public List<Product> GetAll(string search)
        {
            var allproduct = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                allproduct = allproduct.Where(p => p.ProductName.Contains(search));
            }

            var result = allproduct.Select(p => new Product
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Availability = p.Availability,
                Price = p.Price,
                Description = p.Description,
                CategoryID = p.CategoryID,
                SupplierID = p.SupplierID,
                Hinhanh = p.Hinhanh
            });

            return result.ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products.Where(p => p.CategoryID == categoryId).ToList();
        }
    }
}
