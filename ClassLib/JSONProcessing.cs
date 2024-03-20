﻿using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClassLib
{
    public class JSONProcessing
    {

        public static List<AeroexpressTable> SelectedAeroexpressTableJson;

        /// <summary>
        /// This method serialize data to some file.
        /// </summary>
        /// <param name="heroes">This is list, that gives method data to serialize it to the file.</param>
        /// <param name="filePath">This path refer to file, that will take data from method.</param>
        public static Stream Write()
        {
            var title = HelpingMethods.ConvertToAeroexpress(CSVProcessing.Title);
            var secondTitle = HelpingMethods.ConvertToAeroexpress(CSVProcessing.Second);
            HelpingMethods.currentAeroexpressTable.Insert(0, title);
            HelpingMethods.currentAeroexpressTable.Insert(1, secondTitle);
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var json = JsonSerializer.Serialize(HelpingMethods.currentAeroexpressTable, jsonOptions);
            var writePath = HelpingMethods.filePath.Replace(".json", "").Replace(".csv", "") + $"(edited({HelpingMethods.numberOfFile})).json";
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
            return new FileStream(writePath, FileMode.Open);
        }

        /// <summary>
        /// This method deserialize data from json file to C# object(Hero,Units).
        /// </summary>
        /// <param name="filePath">This path refer to file, that will gives data to this method.</param>
        /// <returns>List with json's data.</returns>
        public static List<AeroexpressTable> Read(Stream stream)
        {
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
            HelpingMethods.currentAeroexpressTable = heroesFromJson;
            if (HelpingMethods.currentAeroexpressTable[0] == (title = HelpingMethods.ConvertToAeroexpress(CSVProcessing.Title))) HelpingMethods.currentAeroexpressTable.RemoveAt(0);
            if (HelpingMethods.currentAeroexpressTable[0] == (secondTitle = HelpingMethods.ConvertToAeroexpress(CSVProcessing.Title))) HelpingMethods.currentAeroexpressTable.RemoveAt(0);
            return heroesFromJson;
        }

        internal static void SortTimeStartJson()
        {
            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeStart).ToArray();
            HelpingMethods.currentAeroexpressTable = tables.ToList();
        }

        internal static void SortTimeEndJson()
        {
            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeEnd).ToArray();
            HelpingMethods.currentAeroexpressTable = tables.ToList();
        }

        public string ToJson()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }
}