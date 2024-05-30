# Getting Started
## Onboarding
For now, you need to contact us at
tellus@smartcraft.com
to be registered as a user of the API if you wish to access the SmartCraft hosted Tellus service

## Tenant

### Create your tenant

Your first request should be to create your tenant. 
You do this by making a POST request to the _/Tenants_ endpoint and entering the credentials you wish to add to this object in the body of the request.
</br>
  "volvoCredentials": "string",</br>
  "scaniaClientId": "string",</br>
  "scaniaSecretKey": "string",</br>
  "manToken": "string",</br>

Your credentials can always be added/updated by making a PATCH request to the _/Tenants/{ID}_ endpoint

### Credentials specification
To access a users data from, for example, their Scania trucks, you need to retrieve their Scania API client credentials.
Below is a how-to for each manufacturer

**Scania** </br>
1. Ask the truck owner to contact their Scania dealer and ask for a My Scania account with a
   "Developer" role added to it. The owner will need an appropriate Scania subscription with
   access to fuel consumption data.
2. Have the truck owner follow https://developer.scania.com/get-started-customer to get a
  Client ID and a Secret Key.
3. Collect this data when creating the tenant in the Tellus API.

**Volvo** </br>
1. Ask the truck owner to sign into their Volvo Connect account. They will need a
subscription with the data package Transport Data.
2. Press the Digital tools button in the top right (a square symbol with lots of squares). Go to
Administration.
3. Press API Manager and create an API Account.
4. Fill in an optional user name and choose to share position, vehicle status and fuel &
environmental data.
5. Either choose included vehicles or choose Include All Current and Future Vehicles
6. Copy the username and password. Collect this data when creating the tenant in the Tellus
API.</br>
_Please note that Tellus requires the credentials to be formatted "{userName}:{password}"_

**MAN & Mercedes**

Instructions will be available soon. Contact tellus@smartcraft.com if you need help finding the
necessary credentials.

## Vehicles

### Make a request
**TenantId**</br>
All requests made to Tellus requires your _tenantId_ to be added as a header.

**Vehicle brands**</br>
The vehicle brands currently supported by the API are "Volvo", "Scania" and "MAN", with "Daimler" being on the roadmap.

**Get vehicles**</br>
By making a GET request to the _/Vehicles/{vehicleBrand}/Vehicles_ endpoint you will get a list of vehicles in your fleet.

**Get report**</br>
By making a GET request to the _/Vehicles/{vehicleBrand}/EsgReport_ you will get a report for vehicle(s)
Some queries are required, while some are optional.
- VinOrId(optional) - If excluded, this endpoint will fetch all vehicles in your fleet.
- StartTime (required) - formatted as YYYYMMDD.
- StopTime (optional) - formatted as YYYYMMDD. If excluded, will default to the current date.
