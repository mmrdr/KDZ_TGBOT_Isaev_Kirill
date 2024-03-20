using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    /// <summary>
    /// Эта часть partial-класса HelpingMethods, реализует все статические поля, необходимые в различных методах из других классов.
    /// </summary>
    public partial class HelpingMethods
    {
        internal static string LogPath;

        public static int numberOfFile;

        internal static string filePath;

        internal static List<AeroexpressTable> currentAeroexpressTable;

        internal static bool IsSelecting;

        internal static string curFieldToSelect;

        internal static string curValueToSelect;

        internal static string[] curValuesToSelect;

        internal static bool fileCorr;

        internal static AeroexpressTable Title = new AeroexpressTable("ID", "StationStart", "Line", "TimeStart", "StationEnd", "TimeEnd", "global_id");
        internal static AeroexpressTable SecondTitle = new AeroexpressTable("Локальный идентификатор", "Станция отправления", "Направление Аэроэкспресс", "Время отправления со станции", "Конечная станция направления Аэроэкспресс", "Время прибытия на конечную станцию направления Аэроэкспресс", "global_id");

    }
}
