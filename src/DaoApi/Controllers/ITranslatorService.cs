namespace Zuto.Uk.Sample.API.Controllers
{
    public interface ITranslatorService
    {
        string TranslateText(string message, string source, string destination);
    }
}