using Smth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ClassLib
{
    public class CSVProcessing
    {
        internal static string dataPath = "";

        internal static char csvSeparator = ';';

        internal static bool fileCorr;

        internal static int NumOfFields = 8;

        internal static string? Title;

        internal static string? SecondLine;

        internal static int Count;

        internal static List<AeroexpressTable> SelectedAeroexpressTableCsv;

        private static bool CheckFileData(string[] records, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (records[i] == "NA")
                {
                    continue;
                }

                switch (i)
                {
                    case 0:
                    case 3:
                    case 5:
                    case 6:
                        if (!uint.TryParse(records[i][1..^1], out uint X) && !DateTime.TryParse(records[i][1..^1], out DateTime x))
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        internal static bool CheckFileCor()
        {
            int count = 0;
            Count = count;
            using (StreamReader file = new StreamReader(BotUpdates.lastCsvDownload))
            {
                string? line = file.ReadLine();
                string[] title = line.Split(csvSeparator);
                if (line == null)
                {
                    return false;
                }
                if (title.Length != NumOfFields)
                {
                    return false;
                }
                line = file.ReadLine();
                while ((line = file.ReadLine()) != null)
                {
                    string[] record = line.Split(csvSeparator);
                    if (NumOfFields != record.Length || !CheckFileData(record, NumOfFields))
                    {
                        return false;
                    }
                    Count++;
                }
                return true;
            }
        }

        internal static AeroexpressTable[] Read(Stream stream)
        {
            fileCorr = true;
            if (!CheckFileCor())
            {
                Console.WriteLine("Некорректный файл");
                fileCorr = false;
                return new AeroexpressTable[0];
            }
            using (StreamReader file = new StreamReader(stream))
            {
                string? line = file.ReadLine();
                Title = line;
                line = file.ReadLine();
                SecondLine = line;
                string[] records = new string[Count];
                string[] finalLines = new string[Count];
                var temporaryAeroexpressTableCsv = new AeroexpressTable[Count];
                int iterCount = 0;
                while ((line = file.ReadLine()) != null)
                {
                    finalLines[iterCount] = line;

                    records = line.Split(csvSeparator);

                    temporaryAeroexpressTableCsv[iterCount++] = new AeroexpressTable(records[0],
                        records[1],
                        records[2],
                        records[3],
                        records[4],
                        records[5],
                        records[6]);
                }
                HelpingMethods.currentAeroexpressTable = temporaryAeroexpressTableCsv.ToList();
                return temporaryAeroexpressTableCsv;
            }
        }

        
        internal static Stream Write()
        {
            var writePath = dataPath + $"(edited).csv";
            Stream stream = System.IO.File.Create(writePath);
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (var table in HelpingMethods.currentAeroexpressTable)
                {
                    writer.WriteLine(table.ToString());
                }
            }
            stream.Close();
            return new FileStream(writePath, FileMode.Open);
        }

        internal static async Task<Stream> DownloadFile(ITelegramBotClient botClient, Update update)
        {
            var fileId = update.Message.Document.FileId;
            var path = dataPath + $"\\LastUserInput.csv";

            Stream fileStream = System.IO.File.Create(path);
            await botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
            fileStream.Close();
            return new FileStream(path, FileMode.Open);
        }

        internal static async Task UploadFile(ITelegramBotClient botClient, Update update, Stream stream)
        {
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
                                }
                            })
            { ResizeKeyboard = true };

            Message message = await botClient.SendDocumentAsync(update.Message.Chat.Id, InputFile.FromStream(stream, $"Aeroexpress.csv"),
                replyMarkup: replyKeyboard);
        }


        internal static void StationEndSelection()
        {
            string? selectThisField;
            while (true)
            {
                Console.Write("Введите значение для выборки: ");
                selectThisField = Console.ReadLine();
                if (string.IsNullOrEmpty(selectThisField))
                {
                    Console.WriteLine("Вы ввели пустое значение");
                    Console.Write("Введите значение для выборки: ");
                    selectThisField = Console.ReadLine();
                }
                break;
            }
            SelectedAeroexpressTableCsv = new List<AeroexpressTable>();
            int count = 0;
            foreach (var elem in HelpingMethods.currentAeroexpressTable)
            {
                if (elem.StationEnd[1..^1] == selectThisField)
                {
                    SelectedAeroexpressTableCsv[count++] = elem;
                }
            }
            if (count == 0)
            {
                Console.WriteLine("По этому значению нет ничего в расписании");
                return;
            }
            HelpingMethods.currentAeroexpressTable = SelectedAeroexpressTableCsv;
        }

        internal static void StationStartAndEndSelection()
        {
            string? selectThisField;
            while (true)
            {
                Console.Write("Введите значение для выборки: ");
                selectThisField = Console.ReadLine();
                if (string.IsNullOrEmpty(selectThisField))
                {
                    Console.WriteLine("Вы ввели пустое значение");
                    Console.Write("Введите значение для выборки: ");
                    selectThisField = Console.ReadLine();
                }
                break;
            }
            SelectedAeroexpressTableCsv = new List<AeroexpressTable>();
            int count = 0;
            foreach (var elem in HelpingMethods.currentAeroexpressTable)
            {
                if (elem.StationEnd[1..^1] == selectThisField || elem.StationStart[1..^1] == selectThisField)
                {
                    SelectedAeroexpressTableCsv[count++] = elem;
                }
            }
            if (count == 0)
            {
                Console.WriteLine("По этому значению нет ничего в расписании");
                return;
            }
            HelpingMethods.currentAeroexpressTable = SelectedAeroexpressTableCsv;
        }

        internal static void SortTimeStart()
        {
            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeStart).ToArray();
            HelpingMethods.currentAeroexpressTable = tables.ToList();
        }

        internal static void SortTimeEnd()
        {
            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeEnd).ToArray();
            HelpingMethods.currentAeroexpressTable = tables.ToList();
        }
    }
}