using System;

namespace AddressAPI.Models
{
    public class AddressQuery
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}

