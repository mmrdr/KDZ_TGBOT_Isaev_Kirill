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
                else if (HelpingMethods.IsSelecting)
                {
                    if (message.Type != MessageType.Text)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Некорректное значение для выборки, повторите попытку");
                        return;
                    }
                    else
                    {
                        if (HelpingMethods.curFieldToSelect == "StationStart")
                        {
                            CSVProcessing.TakeValueToSelect(message.Text);
                            HelpingMethods.IsSelecting = false;
                            CSVProcessing.StationSelection(HelpingMethods.curFieldToSelect, HelpingMethods.curValueToSelect);
                            Answer(update);
                        }
                        if (HelpingMethods.curFieldToSelect == "StationEnd")
                        {
                            CSVProcessing.TakeValueToSelect(message.Text);
                            HelpingMethods.IsSelecting = false;
                            CSVProcessing.StationSelection(HelpingMethods.curFieldToSelect, HelpingMethods.curValueToSelect);
                            Answer(update);
                        }
                        if (HelpingMethods.curFieldToSelect == "StationStart,StationEnd")
                        {
                            CSVProcessing.TakeValuesToSelect(message.Text);
                            HelpingMethods.IsSelecting = false;
                            CSVProcessing.BothStationSelect(HelpingMethods.curValuesToSelect);
                            Answer(update);
                        }
                    }
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
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Данные корректны! Загружены");
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите нужную опцию", replyMarkup: HelpingMethods.ShowButtons());
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
                    else
                    {
                        CSVProcessing.TakeFieldToSelect(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Введите значение для выборки\n" +
                            "Параметр должен точно совпадать с параметром из файла");
                        HelpingMethods.IsSelecting = true;
                    }
                }
                else if(message.Text == "Выборка по StationEnd")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else
                    {
                        CSVProcessing.TakeFieldToSelect(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Введите значение для выборки\n" +
                            "Параметр должен точно совпадать с параметром из файла");
                        HelpingMethods.IsSelecting = true;
                    }
                }
                else if(message.Text == "Выборка по StationStart,StationEnd")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else
                    {
                        CSVProcessing.TakeFieldToSelect(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Введите значение для выборки\n" +
                             "Параметр должен точно совпадать с параметром из файла\n" +
                             "[значение;значение]");
                        HelpingMethods.IsSelecting = true;
                    }
                       
                }
                else if (message.Text == "Сортировка по TimeStart(в порядке увеличения времени)")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else
                    {
                        CSVProcessing.SortTimeStart();
                        Answer(update);
                    }
                }
                else if (message.Text == "Сортировка по TimeEnd(в порядке увеличения времени)")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else 
                    {
                        CSVProcessing.SortTimeEnd();
                        Answer(update);
                    } 
                }
                else if (message.Text == "Выгрузить файл в формате CSV")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else
                    {
                        HelpingMethods.numberOfFile++;
                        await HelpingMethods.UploadCsvFile(botClient, update);
                    }
                }
                else if (message.Text == "Выгрузить файл в формате JSON")
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для начала - прикрепи файл!");
                        return;
                    }
                    else
                    {
                        HelpingMethods.numberOfFile++;
                        await HelpingMethods.UploadJsonFile(botClient, update);
                    }
                }
                else
                {
                    if (message.Type == MessageType.Document)
                    {
                        await HelpingMethods.DownloadData(update);
                        if (CSVProcessing.fileCorr)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Данные корректны! Загружены");

                            await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите нужную опцию", replyMarkup: HelpingMethods.ShowButtons());
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

        private static async void Answer(Update update)
        {
            if (HelpingMethods.currentAeroexpressTable.Count == 0 || CSVProcessing.SelectedAeroexpressTableCsv.Count == 0)
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не нашлось значения.\n" +
                    "Что дальше?", replyMarkup: HelpingMethods.ShowButtons());
            else await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Успех, проведена выборка.\n" +
                "Что дальше?", replyMarkup: HelpingMethods.ShowButtons());
        }
    }
}