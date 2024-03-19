using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassLib
{
    public class AeroexpressTable
    {
        // Эти поля соответствуют записям из строки.
        private string id;
        private string stationStart;
        private string line;
        private string timeStart;
        private string stationEnd;
        private string timeEnd;
        private string globalId;

        [JsonPropertyName("ID")]
        public string Id { get { return id; } set { id = value; } }

        [JsonPropertyName("StationStart")]
        public string StationStart { get { return stationStart; } set { stationStart = value; } }

        [JsonPropertyName("Line")]
        public string Line { get { return line; } set { line = value; } }

        [JsonPropertyName("TimeStart")]
        public string TimeStart { get { return timeStart; } set { timeStart = value; } }

        [JsonPropertyName("StationEnd")]
        public string StationEnd { get { return stationEnd; } set { stationEnd = value; } }

        [JsonPropertyName("TimeEnd")]
        public string TimeEnd { get { return timeEnd; } set { timeEnd = value; } }

        [JsonPropertyName("globalId")]
        public string GlobalId { get { return globalId; } set { globalId = value; } }


        public AeroexpressTable(string id,
        string stationstart,
        string line,
        string timestart,
        string stationend,
        string timeend,
        string globalid)
        {
            this.id = id;
            stationStart = stationstart;
            this.line = line;
            timeStart = timestart;
            stationEnd = stationend;
            timeEnd = timeend;
            globalId = globalid;
        }

        // Переопределяем метод ToString(), чтобы успешно записывать информацию в новый файл.
        public override string ToString()
        {
            string result = $"\"{id}\";" + $"\"{stationStart}\";" + $"\"{line}\";" + $"\"{timeStart}\";"
            + $"\"{stationEnd}\";" + $"\"{timeEnd}\";" + $"\"{globalId}\";";
            return result;
        }
    }
}