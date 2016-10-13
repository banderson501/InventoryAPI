using System;
using System.Collections.Generic;
using System.Linq;
using InventoryRepository.Models;

namespace InventoryRepository
{
    public class InventoryRepo : IInventoryRepository
    {

        List<InventoryItem> _inventory = new List<InventoryItem>
        {
            new InventoryItem { Label = "claw hammer", Type = "hammer", Expiration = 
                new DateTime(year: 2016, month: 10, day: 12, hour: 9, minute: 46, second: 0)},
            new InventoryItem { Label = "ballpin hammer", Type = "hammer", Expiration = new DateTime(2016, 10, 12, 9, 46, 30)},
            new InventoryItem { Label = "rip saw", Type = "saw", Expiration = new DateTime(2016, 10, 12, 9, 47, 0) },
            new InventoryItem { Label = "crosscut saw", Type = "saw", Expiration = new DateTime(2016, 10, 12, 9, 47, 30)},
            new InventoryItem { Label = "pliers", Type = "pliers", Expiration = new DateTime(2016, 10, 12, 9, 48, 0)},
            new InventoryItem { Label = "needlenose pliers", Type = "pliers", Expiration = new DateTime(2016, 10, 12, 9, 48, 30)},
        };

        public InventoryItem DeleteItem(string label, string type)
        {
            var product = _inventory.FirstOrDefault((p) => p.Label == label && p.Type == type);
            if (product != null)
            {
                _inventory.RemoveAll(((p) => p.Label == label && p.Type == type));
                return product;
            }
            return null;

        }

        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _inventory.ToList();
        }

        public InventoryItem AddItem(InventoryItem newItem)
        {
            if (newItem == null)
            {
                throw new ArgumentNullException(nameof(newItem));
            }

            var product = _inventory.FirstOrDefault((p) => p.Label == newItem.Label && p.Type == newItem.Type);
            if (product != null)
            {
                return null;
            }
            _inventory.Add(newItem);
            return newItem;
        }
    }
}