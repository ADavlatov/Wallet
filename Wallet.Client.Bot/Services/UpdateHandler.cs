using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Wallet.Client.Bot.Models;

namespace Wallet.Client.Bot.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, HttpClient httpClient)
    : IUpdateHandler
{
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        Message sentMessage = await (messageText.Split(' ')[0] switch
        {
            _ => Usage(msg)
        });
        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """Отправьте api ключ полученный на сайте""";

        if (msg.Text == null || msg.Text.Split().Length != 1)
        {
            return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
        }

        var request = new ApiKeyRequest(msg.Text, msg.From.Id);
        var content = JsonSerializer.Serialize(request);
        var response = await httpClient.PostAsync("/api/v1/users/ValidateApiKey",
            new StringContent(content, Encoding.UTF8, "application/json"));
        Console.WriteLine(await new StringContent(content, Encoding.UTF8, "application/json").ReadAsStringAsync());
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        Console.WriteLine(response.RequestMessage.RequestUri);
        if (response.IsSuccessStatusCode)
        {
            return await bot.SendMessage(msg.Chat, "Вы успешно привязались", parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
        }

        return await bot.SendMessage(msg.Chat, "Ошибка", parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}