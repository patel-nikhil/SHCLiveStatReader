using System;
using System.Collections.Generic;
using System.Text;

namespace SHC
{
    class Data
    {
        private Dictionary<String, Dictionary<String, Object>> dataset;
        private String title;

        public Data(String title, Dictionary<String, Dictionary<String, Object>> items)
        {
            this.title = title;
            this.dataset = items;
        }

        public Dictionary<string, Dictionary<String, Object>> GameData()
        {
            return dataset;
        }

        public String GetTitle()
        {
            return this.title;
        }

        public override String ToString()
        {
            if ((String)this.dataset["Name"]["Value"] == "") return "";
                String str = '"' + this.title + '"';
            str += ":\n";
            str += "{\n";
            foreach (KeyValuePair<String, Dictionary<String, Object>> dataItem in dataset)
            {
                Object rawData = dataItem.Value["Value"];

                if (rawData is Int32)
                {
                    str += '"' + dataItem.Key.ToString() + '"' + ":" + (((Int32)rawData < -1) ? (Int32)rawData + 30 : rawData) + ",\n";
                }
                else
                {
                    str += '"' + dataItem.Key.ToString().Replace("'", "\\'").Replace("\"", "\\\"") + '"' + ":" + '"' + ((String)rawData).Replace("'", "\\'").Replace("\"", "\\\"") + '"' + ",\n";
                }


            }
            str = str.Remove(str.Length - 2, 1);
            str += "}";
            return str;
        }
    }
}
