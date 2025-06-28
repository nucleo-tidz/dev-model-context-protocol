using capacity.api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer().WithHttpTransport(option => { option.Stateless = true; }).WithTools<CapacityTool>();
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
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapMcp().RequireAuthorization(policy =>
{
    policy.RequireAuthenticatedUser();
    policy.RequireRole("mcp.shipment");
});
app.Run();
