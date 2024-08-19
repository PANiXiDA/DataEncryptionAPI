using Microsoft.AspNetCore.Mvc;

namespace DataEncryption.Controllers
{
    [ApiController]
    [Route("api/error")]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError()
        {
            return Problem("An unexpected error occurred.");
        }
    }
}
