using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Hosting;

namespace Zuto.Uk.Sample.API.Controllers
{
    public class TranslatorService : ITranslatorService
    {
        private readonly IHostingEnvironment _env;

        public TranslatorService(IHostingEnvironment env)
        {
            _env = env;
        }

        public string TranslateText(string message, string source, string destination)
        {
            var client =
                TranslationClient.Create(GoogleCredential.FromJson(
                    System.IO.File.ReadAllText(System.IO.Path.Combine(_env.ContentRootPath, "google-creds.json"))));
            var response = client.TranslateText(message, destination, source, TranslationModel.NeuralMachineTranslation);
            var responseTranslatedText = response.TranslatedText;
            return responseTranslatedText;
        }
    }
}