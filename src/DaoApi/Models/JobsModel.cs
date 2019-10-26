using System;
using System.Collections.Generic;

namespace Zuto.Uk.Sample.API.Models
{
    public class JobsModel  
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public DateTime TimestampRequested { get; set; }
        public DateTime TimestampRequiredFor { get; set; }
        public string Message { get; set; }
        public string TranslatedMessage { get; set; }
        public string LanguageRequested { get; set; }
        public List<string> Disabilities { get; set; }
    }
}