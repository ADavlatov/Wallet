using Wallet.Client.Bot.Abstract;

namespace Wallet.Client.Bot.Services;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);