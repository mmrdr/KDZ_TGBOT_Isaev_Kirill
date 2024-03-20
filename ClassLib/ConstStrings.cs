using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    /// <summary>
    /// Класс, реализующий все сообщения для пользователя.
    /// </summary>
    internal class ConstStrings
    {
        internal const string startMethod = "начал работу";
        internal const string endMethod = "закончил работу";

        internal const string funcSelStationStart = "Выборка по StationStart";
        internal const string funcSelStationEnd = "Выборка по StationEnd";
        internal const string funcSelBoth = "Выборка по StationStart,StationEnd";
        internal const string funcSortTimeStart = "Сортировка по TimeStart(в порядке увеличения времени)";
        internal const string funcSortTimeEnd = "Сортировка по TimeEnd(в порядке увеличения времени)";
        internal const string funcUploadCsv = "Выгрузить файл в формате CSV";
        internal const string funcUploadJson = "Выгрузить файл в формате JSON";

        internal const string rage = "Для начала - прикрепи файл!";

        internal const string selectChoose = "Введите значение для выборки\n" +
                            "Параметр должен точно совпадать с параметром из файла";
    }
}
