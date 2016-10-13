using System.Collections.Generic;

namespace InventoryRepository.Models
{
    public interface IInventoryRepository
    {
        InventoryItem DeleteItem(string label, string type);
        IEnumerable<InventoryItem> GetAllItems();
        InventoryItem AddItem(InventoryItem item);
    }
}
