using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Wallet.Client.Web.Interfaces;
using Wallet.Quartz.Infrastructure.Contexts;

namespace Wallet.Quartz.Application.Services;
//
// public class TelegramBotService(TelegramBotClient botClient, IUserService userService, QuartzContext db)
//     : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         var receiverOptions = new ReceiverOptions
//         {
//             AllowedUpdates = new { UpdateType.Message }
//         };
//
//         using var receiver = new DefaultUpdateHandler(botClient, HandleUpdateAsync, HandleErrorAsync);
//         botClient.StartReceiving(
//             receiver,
//             receiverOptions,
//             stoppingToken
//         );
//
//         var me = await botClient.GetMeAsync(cancellationToken: stoppingToken);
//         Console.WriteLine($"Бот запущен: @{me.Username}");
//
//         await Task.Delay(Timeout.Infinite, stoppingToken);
//     }
//
//     private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
//         CancellationToken cancellationToken)
//     {
//         if (update.Message is not { } message)
//             return;
//         if (message.Text is not { } messageText)
//             return;
//
//         var chatId = message.Chat.Id;
//
//         Console.WriteLine($"Получено сообщение '{messageText}' от {chatId}.");
//
//         // Проверяем, является ли сообщение ключом для привязки
//         if (TelegramBindingStorage.KeyToTelegramId.ContainsKey(messageText))
//         {
//             await botClient.SendTextMessageAsync(
//                 chatId: chatId,
//                 text: "Этот ключ уже использован.",
//                 cancellationToken: cancellationToken);
//             return;
//         }
//
//         // Здесь вам нужно будет проверить, существует ли такой ключ в вашей базе данных пользователей
//         // и получить идентификатор пользователя на вашем сайте, связанный с этим ключом.
//         // Для примера, предположим, что вы можете получить эту информацию из сервиса.
//         using (var scope = _serviceProvider.CreateScope())
//         {
//             var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
//             var user = userService.GetUserByBindingKey(messageText);
//
//             if (user != null)
//             {
//                 // Связываем Telegram ID с пользователем
//                 TelegramBindingStorage.KeyToTelegramId[messageText] = chatId;
//                 user.TelegramId = chatId;
//                 user.TelegramBindingKey = null; // Очищаем использованный ключ
//                 userService.UpdateUser(user);
//
//                 await botClient.SendTextMessageAsync(
//                     chatId: chatId,
//                     text: $"Ваш Telegram аккаунт успешно привязан!",
//                     cancellationToken: cancellationToken);
//             }
//             else
//             {
//                 await botClient.SendTextMessageAsync(
//                     chatId: chatId,
//                     text: "Неверный ключ привязки.",
//                     cancellationToken: cancellationToken);
//             }
//         }
//     }
//
//     private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
//         CancellationToken cancellationToken)
//     {
//         var ErrorMessage = exception switch
//         {
//             ApiRequestException apiRequestException =>
//                 $"Ошибка Telegram API:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
//             _ => exception.ToString()
//         };
//
//         Console.WriteLine(ErrorMessage);
//         return Task.CompletedTask;
//     }
// }