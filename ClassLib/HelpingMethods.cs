using Microsoft.VisualBasic;
using Smth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ClassLib
{
    /// <summary>
    /// Основная часть partial-класса HelpingMethods. Реализует все методы, которые как-либо помогают корректно выстраивать логику программы.
    /// </summary>
    public partial class HelpingMethods
    {
        /// <summary>
        /// Создает путь к папке с данными и логами.
        /// </summary>
        public static void GetPathForFile()
        {
            Process process = Process.GetCurrentProcess();
            var tempPath = process.MainModule.FileName;
            for (int i = 0;i < 5; i++)
            {
                tempPath = Path.GetDirectoryName(tempPath);
            }
            filePath = tempPath + "\\Data";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            LogPath = tempPath + "\\var";
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }
        /// <summary>
        /// Конвертирует строку в экземпляр класса AeroexpressTable.
        /// </summary>
        /// <param name="title">Эту строку конвертирует.</param>
        /// <returns>Экземпляр класса AeroexpressTable.</returns>
        internal static AeroexpressTable ConvertToAeroexpress(string title)
        {
            string[] table = title.Split(CSVProcessing.csvSeparator);
            AeroexpressTable aeroexpressTable = new AeroexpressTable(RemoveKovichka(table[0]),
                RemoveKovichka(table[1]),
                RemoveKovichka(table[2]),
                RemoveKovichka(table[3]),
                RemoveKovichka(table[4]),
                RemoveKovichka(table[5]),
                RemoveKovichka(table[6]));
            return aeroexpressTable;
        }

        /// <summary>
        /// Красивый вывод для пользователя.
        /// </summary>
        /// <param name="update">Текущее сообщение.</param>
        /// <param name="botClient">Позволяет взаимодействовать с ботом.</param>
        internal static async void Answer(Update update, ITelegramBotClient botClient)
        {
            if (currentAeroexpressTable.Count == 0 || DataIteraction.SelectedAeroexpressTableCsv.Count == 0)
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не нашлось значения.\n" +
                    "Что дальше?", replyMarkup: ShowButtons());
            else await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Успех, проведена выборка.\n" +
                "Что дальше?", replyMarkup: ShowButtons());
        }

        /// <summary>
        /// Метод убирает лишние кавычки.
        /// </summary>
        /// <param name="line">Убирает из этой строки.</param>
        /// <returns>Строку без кавычек.</returns>
        internal static string RemoveKovichka(string line)
        {
            string answer = new string(line.Where(sym => sym != '\"' && sym != ';').ToArray());
            return answer;
        }

        /// <summary>
        /// Проверяет пользователя на корректность ввода сообщения.
        /// </summary>
        /// <param name="update">Текущее сообщение пользователя</param>
        /// <returns>Корректно или нет.</returns>
        internal static bool CheckMessage(Update update)
        {
            if (update.Message.Text != ConstStrings.funcSelStationStart && update.Message.Text != ConstStrings.funcSelStationEnd && update.Message.Text != ConstStrings.funcSelBoth
                    && update.Message.Text != ConstStrings.funcSortTimeStart && update.Message.Text != ConstStrings.funcSortTimeEnd
                    && update.Message.Text != ConstStrings.funcUploadCsv && update.Message.Text != ConstStrings.funcUploadJson)
            {
                return true;
            }
            return false;
        }
    }
}

