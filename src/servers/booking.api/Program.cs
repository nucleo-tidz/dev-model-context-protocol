using booking.api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer().WithHttpTransport().WithTools<BookingContainerTool>();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapMcp();
app.Run();
