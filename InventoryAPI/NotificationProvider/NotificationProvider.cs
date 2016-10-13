using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InventoryRepository.Models;

namespace InventoryAPI.NotificationProvider
{
    public class NotificationProvider : INotificationProvider
    {
        public void SendDeleteNotification(InventoryItem item)
        {
            // In a production system this would put a message into a message bus for subscribers to catch.
            // One subscriber would do its appropriate notification when this message comes across the bus.

            // Here we will just write out to the console to show this happened.
            System.Diagnostics.Debug.WriteLine(String.Format("Pulling inventory item: Label: {0}, Type: {1}, Expiration: {2}", item.Label, item.Type, item.Expiration));
//            Console.WriteLine(String.Format("Pulling inventory item: Label: {0}, Type: {1}, Expiration: {2}", item.Label, item.Type, item.Expiration));
        }
    }
}