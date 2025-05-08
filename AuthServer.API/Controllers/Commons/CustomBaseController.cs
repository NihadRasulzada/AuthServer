using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;

namespace AuthServer.API.Controllers.Commons
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult ActionResultInstance<T>(Response<T> response)
            where T : class => new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
    }
}
