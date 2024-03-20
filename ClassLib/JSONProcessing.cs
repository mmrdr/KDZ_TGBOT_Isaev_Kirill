using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClassLib
{
    public class JSONProcessing
    {
        /// <summary>
        /// This method serialize data to some file.
        /// </summary>
        /// <param name="heroes">This is list, that gives method data to serialize it to the file.</param>
        /// <param name="filePath">This path refer to file, that will take data from method.</param>
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
        /// This method deserialize data from json file to C# object(Hero,Units).
        /// </summary>
        /// <param name="filePath">This path refer to file, that will gives data to this method.</param>
        /// <returns>List with json's data.</returns>
        public static List<AeroexpressTable> Read(Stream stream)
        {
            Logger.WriteLog(nameof(Read), ConstStrings.startMethod);

            try
            {
                HelpingMethods.fileCorr = true;
                AeroexpressTable title;
                AeroexpressTable secondTitle;
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
                if (title.ToString() == HelpingMethods.Title.ToString()) { heroesFromJson.RemoveAt(0); }
                if (secondTitle.ToString() == HelpingMethods.SecondTitle.ToString()) { heroesFromJson.RemoveAt(0); }
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

        public string ToJson()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }
}