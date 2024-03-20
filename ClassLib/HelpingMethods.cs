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

        internal static async void Answer(Update update, ITelegramBotClient botClient)
        {
            if (currentAeroexpressTable.Count == 0 || DataIteraction.SelectedAeroexpressTableCsv.Count == 0)
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не нашлось значения.\n" +
                    "Что дальше?", replyMarkup: ShowButtons());
            else await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Успех, проведена выборка.\n" +
                "Что дальше?", replyMarkup: ShowButtons());
        }

        internal static string RemoveKovichka(string line)
        {
            string answer = new string(line.Where(sym => sym != '\"' && sym != ';').ToArray());
            return answer;
        }
    }
}

