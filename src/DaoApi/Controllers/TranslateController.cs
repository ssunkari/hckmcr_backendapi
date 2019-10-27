using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Zuto.Uk.Sample.API.Controllers
{
    [Route("api/translate")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private readonly IHostingEnvironment _env;

        public TranslateController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET api/health
        [HttpGet]
        [Route("")]
        public async Task<JsonResult> Translate(string message, string source, string destination)
        {

            var client = TranslationClient.Create(GoogleCredential.FromJson(System.IO.File.ReadAllText(System.IO.Path.Combine(_env.ContentRootPath, "google-creds.json"))));
            var response = client.TranslateText(message, destination, source, TranslationModel.NeuralMachineTranslation);
            return new JsonResult(new
            {
                Destination = response.TranslatedText,
                Source = message,
            });
        }
    }
}