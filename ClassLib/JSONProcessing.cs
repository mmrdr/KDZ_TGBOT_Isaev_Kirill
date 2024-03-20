using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClassLib
{
    /// <summary>
    /// Класс, реализующий обработку файлов с расширением json.
    /// </summary>
    public class JSONProcessing
    {
        /// <summary>
        /// Метод записывает данные вида AeroexpressTable в json файл.
        /// </summary>
        /// <returns>Поток, куда был записан json файл.</returns>
        public static Stream Write()
        {
            Logger.WriteLog(nameof(Write), ConstStrings.startMethod);

            var title = HelpingMethods.Title;
            var secondTitle = HelpingMethods.SecondTitle;
            HelpingMethods.currentAeroexpressTable.Insert(0, title);
            HelpingMethods.currentAeroexpressTable.Insert(1, secondTitle);
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var json = JsonSerializer.Serialize(HelpingMethods.currentAeroexpressTable, jsonOptions);
            var writePath = HelpingMethods.filePath.Replace(".json", "").Replace(".csv", "") + $"\\BeautyOutput(edited({HelpingMethods.numberOfFile})).json";
            Console.WriteLine(writePath);
            Stream stream = File.Create(writePath);
            TextWriter oldOut = Console.Out;
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                Console.SetOut(writer);
                Console.Write(json);
            }
            Console.SetOut(oldOut);
            stream.Close();
            HelpingMethods.currentAeroexpressTable.RemoveAt(0);
            HelpingMethods.currentAeroexpressTable.RemoveAt(0);

            Logger.WriteLog(nameof(Write), ConstStrings.endMethod);

            return new FileStream(writePath, FileMode.Open);
        }

        /// <summary>
        /// Метод считывает данные из json файла.
        /// </summary>
        /// <param name="stream">Берет данные из этого потока.</param>
        /// <returns>Лист типа AeroexpressTable, с данными из json файла.</returns>
        public static List<AeroexpressTable> Read(Stream stream)
        {
            Logger.WriteLog(nameof(Read), ConstStrings.startMethod);

            try
            {
                HelpingMethods.fileCorr = true;
                AeroexpressTable title; // Это все махинации с первыми двумя строками файла, они будут не нужны во время действий с данными.
                AeroexpressTable secondTitle; // Просто удаляю две верхние строчки.
                TextReader oldIn = Console.In;
                var json = "";
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    Console.SetIn(streamReader);
                    json = streamReader.ReadToEnd();
                }
                var heroesFromJson = JsonSerializer.Deserialize<List<AeroexpressTable>>(json);
                title = heroesFromJson[0];
                secondTitle = heroesFromJson[1];
                if (title.ToString() == HelpingMethods.Title.ToString()) { heroesFromJson.RemoveAt(0); } // Это все махинации с первыми двумя строками файла, они будут не нужны во время действий с данными.
                if (secondTitle.ToString() == HelpingMethods.SecondTitle.ToString()) { heroesFromJson.RemoveAt(0); } // Просто удаляю две верхние строчки.
                HelpingMethods.currentAeroexpressTable = heroesFromJson;

                Logger.WriteLog(nameof(Read), ConstStrings.endMethod);

                return heroesFromJson;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                HelpingMethods.fileCorr = false;

                Logger.WriteExceptionLog(nameof(Read), ex);

                return new List<AeroexpressTable>(0);
            }
        }
        /// <summary>
        /// Превращает что-то в json строку.
        /// </summary>
        /// <returns>Строку, которую можно представить в виде json файла.</returns>
        public string ToJson()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }
}