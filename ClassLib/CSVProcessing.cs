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

        internal static bool CheckFileCor()
        {
            int count = 0;
            Count = count;
            using (StreamReader file = new StreamReader(HelpingMethods.filePath))
            {
                string? line = file.ReadLine();
                List<string> title = line.Split(csvSeparator).ToList();
                title.RemoveAt(title.Count-1);
                if (line == null)
                {
                    return false;
                }
                if (title.Count != NumOfFields)
                {
                    return false;
                }
                line = file.ReadLine();
                while ((line = file.ReadLine()) != null)
                {
                    List<string> record = line.Split(csvSeparator).ToList();
                    record.RemoveAt(record.Count-1);
                    if (NumOfFields != record.Count || !CheckFileData(record, NumOfFields))
                    {
                        return false;
                    }
                    Count++;
                }
                return true;
            }
        }

        internal static string CheckKovichka(string line)
        {
            string answer = new string(line.Where(sym => sym != '\"' && sym != ';').ToArray());
            return answer;
        }

        internal static AeroexpressTable[] Read()
        {
            fileCorr = true;
            if (!CheckFileCor())
            {
                Console.WriteLine("Некорректный файл");
                fileCorr = false;
                return new AeroexpressTable[0];
            }
            using (StreamReader file = new StreamReader(HelpingMethods.filePath))
            {
                string? line = file.ReadLine();
                Title = line;
                line = file.ReadLine();
                Second = line;
                string[] records = new string[Count];
                string[] finalLines = new string[Count];
                var temporaryAeroexpressTableCsv = new AeroexpressTable[Count];
                int iterCount = 0;
                while ((line = file.ReadLine()) != null)
                {
                    finalLines[iterCount] = line;

                    records = line.Split(csvSeparator);

                    temporaryAeroexpressTableCsv[iterCount++] = new AeroexpressTable(CheckKovichka(records[0]),
                        CheckKovichka(records[1]),
                        CheckKovichka(records[2]),
                        CheckKovichka(records[3]),
                        CheckKovichka(records[4]),
                        CheckKovichka(records[5]),
                        CheckKovichka(records[6]));
                }
                HelpingMethods.currentAeroexpressTable = temporaryAeroexpressTableCsv.ToList();
                return temporaryAeroexpressTableCsv;
            }
        }

        internal static string Write()
        {
            var writePath = HelpingMethods.filePath.Replace(".csv", "").Replace(".json", "") + $"(edited({HelpingMethods.numberOfFile})).csv";
            using (StreamWriter writer = new StreamWriter(writePath))
            {
                writer.WriteLine(Title); writer.WriteLine(Second);
                foreach (var table in HelpingMethods.currentAeroexpressTable)
                {
                    writer.WriteLine(table.ToString());
                }
                return writePath;
            }
        }

        internal static async Task<string> DownloadFile(ITelegramBotClient botClient, Update update)
        {
            var fileId = update.Message.Document.FileId;
            var path = HelpingMethods.filePath + $"\\LastUserInput.csv";

            Stream fileStream = System.IO.File.Create(path);
            await botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
            fileStream.Close();
            HelpingMethods.filePath = path;
            return path;
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

        internal static void StationStartSelectionCsv()
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
                if (elem.StationStart[1..^1] == selectThisField)
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

        internal static void StationEndSelectionCsv()
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

        internal static void StationStartAndEndSelectionCsv()
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

        internal static void SortTimeStartCsv()
        {
            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeStart).ToArray();
            HelpingMethods.currentAeroexpressTable = tables.ToList();
        }

        internal static void SortTimeEndCsv()
        {
            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeEnd).ToArray();
            HelpingMethods.currentAeroexpressTable = tables.ToList();
        }
    }
}