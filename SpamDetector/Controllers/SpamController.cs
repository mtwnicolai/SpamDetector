using Microsoft.AspNetCore.Mvc;
using SpamDetector.Models;

namespace SpamDetector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpamController : ControllerBase
    {
        [HttpPost("analyze")]
        public ActionResult<SpamResponse> Analyze(SpamRequest request)
        {
            var response = new SpamResponse();
            {
                bool IsSpam = false;
                double Probability = 0.0;
            };
            return Ok(response);
        }
    }
}
