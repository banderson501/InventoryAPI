using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InventoryAPI.Models;
using InventoryAPI.NotificationProvider;
using InventoryRepository.Models;

namespace InventoryAPI.Controllers
{
    public class InventoryController : ApiController
    {
        private IInventoryRepository _repository;
        private INotificationProvider _notifier;

        public InventoryController(IInventoryRepository repository, INotificationProvider notifier)
        {
            // TODO: Should also inject a logging subsystem here.

            _repository = repository;
            _notifier = notifier;
        }

        // POST: api/Inventory/add
        public HttpResponseMessage PostAdd([FromBody]InventoryAPI.Models.InventoryItem item)
        {
            HttpResponseMessage response;
            if (_repository == null)
            {
                // TODO: Need to implement a logging subsystem and log this.

                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, "Null _repository.");
            }

            try
            {
                // Validate the request.
                ValidateItem(item);

                var addedItem = _repository.AddItem(ToRepositoryItem(item));
                if (addedItem == null)
                {
                    response = Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError,
                        "Item is already in the inventory repository.");
                }
                else
                {
                    response =
                        Request.CreateResponse<InventoryRepository.Models.InventoryItem>(
                            System.Net.HttpStatusCode.Created, addedItem);
                }
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                // TODO: Map exceptions to HttpStatusCodes and api return codes and api return messages that we define for returning to clients.

                response = Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }

        // DELETE: api/Inventory/{label}/{type}
        public HttpResponseMessage Delete(string label, string type)
        {
            HttpResponseMessage response;
            if (_repository == null)
            {
                // TODO: Need to implement a logging subsystem and log this.

                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, "Null _repository.");
            }

            if (_notifier == null)
            {
                // TODO: Need to implement a logging subsystem and log this.

                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, "Null _notifier.");
            }

            // Validate parameters.
            if (String.IsNullOrEmpty(label) || String.IsNullOrEmpty(type))
            {
                // TODO: Log a message about this.
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, "Label or Type is null or empty.");
            }

            try
            {
                var item = _repository.DeleteItem(label, type);
                if (item == null)
                {
                    response = Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
                else
                {
                    _notifier.SendDeleteNotification(item);
                        // In a production system this probably puts something on a queue for later processing by subscribers.

                    response = Request.CreateResponse(System.Net.HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                // TODO: Map exceptions to HttpStatusCodes and api return codes and api return messages that we define for returning to clients.

                response = Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError,
                    string.Format("Unexpected Exception. ex:{0}.", ex.ToString()));
            }

            return response;
        }

        private void ValidateItem(InventoryAPI.Models.InventoryItem item)
        {
            if (item == null)
            {
                // TODO: Log a message about this.
                throw new ArgumentNullException("item");
            }

            if (String.IsNullOrEmpty(item.Label) || String.IsNullOrEmpty(item.Type))
            {
                // TODO: Log a message about this.
                throw new ArgumentException("Item Label or Type is null or empty.");
            }

            // TODO: Validate Expiration value
        }


        // Could aggregate conversion utilities like this into a separate class. Could be extension methods.

        // For this exercise the InventoryAPI.Models.InventoryItem and InventoryRepository.Models.InventoryItem definitions ended up
        // the same but originally I had a count in the InventoryRepository.Models.InventoryItem. Unless each layer is sharing a common
        // model for this class then they will be different and this is needed to transform the one used in one layer to that used in another.
        private InventoryRepository.Models.InventoryItem ToRepositoryItem(InventoryAPI.Models.InventoryItem item)
        {
            return new InventoryRepository.Models.InventoryItem
            {
                Label = item.Label.ToLower(),
                Type = item.Type.ToLower(),
                Expiration = item.Expiration
            };
        }
    }
}
