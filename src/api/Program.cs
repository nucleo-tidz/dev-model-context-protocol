var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();


//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
app.MapMcp();
app.Run();
