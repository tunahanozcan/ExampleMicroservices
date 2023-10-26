using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json");

//builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();



builder.Services.AddOcelot();


var app = builder.Build();





app.UseAuthorization();
app.UseDeveloperExceptionPage();
app.MapControllers();
await app.UseOcelot();
app.Run();
