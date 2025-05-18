using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddMcpServer().WithHttpTransport()
    .WithToolsFromAssembly(Assembly.Load("mcp.tools"));
var app = builder.Build();
app.UseHttpsRedirection();
app.MapMcp();
app.Run();
