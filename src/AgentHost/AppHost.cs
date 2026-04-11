using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


var bookingMcp= builder.AddProject<Projects.booking_api>("bookingapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "local");
var CapacityMcp = builder.AddProject<Projects.capacity_api>("capacityapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "local");

var VesselMcp = builder.AddProject<Projects.vessel_api>("vesselapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "local");

builder.AddProject<Projects.agent_invoker>("invoker")
 
    .WaitFor(bookingMcp)
    .WaitFor(CapacityMcp)
    .WaitFor(VesselMcp);
    

builder.Build().Run();
