using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    /// <summary>
    /// Класс, реализующий все взаимодействие с данными.
    /// </summary>
    internal class DataIteraction
    {

        internal static List<AeroexpressTable> SelectedAeroexpressTableCsv;

        /// <summary>
        /// Определяет по какому полю проводить выборку.
        /// </summary>
        /// <param name="message">Определяет по этому сообщению от пользователя.</param>
        internal static void TakeFieldToSelect(string message)
        {
            var answer = message.Split(" ");
            HelpingMethods.curFieldToSelect = answer[answer.Length - 1];
        }
        /// <summary>
        /// Определяет по какому значению данного поля проводить выборку.
        /// </summary>
        /// <param name="message">Определяет по этому сообщению от пользователя.</param>
        internal static void TakeValueToSelect(string message)
        {
            HelpingMethods.curValueToSelect = message;
        }
        /// <summary>
        /// Аналогично верхнему, но выбирает для метода BothStationSelect.
        /// </summary>
        /// <param name="message">Определяет по этому сообщению от пользователя.</param>
        internal static void TakeValuesToSelect(string message)
        {
            var countSep = 0;
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == ';') countSep++; 
            }

            if (countSep == 1) // Проверка корректности ввода пользователя.
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

        /// <summary>
        /// Проводит выборку, используя LINQ метод Where.
        /// </summary>
        /// <param name="fieldToSelect">По какому полю.</param>
        /// <param name="value">По какому значению.</param>
        internal static void StationSelection(string fieldToSelect, string value)
        {
            Logger.WriteLog(nameof(StationSelection), ConstStrings.startMethod); // Начало логгирования этого метода.

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

            Logger.WriteLog(nameof(StationSelection), ConstStrings.endMethod); // Конец. Остальные методы - аналогично.
        }

        /// <summary>
        /// Проводит выборку по двум параметрам, используя LINQ метод Where.
        /// </summary>
        /// <param name="values">По этим значениям.</param>
        internal static void BothStationSelect(string[] values)
        {
            Logger.WriteLog(nameof(BothStationSelect), ConstStrings.startMethod);

            SelectedAeroexpressTableCsv = new List<AeroexpressTable>(HelpingMethods.currentAeroexpressTable);
            SelectedAeroexpressTableCsv = SelectedAeroexpressTableCsv.Where(x => x.StationStart == values[0] && x.StationEnd == values[1]).ToList();
            if (SelectedAeroexpressTableCsv.Count == 0) return;
            HelpingMethods.currentAeroexpressTable = SelectedAeroexpressTableCsv;

            Logger.WriteLog(nameof(BothStationSelect), ConstStrings.endMethod);
        }

        /// <summary>
        /// Сортировка по TimeStart(По увеличению времени).
        /// </summary>
        internal static void SortTimeStart()
        {
            Logger.WriteLog(nameof(SortTimeStart), ConstStrings.startMethod);

            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeStart).ToArray();
            SelectedAeroexpressTableCsv = tables.ToList();
            HelpingMethods.currentAeroexpressTable = tables.ToList();

            Logger.WriteLog(nameof(SortTimeStart), ConstStrings.endMethod);
        }

        /// <summary>
        /// Сортировка по TimeEnd(По увеличению времени).
        /// </summary>
        internal static void SortTimeEnd()
        {
            Logger.WriteLog(nameof(SortTimeEnd), ConstStrings.startMethod);

            AeroexpressTable[] tables = new AeroexpressTable[HelpingMethods.currentAeroexpressTable.Count];
            HelpingMethods.currentAeroexpressTable.CopyTo(tables, 0);
            tables = tables.OrderBy(x => x.TimeEnd).ToArray();
            SelectedAeroexpressTableCsv = tables.ToList();
            HelpingMethods.currentAeroexpressTable = tables.ToList();

            Logger.WriteLog(nameof(SortTimeEnd), ConstStrings.endMethod);
        }
    }
}
