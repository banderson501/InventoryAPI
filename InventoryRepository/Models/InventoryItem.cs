using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryRepository.Models
{
    public class InventoryItem
    {
        public string Label { get; set; }
        public DateTime Expiration { get; set; }
        public string Type { get; set; }
    }
}