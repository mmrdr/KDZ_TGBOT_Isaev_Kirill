using Newtonsoft.Json.Linq;
using Smth;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ClassLib
{
    /// <summary>
    /// Класс, реализующий обработку файлов с расширением csv.
    /// </summary>
    public class CSVProcessing
    {
        // Ниже все важные статические поля, необходимые для корректной работы методов.

        internal static char csvSeparator = ';';  

        internal static int NumOfFields = 7;

        private static string Title = HelpingMethods.Title.ToString();

        private static string Second = HelpingMethods.SecondTitle.ToString();

        /// <summary>
        /// Метод, проверяющий каждую строку из csv файла.
        /// </summary>
        /// <param name="records">Строка с данными.</param>
        /// <param name="length">Сколько данных в строке проверить.</param>
        /// <returns>Корректны данные или нет.</returns>
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
        /// <summary>
        /// Считывает данные из csv файла.
        /// </summary>
        /// <param name="stream">Использует этот поток.</param>
        /// <returns>Данные из csv файла в виде листа типа AeroexpressTable.</returns>
        internal static List<AeroexpressTable> Read(Stream stream)
        {
            Logger.WriteLog(nameof(Read), ConstStrings.startMethod);

            var temporaryAeroexpressTableCsv = new List<AeroexpressTable>();
            HelpingMethods.fileCorr = true;
            using (StreamReader file = new StreamReader(stream))
            {
                string? line = file.ReadLine();
                List<string> title = line.Split(csvSeparator).ToList();
                title.RemoveAt(title.Count - 1); // В строке есть лишнее 8 поле - пустой символ. Удаляем.
                if (line == null)
                {
                    Console.WriteLine("Некорректный файл");
                    HelpingMethods.fileCorr = false;
                    return new List<AeroexpressTable>(0);
                }
                if (title.Count != NumOfFields)
                {
                    Console.WriteLine("Некорректный файл");
                    HelpingMethods.fileCorr = false;
                    return new List<AeroexpressTable>(0);
                }
                Title = line; // Заголовок файла не понадобится.
                line = file.ReadLine();
                Second = line; // Эта строка тоже не содержит важных данных, убираем в отдельную переменную.
                List<string> records = new List<string>();
                List<string> finalLines = new List<string>();
                while ((line = file.ReadLine()) != null)
                {
                    List<string> record = line.Split(csvSeparator).ToList();
                    record.RemoveAt(record.Count - 1); 
                    if (NumOfFields != record.Count || !CheckFileData(record, NumOfFields))
                    {
                        Console.WriteLine("Некорректный файл");
                        HelpingMethods.fileCorr = false;
                        return new List<AeroexpressTable>(0);
                    }

                    finalLines.Add(line);

                    records = line.Split(csvSeparator).ToList();

                    temporaryAeroexpressTableCsv.Add(new AeroexpressTable(HelpingMethods.RemoveKovichka(records[0]),
                        HelpingMethods.RemoveKovichka(records[1]),
                        HelpingMethods.RemoveKovichka(records[2]),
                        HelpingMethods.RemoveKovichka(records[3]),
                        HelpingMethods.RemoveKovichka(records[4]),
                        HelpingMethods.RemoveKovichka(records[5]),
                        HelpingMethods.RemoveKovichka(records[6])));
                }
            }
            HelpingMethods.currentAeroexpressTable = temporaryAeroexpressTableCsv;

            Logger.WriteLog(nameof(Read), ConstStrings.endMethod);

            return temporaryAeroexpressTableCsv;
        }

        /// <summary>
        /// Записывает данные из AeroexressTable в csv файл.
        /// </summary>
        /// <returns>Поток, куда файл записали.</returns>
        internal static Stream Write()
        {
            Logger.WriteLog(nameof(Write), ConstStrings.startMethod);

            // Пояснение к ужасу снизу: 1). Чтобы не дублировалось расширение файла, удаляю его с помощью метода Replace.
            // 2). Чтобы работал поток, файлы должны быть с разным названием, это реализует переменная numberOfFile.
            var writePath = HelpingMethods.filePath.Replace(".csv", "").Replace(".json", "") + $"\\BeautyOutput(edited({HelpingMethods.numberOfFile})).csv"; 
            Console.WriteLine(writePath);
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

            Logger.WriteLog(nameof(Write), ConstStrings.endMethod);

            return new FileStream(writePath, FileMode.Open);
        }
        /// <summary>
        /// Выгружает файл в чат с ботом.
        /// </summary>
        /// <param name="botClient">Наш бот.</param>
        /// <param name="update">Текущее сообщение.</param>
        /// <param name="stream">Поток, откуда будет взят файл.</param>
        /// <returns></returns>
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

    }
}