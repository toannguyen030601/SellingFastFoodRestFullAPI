using ASM_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductClientController : ControllerBase
    {
        private readonly IProductSvc _productSvc;
        public ProductClientController(IProductSvc productSvc)
        {
            _productSvc = productSvc;
        }

        [HttpGet]
        public IActionResult GetAllProduct(string? search)
        {
            try
            {
                var result = _productSvc.GetAll(search);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Lỗi");
            }
        }

        [HttpGet("{categoryId}")]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            var products = _productSvc.GetProductsByCategory(categoryId);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }
    }
}
