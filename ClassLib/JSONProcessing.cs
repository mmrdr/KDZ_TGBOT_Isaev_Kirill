using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public static void SerializeToJsonFile(List<AeroexpressTable> heroes, string filePath)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(heroes, jsonOptions);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// This method deserialize data from json file to C# object(Hero,Units).
        /// </summary>
        /// <param name="filePath">This path refer to file, that will gives data to this method.</param>
        /// <returns>List with json's data.</returns>
        public static List<AeroexpressTable> DeserializeFromJsonFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var heroesFromJson = JsonSerializer.Deserialize<List<AeroexpressTable>>(json);
            HelpingMethods.currentAeroexpressTableJson = heroesFromJson;
            return heroesFromJson;
        }


        /// <summary>
        /// This method will sort objects in list with data.
        /// </summary>
        /// <returns>Sorted list(by any choosen field).</returns>
        public static List<AeroexpressTable> Sorting()
        {
            if (HelpingMethods.currentAeroexpressTableJson == null)
            {
                HelpingMethods.PrintWithColor("No data error, try again", ConsoleColor.Red);
                return new List<AeroexpressTable>(0);
            }
            //HelpingMethods.WelcomingForSorting(); Наверное не нужно раз у нас интерфейс пользователя именно в тг боте.
            var tempSelectedUsers = new AeroexpressTable[HelpingMethods.currentAeroexpressTableJson.Count()];
            switch (HelpingMethods.ItemForSorting()) // Это будет взаимодействовать в зависимости от кнопки пользователя в боте.
            {
                case ConsoleKey.D1:
                    Console.Clear();
                    HelpingMethods.currentAeroexpressTableJson.CopyTo(tempSelectedUsers, 0);
                    tempSelectedUsers = tempSelectedUsers.OrderBy(x => x.TimeStart).ToArray();
                    break;
                case ConsoleKey.D2:
                    Console.Clear();
                    HelpingMethods.currentAeroexpressTableJson.CopyTo(tempSelectedUsers, 0);
                    tempSelectedUsers = tempSelectedUsers.OrderBy(x => x.TimeEnd).ToArray();
                    break;
            }
            SelectedAeroexpressTableJson = new List<AeroexpressTable>(tempSelectedUsers.Length);
            foreach (var user in tempSelectedUsers)
            {
                SelectedAeroexpressTableJson.Add(user);
            }
            HelpingMethods.currentAeroexpressTableJson = SelectedAeroexpressTableJson;
            return SelectedAeroexpressTableJson;
        }





        public string ToJson()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }
}