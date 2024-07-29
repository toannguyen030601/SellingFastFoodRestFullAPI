using ASM_api.DB;
using ASM_api.Models;

namespace ASM_Client.Models
{
    public class ProductModel
    {
        private net105AsmDBContext context;

        public ProductModel(net105AsmDBContext context)
        {
            this.context = context;
        }

        public List<Product> FindAll()
        {
            var a = context.Products.ToList();
            return a;
        }

        public Product Find(int id)
        {
            return context.Products.Find(id);
        }
    }
}
