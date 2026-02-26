using FluentValidation.AspNetCore;
using Scalar.AspNetCore;
using WfExperience.Bff.Api.Constants;
using WfExperience.Bff.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.SetupConfiguration();

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.AllowAllPolicyName, corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});


builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.RegisterLayers(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // only activate for development as best practise
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapLiveness();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(ApiConstants.AllowAllPolicyName);

app.MapControllers();

app.Run();
