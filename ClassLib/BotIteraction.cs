using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Smth;

namespace ClassLib
{
    /// <summary>
    /// Класс, реализующий взаимодействие с чатом бота.
    /// </summary>
    internal class BotIteraction
    {
        // Постоянно запоминаем, в каком потоке работаем.
        private static Stream lastCsvDownload, lastCsvUpload, lastJsonDownload, lastJsonUpload;

        /// <summary>
        /// Метод, загружающий файл из чата.
        /// </summary>
        /// <param name="botClient">Наш бот.</param>
        /// <param name="update">Текущее сообщение.</param>
        /// <returns>Поток, где находится файл.</returns>
        internal static async Task<Stream> DownloadFile(ITelegramBotClient botClient, Update update)
        {
            var fileId = update.Message.Document.FileId;
            var path = HelpingMethods.filePath + $"\\LastUserInput.csv";

            Stream fileStream = System.IO.File.Create(path);
            await botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
            fileStream.Close();
            return new FileStream(path, FileMode.Open);
        }

        /// <summary>
        /// Метод, определяющий какого типа файл скину пользователь. В дальнейшем - загружает его из чата и считывает из файла в AeroexpressTable.
        /// </summary>
        /// <param name="update">Текущее сообщение.</param>
        /// <returns></returns>
        internal static async Task DownloadData(Update update)
        {
            string fileName = update.Message.Document.FileName;

            if (fileName.EndsWith(".csv"))
            {
                lastCsvDownload = await DownloadFile(BotUpdates.botClient, update);
                var temporaryTableCsv = CSVProcessing.Read(lastCsvDownload);
            }
            else if (fileName.EndsWith(".json"))
            {
                lastJsonDownload = await DownloadFile(BotUpdates.botClient, update);
                var temporaryTableJson = JSONProcessing.Read(lastJsonDownload);
            }
            else
            {
                await BotUpdates.botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректное расширение файла");
            }
        }

        /// <summary>
        /// Метод, загружающий csv файл в чат.
        /// </summary>
        /// <param name="botClient">Наш бот.</param>
        /// <param name="update">Текущее сообщение.</param>
        /// <returns></returns>
        internal static async Task UploadCsvFile(ITelegramBotClient botClient, Update update)
        {
            lastCsvUpload = CSVProcessing.Write();
            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(lastCsvUpload, $"aeroexpress(edited).csv"));
        }

        /// <summary>
        /// Метод, загружающий json файл в чат.
        /// </summary>
        /// <param name="botClient">Наш бот.</param>
        /// <param name="update">Текущее сообщение.</param>
        /// <returns></returns>
        internal static async Task UploadJsonFile(ITelegramBotClient botClient, Update update)
        {
            lastJsonUpload = JSONProcessing.Write();
            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(lastJsonUpload, $"aeroexpress(edited).json"));
        }
    }
}
