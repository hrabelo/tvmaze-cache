using TVMazeCache.ApiClient.Infrastructure;
using TVMazeCache.Persistence.MongoDB.Infrastructure;
using TVMazeCache.WebApi;
using TVMazeCache.WebApi.BackgroundServices;
using TVMazeCache.Domain.UseCases;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

var appName = "TVMazeCache";

builder.Logging.AddConsole();

var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = new MediaTypeApiVersionReader("v");
});

builder.Services.AddVersionedApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
});

var settings = new Settings(configuration);

builder.Services.AddSingleton(settings.StoreShowsBackgroundServiceSettings);
builder.Services.AddSingleton(settings.TvMazeApiClientSettings);
builder.Services.AddTvMazeApiClient(appName, settings.TvMazeApiClientSettings);

builder.Services.AddSingleton(settings.MongoDbShowsRepositorySettings);
builder.Services.AddShowsRepository();

builder.Services.AddTransient<StoreShowsWithCastUseCase>();
builder.Services.AddTransient<RetrieveShowsWithCastUseCase>();

builder.Services.AddHostedService<StoreShowsBackgroundService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.MapControllers();

app.Run();
