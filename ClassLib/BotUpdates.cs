using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using ClassLib;
using Telegram.Bot.Polling;
using System.Net;
using Telegram.Bot.Requests;

namespace Smth
{
    public class BotUpdates
    {

        internal static Stream lastCsvDownload, lastCsvUpload, lastJsonDownload, lastJsonUpload;

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
                else if (HelpingMethods.currentAeroexpressTable == null)
                {
                    if (message.Type == MessageType.Document)
                    {
                        try
                        {
                            await HelpingMethods.DownloadData(update);
                            if (CSVProcessing.fileCorr)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Данные корректны!Загружены");
                                var replyKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
                                {
                                    new KeyboardButton[]
                                    {
                                        new KeyboardButton("Выборка по StationStart"),
                                        new KeyboardButton("Выборка по StationEnd"),
                                        new KeyboardButton("Выборка по StationStart и StationEnd")
                                    },
                                    new KeyboardButton[]
                                    {
                                        new KeyboardButton("Сортировка по TimeStart(в порядке увеличения времени)"),
                                        new KeyboardButton("Сортировка по TimeEnd(в порядке увеличения времени)")
                                    },
                                    new KeyboardButton[]
                                    {
                                        new KeyboardButton("Выгрузить файл в формате CSV"),
                                        new KeyboardButton("Выгрузить файл в формате JSON"),
                                    }
                                })
                                { ResizeKeyboard = true };
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите нужную опцию", replyMarkup: replyKeyboard);
                            }
                            else await botClient.SendTextMessageAsync(message.Chat.Id, "Некорректные данные");
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
                else if(message.Text == "Выборка по StationStart")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                }
                else if(message.Text == "Выборка по StationEnd")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                }
                else if(message.Text == "Выборка по StationStart и StationEnd")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                }
                else if (message.Text == "Сортировка по TimeStart(в порядке увеличения времени)")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else CSVProcessing.SortTimeStart();
                }
                else if (message.Text == "Сортировка по TimeEnd(в порядке увеличения времени)")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else CSVProcessing.SortTimeEnd();
                }
                else if (message.Text == "Выгрузить файл в формате CSV")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    //else CSVProcessing.UploadFile(botClient, update);
                }
                else if (message.Text == "Выгрузить файл в формате JSON")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                }
                else
                {
                    if (message.Type == MessageType.Document)
                    {
                        await HelpingMethods.DownloadData(update);
                        if (CSVProcessing.fileCorr)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Данные корректны!Загружены");
                            var replyKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>
                            {
                                new KeyboardButton[]
                                {
                                new KeyboardButton("Выборка по StationStart"),
                                new KeyboardButton("Выборка по StationEnd"),
                                new KeyboardButton("Выборка по StationStart и StationEnd")
                                },
                                new KeyboardButton[]
                                {
                                    new KeyboardButton("Сортировка по TimeStart(в порядке увеличения времени)"),
                                    new KeyboardButton("Сортировка по TimeEnd(в порядке увеличения времени)")
                                },
                                new KeyboardButton[]
                                {
                                    new KeyboardButton("Выгрузить файл в формате CSV"),
                                    new KeyboardButton("Выгрузить файл в формате JSON"),
                                }
                            })
                            { ResizeKeyboard = true };
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите нужную опцию", replyMarkup: replyKeyboard);
                        }
                        else await botClient.SendTextMessageAsync(message.Chat.Id, "Некорректные данные");
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