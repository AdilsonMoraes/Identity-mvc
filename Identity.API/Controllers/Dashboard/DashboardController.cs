using Identity.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers.Dashboard
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DashboardController : Controller
    {
        [ExtendedAuthorize("Administrador")]
        [HttpGet]
        public string GetTest()
        {
            return "teste";
        }

        [ExtendedAuthorize("Developer")]
        [HttpGet]
        public string GetTest2()
        {
            return "teste";
        }

        [ExtendedAuthorize("QA")]
        [HttpGet]
        public string GetTest3()
        {
            return "teste";
        }
    }
}
