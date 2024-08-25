using BL.Standard;
using Common.Configuration;
using Dal.SQL;
using TokenCleanupService.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDataAccessLayer(configuration);
builder.Services.AddBusinessLogicLayer();

var cleanupIntervalHours = configuration.GetValue<int?>("CleanupIntervalHours");
SharedConfiguration.UpdateSharedConfiguration(cleanupIntervalHours: cleanupIntervalHours);

builder.Services.AddHostedService<TokenCleanup>();

var app = builder.Build();

app.Run();
