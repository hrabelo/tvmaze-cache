using TVMazeCache.WebApi;
using TVMazeCache.WebApi.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

var settings = new Settings(configuration);
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton(settings.StoringBackgroundServiceSettings);

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
