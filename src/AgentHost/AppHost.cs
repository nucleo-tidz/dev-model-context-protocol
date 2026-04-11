using Aspire.Hosting;
using StackExchange.Redis;


var builder = DistributedApplication.CreateBuilder(args);


var bookingMcp= builder.AddProject<Projects.booking_api>("bookingapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "local");
var CapacityMcp = builder.AddProject<Projects.capacity_api>("capacityapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "local");

var VesselMcp = builder.AddProject<Projects.vessel_api>("vesselapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "local");

var cache = builder.AddRedis("cache")
    .WithRedisInsight();



//var flyway = builder.AddContainer("flyway", "flyway/flyway")
//    .WithArgs("migrate")
//    .WithEnvironment("FLYWAY_URL", "jdbc:sqlserver://host.docker.internal:1433;databaseName=nucleotidz;encrypt=true;trustServerCertificate=true")
//    .WithEnvironment("FLYWAY_USER", "sa")
//    .WithEnvironment("FLYWAY_PASSWORD", "")
//    .WithBindMount(
//        source: "../migrations",
//        target: "/flyway/sql"
//    );

builder.AddProject<Projects.agent_invoker>("invoker")
    //.WaitForCompletion(flyway)
    .WaitFor(bookingMcp)
    .WaitFor(CapacityMcp)
    .WaitFor(VesselMcp);
  //  .WithReference(cache);

builder.Build().Run();
