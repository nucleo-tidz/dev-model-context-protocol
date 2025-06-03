using reposition.server;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer().WithHttpTransport().WithTools<RespositionTool>();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapMcp();
app.Run();
