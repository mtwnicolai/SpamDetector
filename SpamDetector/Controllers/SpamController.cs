using Microsoft.AspNetCore.Mvc;
using SpamDetector.Data;
using SpamDetector.Models;
using SpamDetector.Services;


namespace SpamDetector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpamController : ControllerBase
    {
        private readonly SpamModelService _spamModelService;
        public SpamController(SpamModelService spamModelService)
        {
            _spamModelService = spamModelService;
        }

        [HttpPost("analyze")]
        public ActionResult<SpamResponse> Analyze(SpamRequest request)
        {
            var prediction = _spamModelService.Predict(
                $"{request.Subject} {request.Body}"
            );

            var response = new SpamResponse
            {
                isSpam = prediction.IsSpam,
                probability = prediction.Probability
            };

            return Ok(response);
        }
    }
}