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
    internal class BotIteraction
    {
        private static Stream lastCsvDownload, lastCsvUpload, lastJsonDownload, lastJsonUpload;

        internal static async Task<Stream> DownloadFile(ITelegramBotClient botClient, Update update)
        {
            var fileId = update.Message.Document.FileId;
            var path = HelpingMethods.filePath + $"\\LastUserInput.csv";

            Stream fileStream = System.IO.File.Create(path);
            await botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
            fileStream.Close();
            return new FileStream(path, FileMode.Open);
        }

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

        internal static async Task UploadCsvFile(ITelegramBotClient botClient, Update update)
        {
            lastCsvUpload = CSVProcessing.Write();
            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(lastCsvUpload, $"aeroexpress(edited).csv"));
        }

        internal static async Task UploadJsonFile(ITelegramBotClient botClient, Update update)
        {
            lastJsonUpload = JSONProcessing.Write();
            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(lastJsonUpload, $"aeroexpress(edited).json"));
        }
    }
}
