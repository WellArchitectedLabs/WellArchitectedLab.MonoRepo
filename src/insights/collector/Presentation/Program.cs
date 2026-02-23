using FluentValidation.AspNetCore;
using WfInsights.Collector.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.SetupConfiguration();

builder.Services.AddControllers();

// add auto validation: https://github.com/FluentValidation/FluentValidation.AspNetCore?tab=readme-ov-file#get-started
// installed from FluentValidation.AspNetCore nuget package (different from the core FluentValidation nuget package)
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();
builder.RegisterReadinessChecks();

builder.Services.RegisterLayers(builder.Configuration);

var app = builder.Build();

app.MapLiveness();
app.MapReadiness();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.UseCors("AllowAllPolicy");

app.MapControllers();

app.Run();