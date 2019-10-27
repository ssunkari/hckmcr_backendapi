using System.ComponentModel.DataAnnotations;

namespace Zuto.Uk.Sample.API.Models.Api
{
    public class BuddyApiModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Lat { get; set; }
        [Required]
        public string Long { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Profile { get; set; }

        [Required]
        public string Rating { get; set; }
    }
}