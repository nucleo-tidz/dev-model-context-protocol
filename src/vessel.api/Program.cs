using System.Reflection;

using mcp.tools;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddMcpServer().WithHttpTransport().WithTools<VesselTool>();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapMcp();
app.Run();
