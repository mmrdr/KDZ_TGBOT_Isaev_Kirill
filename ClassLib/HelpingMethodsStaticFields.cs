using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public partial class HelpingMethods
    {
        internal static char fileSeparator = Path.DirectorySeparatorChar;

        // To count files.
        public static int numberOfFile;

        // For method GetPathForFile.
        internal static string filePath;

        // To store information from a file.
        internal static List<AeroexpressTable> currentAeroexpressTable;

        // To alert the "Selection" status.
        internal static bool IsSelecting;

        // The field for which the selection will be made.
        internal static string curFieldToSelect;

        // The value for which the selection will be made.
        internal static string curValueToSelect;

        // The values for which the selection will be made(For method BothStationSelect).
        internal static string[] curValuesToSelect;

        internal static bool fileCorr;

        internal static AeroexpressTable Title = new AeroexpressTable("ID", "StationStart", "Line", "TimeStart", "StationEnd", "TimeEnd", "global_id");
        internal static AeroexpressTable SecondTitle = new AeroexpressTable("Локальный идентификатор", "Станция отправления", "Направление Аэроэкспресс", "Время отправления со станции", "Конечная станция направления Аэроэкспресс", "Время прибытия на конечную станцию направления Аэроэкспресс", "global_id");

    }
}
