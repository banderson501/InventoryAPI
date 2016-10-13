Setup
•	Open the solution file and build it. I used VS 2015.
•	Start the InventoryAPI service in Visual Studio.
•	Use your favorite HTTP client to send requests to the service running on local host. You can use these requests as templates.
o	Add Item to Inventory
POST /api/Inventory/add HTTP/1.1
Host: localhost:52572
Accept: application/json
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: 85cfd318-5a43-510e-1bd5-05a7fde523c6
{
	"Label":"Sledge Hammer",
	"Expiration":"2016-10-12T14:44:00",
	"Type":"Hammer"
}
o	Take Item away from Inventory
DELETE /api/Inventory/claw hammer/hammer HTTP/1.1
Host: localhost:52572
Accept: application/json
Content-Type: application/json
Cache-Control: no-cache
Postman-Token: 9602b782-dced-41b0-26c4-b7b5c9c0d7d4
•	To verify that the service is working correctly.
o	Open the InventoryRepo.cs file
o	By default the service checks for expired items every minute. The frequency is configurable in the app.config file and for a production system would be determined by business requirements.
o	Edit the Expiration times in the _inventory table to some different times over the next few minutes.
o	You can start this service under Visual Studio (start it as an Administrator so that it can write to its logging file).
o	A line is written to the c:\ServiceLog.txt file for each inventory item which is expired when the service runs.
o	You can also start the service from the command line using the steps below but I still don’t have the logging working when the service this way. This is what I would work on next.
•	To run the service from the command line.
o	Start a Windows command prompt as Administrator
o	Cd to the bin directory for the InventoryExpiryService project.
?	Something like, C:\Users\Brian\Documents\Visual Studio 2015\Projects\InventoryExpiryService\InventoryExpiryService\bin\Debug>
o	Install the service with the following command
?	InventoryExpiryService.exe –install
o	You can the Services utility to verify that the InventoryExpiryService is running.
o	On each interval the service will use the InventoryRepository to enumerate all of the inventory items and check the expiration date of each one. 
o	If an item has expired a message is written to the c:\ServiceLog.txt file bin\debug directory. – Don’t have the logging working yet when run this way. See above.
o	To uninstall the service use this command
?	InventoryExpiryService.exe –uninstall
Notes
1.	I implemented the most important parts of the challenge, the two API methods and the service which checks for expired items. For the service I started with a service project template which provided most of the service code.
2.	The solution has four projects.
1.	InventoryAPI
2.	InventoryAPI.Tests – unit tests for the API controller
3.	InventoryExpiryService – The service which checks for expired items
?	I used a predefined service template available through VS’s New Project dialog and did not make a lot of changes to this code. The code which checks for expired inventory items is in the ServiceImplementation.cs file, SchedularCallback method. I also changed the file logging a bit.
4.	InventoryRepository – a barebones, in memory, predefined, inventory store
?	This assembly is accessed by both the Api and the service
3.	I used http DELETE for the take operation. A POST could have been used and include the label and type in an object in the body, along with other information in the future.
4.	Basic security can be provided by requiring clients to use HTTPS to access the API. 
5.	I used a linq FirstOrDefault call which matches the label and type. Before Josh told me that the label:type combo should be used as the unique key for an inventory entry I was using a dictionary with the label as the key. But this is just to access the in memory, mock data set. A true production system would use some sort of DB which would determine the type of query functions available.
6.	I setup dependency injection and use it in the Api controller for the repository and the notification provider. Future work would include using it for a logging service. I use DI in the unit tests to inject mock versions of the repository and the notification provider.
7.	See comments in code for additional notes.

Questions
1.	What are the valid ranges for Expiration date/times?
2.	When we find an inventory items which is expired, we notify. Should we also remove the item from the inventory?  Not doing this right now.
3.	Should time be specified in UTC time or local client time? Current code assumes time provided in to Add item endpoint is the same time zone as the time of the server where the Expiry service is running.
4.	If the add-to-inventory endpoint is called when the item is already in the data store, should we return an error?
a.	Yes
TODO:
1.	While I did DI for the InventoryRepository in to the InventoryController, I did not to DI for the InventoryRepository in to the expiration service.
2.	A logging provider should be injected into the InventoryController.
3.	Add unit tests for the Add operation. I have added test for the Delete endpoint to give you an idea of how I do unit testing. Due to time constraints I have not yet added tests for the Add endpoint.
4.	Add unit tests for InventoryRepository and InventoryExpiryService.
5.	Add Expiration validation.
6.	See TODOs in code for more.

