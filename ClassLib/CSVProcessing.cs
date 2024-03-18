using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ClassLib
{
    public class CSVProcessing
    {
        internal static char csvSeparator = ';';


        // Количество полей в каждой записи.
        internal static int NumOfFields = 8;

        // Первая строка файла.
        internal static string? Title;
        // Вторая строка файла.
        internal static string? SecondLine;
        // Количество записей.
        internal static int Count;

        public static List<AeroexpressTable> SelectedAeroexpressTableCsv;

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

        /// <summary>
        /// Финальный метод проверки файла. Здесь проверяется само наличие записей в файле, его соответствие структуре.
        /// На каждую строку вызывается метод CheckFileData
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static bool CheckFileCor()
        {
            int count = 0;
            Count = count;
            StreamReader file = new StreamReader(HelpingMethods.filePath);
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
            file.Close();
            return true;
        }

        /// <summary>
        /// Метод считывет файл и переносит записи в созданный экземпляр класса Table.
        /// </summary>
        /// <returns></returns>
        internal static AeroexpressTable[] Read()
        {
            if (!CheckFileCor())
            {
                Console.WriteLine("Некорректный файл");
                return new AeroexpressTable[0];
            }
            StreamReader file = new StreamReader(HelpingMethods.filePath);
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
            file.Close();
            HelpingMethods.currentAeroexpressTableCsv = temporaryAeroexpressTableCsv.ToList();
            return temporaryAeroexpressTableCsv;
        }

        
        internal static void Write(List<AeroexpressTable> aeroexpressTables)
        {
            using StreamWriter streamWriter = new StreamWriter(HelpingMethods.filePath, false);
            foreach (var table in HelpingMethods.currentAeroexpressTableCsv)
            {
                streamWriter.WriteLine(table.ToString());
            }
            streamWriter.Close();
        }

        internal static async Task<string> DownloadFile(ITelegramBotClient botClient, Update update, string filePath)
        {
            var fileId = update.Message.Document.FileId;
            var path = $"{filePath}\\LastUserInput.csv";

            await using Stream fileStream = System.IO.File.Create(path);
            await botClient.GetInfoAndDownloadFileAsync(fileId,fileStream);
            fileStream.Close();
            return path;
        }
    }
}