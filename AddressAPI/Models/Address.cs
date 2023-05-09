using System;
using System.ComponentModel.DataAnnotations;

namespace AddressAPI.Models
{
    public class Address
    {
        public Address()
        {
            StreetName = "";
            HouseNumber = "";
            ZipCode = "";
            City = "";
            Country = "";
        }

        public int Id { get; set; }
        [Required]
        public string StreetName { get; set; }
        [Required]
        public string HouseNumber { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }


    }



}

