using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace RareCrewCSharpTest.Controllers
{
    [Route("api/timeentries")]
    [ApiController]
    public class TimeEntriesController : ControllerBase
    {
        private readonly ITimeEntryService _timeEntryService;

        public TimeEntriesController(ITimeEntryService timeEntryService)
        {
            _timeEntryService = timeEntryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeEntriesAsync()
        {
            var result = await _timeEntryService.GetTimeEntriesAsync();
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error); 
        }
    }
}
