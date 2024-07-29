using ASM_api.DB;
using ASM_api.Models;

namespace ASM_api.Services
{
    public class CategorySvc : ICategorySvc
    {
        private readonly net105AsmDBContext _context;
        public CategorySvc(net105AsmDBContext context)
        {
            _context = context;
        }

        public List<Category> GetAll()
        {
            var allCategory = _context.Categories.ToList();

            return allCategory;
        }
    }
}
