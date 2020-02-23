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
        public static Dictionary<String, Dictionary<String, Object>> Data { set;  get; }
        int Number { get; }
        int Score { get;  }

        bool IsAlive
        {
            get
            {
                return Reader.TestZero(0x024BA918 + 4 * (Number - 1), 4);
            }
        }

        public Player(int number)
        {
            this.Number = number;
        }

        public Dictionary<String, Object> Update()
        {
            Int32 gold = 0;
            Int32 weightedUnits = 0;
            Int32 resources = 0;

            Dictionary<String, Object> jsonDict = new Dictionary<String, Object>();
            jsonDict["PlayerNumber"] = this.Number;
            jsonDict["Alive"] = this.IsAlive;

            foreach (KeyValuePair<String, Dictionary<String, Object>> entry in Data)
            {
                Int32 addr = Convert.ToInt32((String)entry.Value["address"], 16) + Convert.ToInt32((String)entry.Value["offset"], 16) * (Number - 1);

                if (entry.Key == "Gold")
                {
                    gold += Reader.ReadInt(addr, 8);
                    jsonDict["Gold"] = gold;
                    continue;
                } else if (entry.Key == "WeightedUnits")
                {
                    weightedUnits += Reader.ReadInt(addr, 8);
                    jsonDict["WeightedUnits"] = weightedUnits;
                    continue;
                }

                String type = (String)entry.Value["type"];
                Object value = Reader.ReadType(addr, type);

                if ((String)entry.Value["category"] == "resource")
                {
                    resources += Convert.ToInt32(value);
                }
                jsonDict[entry.Key] = value;
            }
            Int32 score = (gold >> 2) + weightedUnits * 2 + Math.Min((resources >> 2),1) * 10;
            jsonDict["Score"] = score;
            jsonDict["Resources"] = resources;
            return jsonDict;
        }
    }
}
