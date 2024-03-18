using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using ClassLib;
using Telegram.Bot.Polling;
using System.Net;

namespace Smth
{
    public class BotUpdates
    {
        public const string TELEGRAM_TOKEN = "7067595343:AAFGej232xNIbzGt91dXZP-_rMgL0R8BHfQ";

        public static ITelegramBotClient botClient;

        public static ReceiverOptions receiverOptions;

        public static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Привет! Для начала - прикрепи файл!\n" +
                        "Доступные расширения: .csv, .json");
                }               
                else if (HelpingMethods.currentAeroexpressTableCsv == null && HelpingMethods.currentAeroexpressTableJson == null)
                {
                    if (message.Type == MessageType.Document)
                    {
                        try
                        {
                            await HelpingMethods.DownloadData(update);                           
                        }
                        catch(Exception ex)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Либо прикрепляйте файл, либо молчите!");
                    }
                }
                else
                {
                    if (message.Type == MessageType.Document)
                    {
                        await HelpingMethods.DownloadData(update);
                        return;
                    }
                }
            }
        }

        public static Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

    }
}