using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/translate")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private readonly ITranslatorService _translatorService;
        public TranslateController(ITranslatorService translatorService)
        {
            _translatorService = translatorService;
        }

        // GET api/health
        [HttpGet]
        [Route("")]
        public async Task<JsonResult> Translate(string message, string source, string destination)
        {
            var responseTranslatedText = _translatorService.TranslateText(message, source, destination);
            return new JsonResult(new
            {
                Destination = responseTranslatedText,
                Source = message,
            });
        }


       
    }
}