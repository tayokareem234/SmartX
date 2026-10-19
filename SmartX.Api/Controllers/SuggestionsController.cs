using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuggestionsController : ControllerBase
    {
        private readonly PredictiveSuggestionService _suggestionService;

        public SuggestionsController(
            PredictiveSuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpGet]
        public ActionResult<List<PredictiveSuggestion>> GetSuggestions()
        {
            List<PredictiveSuggestion> suggestions =
                _suggestionService.GenerateSuggestions();

            return Ok(suggestions);
        }
    }
}