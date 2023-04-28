using Api.Template.ApplicationCore.Helpers;
using Api.Template.ApplicationCore.Interfaces.Repositories;
using Api.Template.ApplicationCore.Interfaces.Services;
using Api.Template.ApplicationCore.Services;
using Api.Template.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(o =>
{
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<ITemplateRepository, TemplateRepository>();

var app = builder.Build();

Config.ConnectionString = app.Configuration.GetConnectionString("DataBaseConnection");
Config.DefaultPageSize = app.Configuration.GetValue<int>("DefaultPageSize");
Config.KeyVaultClientID = app.Configuration.GetValue<string>("KeyVault:ClientID");
Config.KeyVaultClientSecret = app.Configuration.GetValue<string>("KeyVault:ClientSecret");
Config.SomeExternalAPI = app.Configuration.GetValue<string>("APIs:SomeExternalAPI");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
