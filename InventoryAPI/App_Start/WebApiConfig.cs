using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using InventoryAPI.NotificationProvider;
using InventoryRepository;
using InventoryRepository.Models;
using Microsoft.Practices.Unity;

namespace InventoryAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterType<IInventoryRepository, InventoryRepo>(new HierarchicalLifetimeManager());
            container.RegisterType<INotificationProvider, InventoryAPI.NotificationProvider.NotificationProvider>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Add",
                routeTemplate: "api/{controller}/add"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{label}/{type}"
            );

        }
    }
}
