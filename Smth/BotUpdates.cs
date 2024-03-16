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
                                        
                                        if (message.Text == "/start")
                                        {                                           
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Выбери клавиатуру:\n" +
                                                "/inline\n" +
                                                "/reply\n");
                                            return;
                                        }

                                        if (message.Text == "/inline")
                                        {
                                            
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                new List<InlineKeyboardButton[]>() 
                                                {


                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Загрузить CSV файл на обработку", "button0")
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Произвести выборку по одному из полей файла", "button2"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Произвести фильтрацию по одному из полей файла", "button3"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Скачать обработанный файл в формате CSV или JSON", "button4"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Загрузить JSON файл на обработку", "button5"),
                                        },
                                                });

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Это inline клавиатура!",
                                                replyMarkup: inlineKeyboard);

                                            return;
                                        }

                                        if (message.Text == "/reply")
                                        {

                                            var replyKeyboard = new ReplyKeyboardMarkup(
                                                new List<KeyboardButton[]>()
                                                {


                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Загрузить CSV файл на обработку"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Произвести выборку по одному из полей файла"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Произвести фильтрацию по одному из полей файла"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Скачать обработанный файл в формате CSV или JSON"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Загрузить JSON файл на обработку"),
                                        },
                                                })
                                            {ResizeKeyboard = true };

                                            
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Это reply клавиатура!",
                                                replyMarkup: replyKeyboard);

                                            return;
                                        }

                                        return;
                                    }
                                
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!");
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
