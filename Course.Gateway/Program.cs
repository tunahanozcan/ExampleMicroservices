using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json");

builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationSchema", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.Audience = "resource_gateway";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddOcelot();


var app = builder.Build();





app.UseAuthorization();
app.UseDeveloperExceptionPage();
app.MapControllers();
await app.UseOcelot();
app.Run();
