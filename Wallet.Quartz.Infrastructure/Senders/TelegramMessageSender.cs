using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Infrastructure.Senders;

public class TelegramMessageSender : INotificationSender
{
    private readonly TelegramBotClient _botClient;

    public TelegramMessageSender(IConfiguration configuration)
    {
        var botToken = configuration["Telegram:BotToken"];
        if (string.IsNullOrEmpty(botToken))
        {
            throw new InvalidOperationException("Telegram Bot Token не найден в конфигурации.");
        }

        _botClient = new TelegramBotClient(botToken);
    }

    public async Task SendNotification(long telegramUserId, string name, string description)
    {
        await _botClient.SendTextMessageAsync(
            chatId: telegramUserId,
            text: "Уведомление:\n" + name + "\n" + description
        );
    }
}