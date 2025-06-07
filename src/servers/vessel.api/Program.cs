
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using vessel.api;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer().WithHttpTransport(o => o.Stateless = true).WithTools<VesselTool>();

var authSetting = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authSetting["Authority"];
        options.Audience = authSetting["Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = authSetting.GetSection("ValidIssuers").Get<string[]>()
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapMcp().RequireAuthorization(policy =>
{
    policy.RequireAuthenticatedUser();
    //policy.RequireRole("mcp.read"); 
});
app.Run();
