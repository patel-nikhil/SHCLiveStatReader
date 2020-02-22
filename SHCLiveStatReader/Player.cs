using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHC
{
    class Player
    {
        public Dictionary<String, Dictionary<String, String>> Data { get; }
        int Number { get; }
        int Score { get;  }

        bool IsAlive
        {
            get
            {
                return Reader.TestZero(0x024BA918 + 4 * (Number - 1), 4);
            }
        }

        public Player(int number, Dictionary<String, Dictionary<String, String>> data)
        {
            this.Number = number;
            this.Data = data;
        }

        public String Update()
        {
            Int32 gold = 0;
            Int32 weightedUnits = 0;
            Int32 resources = 0;

            Dictionary<String, String> jsonDict = new Dictionary<string, string>();
            jsonDict["PlayerNumber"] = this.Number.ToString();
            jsonDict["Alive"] = this.IsAlive.ToString();

            foreach (KeyValuePair<String, Dictionary<String, String>> entry in Data)
            {
                Int32 addr = Convert.ToInt32(entry.Value["address"], 16) + Convert.ToInt32(entry.Value["offset"], 16) * (Number - 1);

                if (entry.Key == "Gold")
                {
                    gold += Reader.ReadInt(addr, 8);
                    jsonDict["Gold"] = gold.ToString();
                    continue;
                } else if (entry.Key == "WeightedUnits")
                {
                    weightedUnits += Reader.ReadInt(addr, 8);
                    jsonDict["WeightedUnits"] = weightedUnits.ToString();
                    continue;
                }

                String type = entry.Value["type"];
                Object value = Reader.ReadType(addr, type);

                if (entry.Value["category"] == "resource")
                {
                    resources += Convert.ToInt32(value);
                }
                jsonDict[entry.Key] = value.ToString();
            }
            Int32 score = (gold >> 2) + weightedUnits * 2 + Math.Min((resources >> 2),1) * 10;
            jsonDict["Score"] = score.ToString();
            jsonDict["Resources"] = resources.ToString();
            return JsonConvert.SerializeObject(jsonDict);
        }
    }
}
