using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var vehicleDb = postgres.AddDatabase("postgresdbvehicles");
var tenantDb = postgres.AddDatabase("postgresdbtenants");

builder.AddProject<SmartCraft_Core_Tellus_Api>("smartcraft-core-tellus-api")
    .WithReference(vehicleDb).WithReference(tenantDb);

builder.Build().Run();
