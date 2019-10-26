using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Mvc;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/translate")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        // GET api/health
        [HttpGet]
        [Route("")]
        public async Task<JsonResult> Translate(string message, string source, string destination)
        {
            var client = TranslationClient.Create(GoogleCredential.FromJson(System.IO.File.ReadAllText("google-creds.json")));
            var response = client.TranslateText(message, destination, source, TranslationModel.NeuralMachineTranslation);
            return new JsonResult(new
            {
                Destination = response.TranslatedText,
                Source = message,
            });
        }
    }
}