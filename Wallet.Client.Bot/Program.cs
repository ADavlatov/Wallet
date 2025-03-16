using Microsoft.Extensions.Options;
using Telegram.Bot;
using Wallet.Client.Bot;
using Wallet.Client.Bot.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Bot configuration
builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));

builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
        ArgumentNullException.ThrowIfNull(botConfiguration);
        TelegramBotClientOptions options = new(botConfiguration.BotToken);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped(x => {
    var apiUrl = new Uri("http://localhost:5221");
    var httpClient = new HttpClient { BaseAddress = apiUrl };
    return httpClient;
});
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();