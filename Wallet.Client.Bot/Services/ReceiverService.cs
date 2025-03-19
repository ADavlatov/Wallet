using Telegram.Bot;
using Wallet.Client.Bot.Abstract;

namespace Wallet.Client.Bot.Services;

public class ReceiverService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<ReceiverServiceBase<UpdateHandler>> logger)
    : ReceiverServiceBase<UpdateHandler>(botClient, updateHandler, logger);