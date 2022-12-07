using Microsoft.AspNetCore.Mvc;
using TVMazeCache.Domain.UseCases;
using TVMazeCache.WebApi.V1.Models;

namespace TVMazeCache.WebApi.V1.Controllers
{
    [Route("shows")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly RetrieveShowsWithCastUseCase _useCase;
        public ShowsController(RetrieveShowsWithCastUseCase useCase) 
        {
            _useCase = useCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            if(page < 1)
                return BadRequest("Page cannot be less than 1");

            var shows = await _useCase.Execute(page);
            return Ok(shows.Select(s => ShowDto.FromDomain(s)));
        }
    }
}
