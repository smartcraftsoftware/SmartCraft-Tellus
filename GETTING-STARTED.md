# Getting Started

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

**Volvo** </br>
Credentials for Volvo can be created at https://developer.volvotrucks.com/
- Volvo uses basic Authorization.
- In Tellus, your volvo credentials should be formatted "{userName}:{password}"
- The volvo credentials should be added to the "_volvoCredentials_"-field of your tenant

**Scania** </br>
Credentials for Scania can be created at https://developer.scania.com/
- Create an account as an FMS-developer.
- Go to _my clients_ and create a new client.
- Copy your Client-Id and SecretKey to the corresponding fields of your tenant.

**MAN**
TBD

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
