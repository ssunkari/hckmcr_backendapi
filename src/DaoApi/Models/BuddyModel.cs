using System;
using Amazon.DynamoDBv2.Model;
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
            Profile = model.Profile;
            Rating = model.Rating;
            ImageUrl = model.ImageUrl;
        }

        public string Profile { get; set; }

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
        public string Rating { get; set; }
        public string ImageUrl { get; set; }
    }
}