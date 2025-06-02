
using vessel.api;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer().WithHttpTransport().WithTools<VesselTool>();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapMcp();
app.Run();
