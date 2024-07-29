using ASM_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryClientController : ControllerBase
    {
        private readonly ICategorySvc _categorySvc;
        public CategoryClientController(ICategorySvc categorySvc)
        {
            _categorySvc = categorySvc;
        }

        [HttpGet]
        public IActionResult GetAllCategory()
        {
            try
            {
                var result = _categorySvc.GetAll();
                return Ok(result);
            }
            catch
            {
                return BadRequest("Lỗi");
            }
        }
    }
}
