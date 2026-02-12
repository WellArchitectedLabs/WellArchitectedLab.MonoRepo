using FluentValidation.AspNetCore;
using WfInsights.Collector.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.SetupConfiguration();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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

builder.Services.RegisterLayers(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.UseCors("AllowAllPolicy");

app.MapControllers();

app.Run();