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
    public partial class HelpingMethods
    {
        internal static List<AeroexpressTable> currentAeroexpressTable;

        public static int numberOfFile;

        internal static string filePath;

        internal static bool IsSelecting;

        internal static string curFieldToSelect;

        internal static string curValueToSelect;

        internal static string[] curValuesToSelect;

        /// <summary>
        /// Check for correctness and return user's input.
        /// </summary>
        /// <returns>So useful ConsoleKey from user's input.</returns>
        internal static ConsoleKey ItemForSorting()
        {
            var key = Console.ReadKey().Key;
            while (key != ConsoleKey.D1 && key != ConsoleKey.D2 && key != ConsoleKey.D3 && key != ConsoleKey.D4
                && key != ConsoleKey.D5 && key != ConsoleKey.D6)
            {
                Console.WriteLine();
                PrintWithColor("Incorrect input, try again: ", ConsoleColor.Red);
                key = Console.ReadKey().Key;
            }
            return key;
        }

        internal static void GetPathForFile()
        {
            Process process = Process.GetCurrentProcess();
            filePath = process.MainModule.FileName;
            filePath = Path.GetDirectoryName(filePath);
            filePath = Path.GetDirectoryName(filePath);
            filePath = Path.GetDirectoryName(filePath);
            filePath = Path.GetDirectoryName(filePath);
            filePath += "\\Data";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        internal static async Task DownloadData(Update update)
        {
            GetPathForFile();
            string fileName = update.Message.Document.FileName;

            if (fileName.EndsWith(".csv"))
            {
                BotUpdates.lastCsvDownload = await CSVProcessing.DownloadFile(BotUpdates.botClient, update);
                var temporaryTableCsv = CSVProcessing.Read(BotUpdates.lastCsvDownload);
            }
            else if (fileName.EndsWith(".json"))
            {
                BotUpdates.lastJsonDownload = await CSVProcessing.DownloadFile(BotUpdates.botClient, update);
                var temporaryTableJson = JSONProcessing.Read(BotUpdates.lastJsonDownload);
            }
            else
            {
                await BotUpdates.botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректное расширение файла");
            }
        }

        internal static async Task UploadCsvFile(ITelegramBotClient botClient, Update update)
        {
            BotUpdates.lastCsvUpload = CSVProcessing.Write();
            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(BotUpdates.lastCsvUpload, $"aeroexpress(edited).csv"));
        }

        internal static async Task UploadJsonFile(ITelegramBotClient botClient, Update update)
        {
            BotUpdates.lastJsonUpload = JSONProcessing.Write();
            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(BotUpdates.lastJsonUpload, $"aeroexpress(edited).json"));
        }

        internal static AeroexpressTable ConvertToAeroexpress(string title)
        {
            string[] table = title.Split(CSVProcessing.csvSeparator);
            AeroexpressTable aeroexpressTable = new AeroexpressTable(CSVProcessing.CheckKovichka(table[0]),
                CSVProcessing.CheckKovichka(table[1]),
                CSVProcessing.CheckKovichka(table[2]),
                CSVProcessing.CheckKovichka(table[3]),
                CSVProcessing.CheckKovichka(table[4]),
                CSVProcessing.CheckKovichka(table[5]),
                CSVProcessing.CheckKovichka(table[6]));
            return aeroexpressTable;
        }
    }
}

