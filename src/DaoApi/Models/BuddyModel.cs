using System;
using Zuto.Uk.Sample.API.Models.Api;

namespace Zuto.Uk.Sample.API.Models
{
    public class BuddyModel
    {
        public BuddyModel(BuddyApiModel model)
        {
            FirstName = model.FirstName;
            LastName = model.LastName;
            Long = model.Long;
            Lat = model.Lat;
            Location = model.Location;
            MobileNumber = model.MobileNumber;
            Id = Guid.NewGuid().ToString();

        }

        public BuddyModel()
        {
            
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Id { get; set; }
        public string MobileNumber { get; set; }
    }
}