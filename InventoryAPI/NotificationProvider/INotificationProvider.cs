using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using InventoryRepository.Models;

namespace InventoryAPI.NotificationProvider
{
    public interface INotificationProvider
    {
        void SendDeleteNotification(InventoryItem item);
    }
}
