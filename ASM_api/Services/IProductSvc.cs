using ASM_api.Models;

namespace ASM_api.Services
{
    public interface IProductSvc
    {
        List<Product> GetAll(string search);
        IEnumerable<Product> GetProductsByCategory(int categoryId);
    }
}
