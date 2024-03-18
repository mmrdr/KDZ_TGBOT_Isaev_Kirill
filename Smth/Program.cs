using ClassLib;
using System.Net.Security;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace Smth
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            BotUpdates.botClient = new TelegramBotClient(BotUpdates.TELEGRAM_TOKEN);
            BotUpdates.receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                },
                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();

            BotUpdates.botClient.StartReceiving(BotUpdates.UpdateHandler, BotUpdates.ErrorHandler, BotUpdates.receiverOptions, cts.Token);

            var me = BotUpdates.botClient.GetMeAsync().Result;

            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1);
        }
    }
}