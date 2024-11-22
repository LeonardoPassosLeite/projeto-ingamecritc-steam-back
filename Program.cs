using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string ExpandEnvironmentVariables(string value)
{
    foreach (var variable in Environment.GetEnvironmentVariables().Keys)
    {
        var key = $"${{{variable}}}";
        var variableValue = Environment.GetEnvironmentVariable(variable.ToString());
        if (value != null && value.Contains(key))
        {
            value = value.Replace(key, variableValue);
        }
    }
    return value;
}

var connectionStringTemplate = builder.Configuration.GetConnectionString("PostgresConnection");
var postgresConnectionString = ExpandEnvironmentVariables(connectionStringTemplate);

builder.Services.AddDbContext<SteamChartsDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddHttpClient("ScrapingClient", client =>
{
    var scraperBaseUrl = ExpandEnvironmentVariables(builder.Configuration["ScrapingBackend:BaseUrl"]);
    if (scraperBaseUrl == null)
    {
        throw new Exception("A variável de ambiente 'SCRAPER_BACKEND_URL' não está definida ou foi configurada incorretamente.");
    }
    Console.WriteLine($"SCRAPER_BACKEND_URL: {scraperBaseUrl}"); // Log para depuração
    client.BaseAddress = new Uri(scraperBaseUrl);
});

builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();