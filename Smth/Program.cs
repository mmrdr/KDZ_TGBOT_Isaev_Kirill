using System.Net.Security;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace Smth
{
    internal class Program
    {
        private const string TELEGRAM_TOKEN = "7067595343:AAFGej232xNIbzGt91dXZP-_rMgL0R8BHfQ";

        private static ITelegramBotClient botClient;

        private static ReceiverOptions receiverOptions;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient(TELEGRAM_TOKEN);
            receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                },
                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();

            botClient.StartReceiving(BotUpdates.UpdateHandler, BotUpdates.ErrorHandler, receiverOptions, cts.Token);

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"{me.Username} запущен!");

            await Task.Delay(-1);
        }
    }
}