using System;
using System.Collections.Generic;

namespace Zuto.Uk.Sample.API.Models.Api
{
    public class JobApiRequestModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string MobileNumber { get; set; }
        public List<string> Disabilities { get; set; }
        public string Message { get; set; }
        public string LanguageRequested { get; set; }
        public DateTime TimestampRequiredFor { get; set; }
        public DateTime TimestampRequested { get; set; }

        public JobsModel Model()
        {
            return new JobsModel
            {
                Name = Name,
                MobileNumber = MobileNumber,
                Location = Location,
                Lat = Lat,
                Long = Long,
                Message = Message,
                TranslatedMessage = Message,
                LanguageRequested = LanguageRequested,
                TimestampRequested = TimestampRequested.ToString(),
                TimestampRequiredFor = TimestampRequiredFor.ToString(),
                Disabilities = Disabilities
            };
        }
    }   
}