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
        static Dictionary<String, Dictionary<String, Int32>> weights;
        static Player() {
            weights = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, Int32>>>(System.IO.File.ReadAllText("memory/weights.json"));
        }

        private Dictionary<String, Object> mostRecentStats = new Dictionary<string, object>()
        {
            {"PlayerNumber", 0 },
            {"Active", true },
            {"Alive", true },
            {"LargestWeightedArmy", 0 },
            {"LargestArmy", 0 },
            {"EconomyScore", 0 },
            {"MilitaryScore", 0 },
            {"Score", 0 }
        };

        private int Number { get; }
        private int Score { get; }

        private bool IsAlive
        {
            get
            {
                return Reader.TestZero(0x024BA918 + 4 * (Number - 1), 4);
            }
        }

        public static Dictionary<String, Dictionary<String, Object>> Data { set;  get; }

        public Player(int number)
        {
            this.Number = number;
        }

        public void ResetLargestArmy()
        {
            mostRecentStats["LargestWeightedArmy"] = 0;
        }

        public Dictionary<String, Object> Update()
        {
            Int32 gold = 0;
            Int32 weightedUnits = 0;
            Int32 resources = 0;

            mostRecentStats["PlayerNumber"] = this.Number;
            bool alive = this.IsAlive;
            mostRecentStats["Alive"] = alive;

            if (!alive)
            {
                return mostRecentStats;
            }

            foreach (KeyValuePair<String, Dictionary<String, Object>> entry in Data)
            {
                Int32 addr = Convert.ToInt32((String)entry.Value["address"], 16) + Convert.ToInt32((String)entry.Value["offset"], 16) * (Number - 1);

                if (entry.Key == "Gold")
                {
                    gold += Reader.ReadInt(addr, 8);
                    mostRecentStats["Gold"] = gold;
                    continue;
                } else if (entry.Key == "WeightedUnits")
                {
                    weightedUnits += Reader.ReadInt(addr, 8);
                    mostRecentStats["WeightedUnits"] = weightedUnits;
                    mostRecentStats["LargestWeightedArmy"] = Math.Max(Convert.ToInt32(mostRecentStats["LargestWeightedArmy"]), weightedUnits);
                    continue;
                }

                String type = (String)entry.Value["type"];
                Object value = Reader.ReadType(addr, type);

                if (entry.Key == "Units")
                {
                    mostRecentStats["LargestArmy"] = Math.Max(Convert.ToInt32(mostRecentStats["LargestArmy"]), Convert.ToInt32(value));
                }

                if ((String)entry.Value["category"] == "resource")
                {
                    try
                    {
                        resources += Convert.ToInt32(value) * weights["Resources"][entry.Key];
                    }
                    catch (KeyNotFoundException)
                    {
                        resources += Convert.ToInt32(value);
                    }
                }
                mostRecentStats[entry.Key] = value;
            }
            mostRecentStats["Resources"] = resources;
            return mostRecentStats;
        }
    }
}
