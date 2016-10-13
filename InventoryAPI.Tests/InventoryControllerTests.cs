using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using InventoryAPI.Controllers;
using InventoryAPI.NotificationProvider;
using InventoryRepository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InventoryAPI.Tests
{
    /// <summary>
    /// Summary description for InventoryControllerTests
    /// </summary>
    [TestClass]
    public class InventoryControllerTests
    {
        private InventoryController _inventoryController;

        List<InventoryItem> _inventory = new List<InventoryItem>
        {
            new InventoryItem { Label = "claw hammer", Type = "hammer", Expiration = new DateTime(2016, 10, 12, 9, 46, 0)},
            new InventoryItem { Label = "ballpin hammer", Type = "hammer", Expiration = new DateTime(2016, 10, 12, 9, 46, 30)},
            new InventoryItem { Label = "rip saw", Type = "saw", Expiration = new DateTime(2016, 10, 12, 9, 47, 0) },
            new InventoryItem { Label = "crosscut saw", Type = "saw", Expiration = new DateTime(2016, 10, 12, 9, 47, 30)},
            new InventoryItem { Label = "pliers", Type = "pliers", Expiration = new DateTime(2016, 10, 12, 9, 48, 0)},
            new InventoryItem { Label = "needlenose pliers", Type = "pliers", Expiration = new DateTime(2016, 10, 12, 9, 48, 30)},
        };


        public InventoryControllerTests()
        {
            // Use this constructor to setup a base mocked configuration that should work for a number of different tests.
            // Tests which need something different will do their own setup.

            // Setup mock InventoryRepository.
            var inventoryRepository = new Mock<IInventoryRepository>();
            inventoryRepository.Setup(x => x.AddItem(It.IsAny<InventoryItem>()))
                .Returns(new InventoryItem());

            inventoryRepository.Setup(x => x.DeleteItem(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new InventoryItem());

            inventoryRepository.Setup(x => x.GetAllItems()).Returns(_inventory.ToList());

            // Setup mock NotificationProvider.
            var notificationProvider = new Mock<INotificationProvider>();
            notificationProvider.Setup(x => x.SendDeleteNotification(It.IsAny<InventoryItem>()));

            SetupInventoryController(inventoryRepository.Object, notificationProvider.Object);
        }

        [TestMethod]
        public void TestDeleteSuccess()
        {
            var httpResponse = _inventoryController.Delete("claw hammer", "hammer");
            Assert.IsTrue(httpResponse.IsSuccessStatusCode);
        }

        [TestMethod]
        public void TestDeleteNullRepository()
        {
            _inventoryController = new InventoryController(null, null);
            _inventoryController.Request = new HttpRequestMessage();
            _inventoryController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var httpResponse = _inventoryController.Delete("claw hammer", "hammer");
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.InternalServerError);

            // TODO: Verify something more specific than just StatusCode == HttpStatusCode.InternalServerError.
        }

        // TODO: Add a test for a null Notifier.

        [TestMethod]
        public void TestDeleteNotFoundLabelTypeMismatch()
        {
            var repository = GetMockRepository(deleteFail: true);
            var notifer = GetMockNotifier();

            SetupInventoryController(repository, notifer);

            var httpResponse = _inventoryController.Delete("Mallet", "hammer");
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void TestDeleteNullLabel()
        {
            var httpResponse = _inventoryController.Delete(null, "hammer");
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.InternalServerError);

            // TODO: Verify something more specific than just StatusCode == HttpStatusCode.InternalServerError.
        }

        [TestMethod]
        public void TestDeleteEmptyType()
        {
            var httpResponse = _inventoryController.Delete("Mallet", "");
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.InternalServerError);

            // TODO: Verify something more specific than just StatusCode == HttpStatusCode.InternalServerError.
        }

        [TestMethod]
        public void TestDeleteRepositoryDeleteItemThrows()
        {
            var repository = GetMockRepository(deleteThrow: true);
            var notifer = GetMockNotifier();
            
            SetupInventoryController(repository, notifer);

            var httpResponse = _inventoryController.Delete("claw hammer", "hammer");
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.InternalServerError);

            // TODO: Verify something more specific than just StatusCode == HttpStatusCode.InternalServerError.
        }

        // TODO: Add a test to verify that the notification provider is called on a DELETE.


        void SetupInventoryController(IInventoryRepository repository, INotificationProvider notifier)
        {
            _inventoryController = new InventoryController(repository, notifier);
            _inventoryController.Request = new HttpRequestMessage();
            _inventoryController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }

        INotificationProvider GetMockNotifier()
        {
            var mockNotifier = new Mock<INotificationProvider>();
            mockNotifier.Setup(x => x.SendDeleteNotification(It.IsAny<InventoryItem>()));

            return mockNotifier.Object;
        }
        IInventoryRepository GetMockRepository(bool deleteFail = false, bool addFail = false, bool getAllFail = false, bool deleteThrow = false)
        {
            var mockRepository = new Mock<IInventoryRepository>();
            if (addFail)
            {
                mockRepository.Setup(x => x.AddItem(It.IsAny<InventoryItem>()));
            }
            else
            {
                mockRepository.Setup(x => x.AddItem(It.IsAny<InventoryItem>()))
                    .Returns(new InventoryItem());
            }

            if (deleteThrow)
            {
                mockRepository.Setup(x => x.DeleteItem(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            } else if (deleteFail)
            {
                mockRepository.Setup(x => x.DeleteItem(It.IsAny<string>(), It.IsAny<string>()));
            }
            else
            {
                mockRepository.Setup(x => x.DeleteItem(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new InventoryItem());
            }

            mockRepository.Setup(x => x.GetAllItems()).Returns(_inventory.ToList());
            return mockRepository.Object;
        }
    }
}
