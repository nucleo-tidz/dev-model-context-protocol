using mcp.tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer().WithHttpTransport().WithTools<CapacityTool>();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapMcp();
app.Run();
