using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    internal class DataIteraction
    {

        internal static List<AeroexpressTable> SelectedAeroexpressTableCsv;

        internal static void TakeFieldToSelect(string message)
        {
            var answer = message.Split(" ");
            HelpingMethods.curFieldToSelect = answer[answer.Length - 1];
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
            Logger.WriteLog(nameof(StationSelection), ConstStrings.startMethod);

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

            Logger.WriteLog(nameof(StationSelection), ConstStrings.endMethod);
        }

        internal static void BothStationSelect(string[] values)
        {
            Logger.WriteLog(nameof(BothStationSelect), ConstStrings.startMethod);

            SelectedAeroexpressTableCsv = new List<AeroexpressTable>(HelpingMethods.currentAeroexpressTable);
            SelectedAeroexpressTableCsv = SelectedAeroexpressTableCsv.Where(x => x.StationStart == values[0] && x.StationEnd == values[1]).ToList();
            if (SelectedAeroexpressTableCsv.Count == 0) return;
            HelpingMethods.currentAeroexpressTable = SelectedAeroexpressTableCsv;

            Logger.WriteLog(nameof(BothStationSelect), ConstStrings.endMethod);
        }

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
