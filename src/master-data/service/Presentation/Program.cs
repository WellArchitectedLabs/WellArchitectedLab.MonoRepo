using MasterData.Api.Constants;
using MasterData.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.SetupConfiguration();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.DefaultCorsPolicy, corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});

builder.Services.RegisterLayers();

var app = builder.Build();

app.MapLiveness();
app.MapReadiness();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(ApiConstants.DefaultCorsPolicy);

app.MapControllers();

app.Run();
