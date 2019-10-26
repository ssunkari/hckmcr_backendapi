using System.Collections.Generic;

namespace Zuto.Uk.Sample.API.Models
{
    public class JobsModel  
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string TimestampRequested { get; set; }
        public string TimestampRequiredFor { get; set; }
        public string Message { get; set; }
        public string TranslatedMessage { get; set; }
        public string LanguageRequested { get; set; }
        public List<string> Disabilities { get; set; }
    }
}