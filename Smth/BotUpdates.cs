using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

namespace Smth
{
    internal class BotUpdates
    {
        public static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;
                            var user = message.From;
                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            var chat = message.Chat;
                            switch (message.Type)
                            {
                                
                                case MessageType.Text:
                                    {
                                        switch (message.Text)
                                        {
                                            case "/start":
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Приветствую Вас, {user.FirstName}\n" +
                                                $"Я бот, созданный для обработки и работы с файлами типа CSV и JSON"); ;
                                                    var replyKeyboard = new ReplyKeyboardMarkup(
                                                        new List<KeyboardButton[]>()
                                                        {
                                                            new KeyboardButton[]
                                                            {
                                                                new KeyboardButton("Загрузить CSV файл на обработку"),
                                                            },
                                                            new KeyboardButton[]
                                                            {
                                                                new KeyboardButton("Загрузить JSON файл на обработку"),
                                                            },
                                                            new KeyboardButton[]
                                                            {
                                                                new KeyboardButton("Произвести фильтрацию по одному из полей файла"),
                                                            },
                                                            new KeyboardButton[]
                                                            {
                                                                new KeyboardButton("Произвести выборку по одному из полей файла"),
                                                            },
                                                            new KeyboardButton[]
                                                            {
                                                                new KeyboardButton("Скачать обработанный файл в формате CSV или JSON"),
                                                            },
                                                        })
                                                        { ResizeKeyboard = true };

                                                    await botClient.SendTextMessageAsync(
                                                        chat.Id,
                                                        "Взаимодействовать со мной Вы можете через эти прекрасные кнопки",
                                                        replyMarkup: replyKeyboard);
                                                    return;
                                                }

                                            case "Что такое магическая битва?":
                                                {
                                                    var inlineKeyboard = new InlineKeyboardMarkup(
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithUrl("Держите трейлер",
                                                        "https://www.youtube.com/watch?v=gcgKUcJKxIs&ab_channel=TOHOanimation%E3%83%81%E3%83%A3%E3%83%B3%E3%83%8D%E3%83%AB")
                                                    });
                                                    await botClient.SendTextMessageAsync(
                                                        chat.Id,
                                                        "Держите ссылку на данное произведение",
                                                        replyMarkup: inlineKeyboard);
                                                    return;
                                                }
                                            case "Загрузить CSV файл на обработку":
                                                {
                                                    if (update.Message.Document == null)
                                                    {
                                                        await botClient.SendTextMessageAsync(chat.Id, "Скиньте Ваш CSV файл в чат");
                                                        var fileId = update.Message.Document.FileId;
                                                        var fileInfo = await botClient.GetFileAsync(fileId);
                                                        var filePath = fileInfo.FilePath;
                                                    }
                                                    return;
                                                }
                                            case "Загрузить JSON файл на обработку":
                                                {
                                                    return;
                                                }
                                            case "Произвести фильтрацию по одному из полей файла":
                                                {
                                                    return;
                                                }
                                            case "Произвести выборку по одному из полей файла":
                                                {
                                                    return;
                                                }
                                            case "Скачать обработанный файл в формате CSV или JSON":
                                                {
                                                    return;
                                                }

                                            default:
                                                {
                                                    await botClient.SendTextMessageAsync(
                                                        chat.Id,
                                                        "Я не chatgpt, не умею общаться на любые темы!",
                                                        replyToMessageId: message.MessageId);
                                                    return;
                                                }
                                        }
                                    }

                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!",
                                            replyToMessageId: message.MessageId);
                                        return;
                                    }
                            }
                        }            
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
