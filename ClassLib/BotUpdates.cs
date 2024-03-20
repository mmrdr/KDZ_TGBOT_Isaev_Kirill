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
        public const string TELEGRAM_TOKEN = "7067595343:AAFGej232xNIbzGt91dXZP-_rMgL0R8BHfQ";

        public static ITelegramBotClient botClient;

        public static ReceiverOptions receiverOptions;

        /// <summary>
        /// Реагирует на сообщения пользователя в чате. Реализует все взаимодействие с пользователем.
        /// </summary>
        /// <param name="botClient">Наш бот.</param>
        /// <param name="update">Текущее вообщение пользователя, отправленное в чат</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                Console.WriteLine($"Пользователь {message.From} написал сообщение \"{message.Text}\" в {DateTime.Now}");
                if (message.Text == "/start")
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton[]>()
                    {
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.WithUrl("Ссылка на Github", "https://github.com/mmrdr/KDZ_TGBOT_Isaev_Kirill"),
                            InlineKeyboardButton.WithUrl("Ссылка на readme файл", "https://github.com/mmrdr/KDZ_TGBOT_Isaev_Kirill/blob/master/README.md"),
                        },
                    });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Немного о самом боте(readme обязательно к прочтению)", replyMarkup: inlineKeyboard);
                }
                else if (message.Text == "Что такое магическая битва?")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Прекрасный вопрос, тут Вы найдете опенинг", replyMarkup: HelpingMethods.ShowMagic());
                }
                else if (HelpingMethods.IsSelecting) // Костыль, реализующий адекватный запрос о выборке у пользователя.
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
                            DataIteraction.TakeValueToSelect(message.Text);
                            HelpingMethods.IsSelecting = false;
                            DataIteraction.StationSelection(HelpingMethods.curFieldToSelect, HelpingMethods.curValueToSelect);
                            HelpingMethods.Answer(update, botClient);
                        }
                        if (HelpingMethods.curFieldToSelect == "StationEnd")
                        {
                            DataIteraction.TakeValueToSelect(message.Text);
                            HelpingMethods.IsSelecting = false;
                            DataIteraction.StationSelection(HelpingMethods.curFieldToSelect, HelpingMethods.curValueToSelect);
                            HelpingMethods.Answer(update, botClient);
                        }
                        if (HelpingMethods.curFieldToSelect == "StationStart,StationEnd")
                        {
                            DataIteraction.TakeValuesToSelect(message.Text);
                            HelpingMethods.IsSelecting = false;
                            DataIteraction.BothStationSelect(HelpingMethods.curValuesToSelect);
                            HelpingMethods.Answer(update, botClient);
                        }
                    }
                }
                else if (HelpingMethods.currentAeroexpressTable == null)
                {
                    if (message.Type == MessageType.Document)
                    {
                        try
                        {
                            await BotIteraction.DownloadData(update);
                            if (HelpingMethods.fileCorr)
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
                else if(message.Text == ConstStrings.funcSelStationStart)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else
                    {
                        DataIteraction.TakeFieldToSelect(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.selectChoose);
                        HelpingMethods.IsSelecting = true;
                    }
                }
                else if(message.Text == ConstStrings.funcSelStationEnd)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else
                    {
                        DataIteraction.TakeFieldToSelect(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.selectChoose);
                        HelpingMethods.IsSelecting = true;
                    }
                }
                else if(message.Text == ConstStrings.funcSelBoth)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else
                    {
                        DataIteraction.TakeFieldToSelect(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.selectChoose + "\n" + "[значение;значение]");
                        HelpingMethods.IsSelecting = true;
                    }
                       
                }
                else if (message.Text == ConstStrings.funcSortTimeStart)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else
                    {
                        DataIteraction.SortTimeStart();
                        HelpingMethods.Answer(update, botClient);
                    }
                }
                else if (message.Text == ConstStrings.funcSortTimeEnd)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else 
                    {
                        DataIteraction.SortTimeEnd();
                        HelpingMethods.Answer(update, botClient);
                    } 
                }
                else if (message.Text == ConstStrings.funcUploadCsv)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else
                    {
                        HelpingMethods.numberOfFile++;
                        await BotIteraction.UploadCsvFile(botClient, update);
                    }
                }
                else if (message.Text == ConstStrings.funcUploadJson)
                {
                    if (HelpingMethods.currentAeroexpressTable == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, ConstStrings.rage);
                        return;
                    }
                    else
                    {
                        HelpingMethods.numberOfFile++;
                        await BotIteraction.UploadJsonFile(botClient, update);
                    }
                }
                else if (message.Type == MessageType.Document)
                {
                    await BotIteraction.DownloadData(update);
                    if (HelpingMethods.fileCorr)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Данные корректны! Загружены");

                        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите нужную опцию", replyMarkup: HelpingMethods.ShowButtons());
                    }
                    else await botClient.SendTextMessageAsync(message.Chat.Id, "Некорректные данные");
                    return;
                }
                else if (HelpingMethods.CheckMessage(update))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Используйте команды, которые я умею обрабатывать", replyMarkup: HelpingMethods.ShowButtons());
                }
            }
        }
        /// <summary>
        /// Отлавливает исключения.
        /// </summary>
        /// <param name="botClient">Наш бот.</param>
        /// <param name="exception">Возникшее исключение.</param>
        /// <param name="token"></param>
        /// <returns></returns>
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