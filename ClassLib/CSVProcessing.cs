using Newtonsoft.Json.Linq;
using Smth;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ClassLib
{
    public class CSVProcessing
    {
        internal static char csvSeparator = ';';

        internal static bool fileCorr;

        internal static int NumOfFields = 7;

        internal static string Title;

        internal static string Second;

        internal static int Count;

        internal static List<AeroexpressTable> SelectedAeroexpressTableCsv;

        private static bool CheckFileData(List<string> records, int length)
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

        internal static string CheckKovichka(string line)
        {
            string answer = new string(line.Where(sym => sym != '\"' && sym != ';').ToArray());
            return answer;
        }

        internal static List<AeroexpressTable> Read(Stream stream)
        {
            var temporaryAeroexpressTableCsv = new List<AeroexpressTable>();
            fileCorr = true;
            using (StreamReader file = new StreamReader(stream))
            {
                string? line = file.ReadLine();
                List<string> title = line.Split(csvSeparator).ToList();
                title.RemoveAt(title.Count - 1);
                if (line == null)
                {
                    Console.WriteLine("Некорректный файл");
                    fileCorr = false;
                    return new List<AeroexpressTable>(0);
                }
                if (title.Count != NumOfFields)
                {
                    Console.WriteLine("Некорректный файл");
                    fileCorr = false;
                    return new List<AeroexpressTable>(0);
                }
                Title = line;
                line = file.ReadLine();
                Second = line;
                List<string> records = new List<string>();
                List<string> finalLines = new List<string>();
                int iterCount = 0;
                while ((line = file.ReadLine()) != null)
                {
                    List<string> record = line.Split(csvSeparator).ToList();
                    record.RemoveAt(record.Count - 1);
                    if (NumOfFields != record.Count || !CheckFileData(record, NumOfFields))
                    {
                        Console.WriteLine("Некорректный файл");
                        fileCorr = false;
                        return new List<AeroexpressTable>(0);
                    }

                    finalLines.Add(line);

                    records = line.Split(csvSeparator).ToList();

                    temporaryAeroexpressTableCsv.Add(new AeroexpressTable(CheckKovichka(records[0]),
                        CheckKovichka(records[1]),
                        CheckKovichka(records[2]),
                        CheckKovichka(records[3]),
                        CheckKovichka(records[4]),
                        CheckKovichka(records[5]),
                        CheckKovichka(records[6])));
                }
            }
            HelpingMethods.currentAeroexpressTable = temporaryAeroexpressTableCsv;
            return temporaryAeroexpressTableCsv;
        }

        internal static Stream Write()
        {
            var writePath = HelpingMethods.filePath.Replace(".csv", "").Replace(".json", "") + $"(edited({HelpingMethods.numberOfFile})).csv";
            Stream stream  = System.IO.File.Create(writePath);
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(Title); writer.WriteLine(Second);
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
            var path = HelpingMethods.filePath + $"\\LastUserInput.csv";

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

        internal static void TakeFieldToSelect(string message)
        {
            var answer = message.Split(" ");
            HelpingMethods.curFieldToSelect = answer[answer.Length-1];
        }

        internal static void TakeValueToSelect(string message)
        {
            HelpingMethods.curValueToSelect = message;
        }

        internal static void TakeValuesToSelect(string message)
        {
            var countSep = 0;
            for (int i = 0; i < message.Length; i++)
            {
               if (message[i] == ';') countSep++;
            }

            if (countSep == 1)
            {
                var answer = message.Split(';');
                HelpingMethods.curValuesToSelect = new string[2];
                HelpingMethods.curValuesToSelect[0] = answer[0];
                HelpingMethods.curValuesToSelect[1] = answer[1];
            }
            else
            {
                Console.WriteLine("Некорректные данные\n" +
                "Выборка не будет осуществленна");
                HelpingMethods.curValuesToSelect = new string[2];
                HelpingMethods.curValuesToSelect[0] = "";
                HelpingMethods.curValuesToSelect[1] = "";
            }
        }

        internal static void StationSelection(string fieldToSelect, string value)
        {
            SelectedAeroexpressTableCsv = new List<AeroexpressTable>(HelpingMethods.currentAeroexpressTable);

            if (fieldToSelect.StartsWith("StationStart"))
            {
                SelectedAeroexpressTableCsv = SelectedAeroexpressTableCsv.Where(x => x.StationStart == value).ToList();
            }
            else if (fieldToSelect.StartsWith("StationEnd"))
            {
                SelectedAeroexpressTableCsv = SelectedAeroexpressTableCsv.Where(x => x.StationEnd == value).ToList();
            }
            if (SelectedAeroexpressTableCsv.Count == 0) return;
            HelpingMethods.currentAeroexpressTable = SelectedAeroexpressTableCsv;
        }

        internal static void BothStationSelect(string[] values)
        {
            SelectedAeroexpressTableCsv = new List<AeroexpressTable>(HelpingMethods.currentAeroexpressTable);
            SelectedAeroexpressTableCsv = SelectedAeroexpressTableCsv.Where(x => x.StationStart == values[0] && x.StationEnd == values[1]).ToList();
            if (SelectedAeroexpressTableCsv.Count == 0) return;
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