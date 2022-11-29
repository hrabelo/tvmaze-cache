using Microsoft.AspNetCore.Mvc;

namespace TVMazeCache.WebApi.V1.Controllers
{
    [Route("shows")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        public ShowsController() { }

        [HttpGet]
        public async Task Get()
        {
            await Task.CompletedTask;
        }
    }
}
