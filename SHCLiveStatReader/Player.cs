using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using static SHC.Constants;

namespace SHC
{
    class Player
    {
        readonly static Dictionary<string, Dictionary<string, string>> names;
        readonly static Dictionary<string, Dictionary<string, int>> weights;
        readonly static Dictionary<string, Dictionary<string, string>> buildingData;

        static Player() {
            buildingData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText("memory/buildings.json"));
            weights = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(File.ReadAllText("memory/weights.json"));
            names = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText("memory/names.json"));
        }

        private Dictionary<string, object> mostRecentStats = new Dictionary<string, object>()
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

        private bool IsAlive
        {
            get
            {
                return Reader.TestZero(PLAYER_DEATH_TIME + 4 * (Number - 1), 4);
            }
        }

        public static Dictionary<string, Dictionary<string, object>> Data { set;  get; }

        public Player(int number)
        {
            this.Number = number;
        }

        public void ResetWeightedStats()
        {
            mostRecentStats["LargestWeightedArmy"] = 0;
            mostRecentStats["LargestArmy"] = 0;
            mostRecentStats["WeightedUnits"] = 0;
        }

        public Dictionary<string, object> Update()
        {
            int gold = 0;
            int weightedUnits = 0;
            int resources = 0;

            mostRecentStats["PlayerNumber"] = this.Number;
            bool alive = this.IsAlive;
            mostRecentStats["Alive"] = alive;

            if (!alive)
            {
                return mostRecentStats;
            }

            foreach (KeyValuePair<string, Dictionary<string, object>> entry in Data)
            {
                int baseAddress = Convert.ToInt32((string)entry.Value["address"], 16);
                int offset = Convert.ToInt32((string)entry.Value["offset"], 16) * (Number - 1);
                int addr = baseAddress + offset;

                if (entry.Key == "Gold")
                {
                    gold += Reader.ReadInt(addr, 4);
                    mostRecentStats["Gold"] = gold;
                    continue;
                } else if (entry.Key == "WeightedUnits")
                {
                    weightedUnits += Reader.ReadInt(addr, 4);
                    mostRecentStats["WeightedUnits"] = weightedUnits;
                    mostRecentStats["LargestWeightedArmy"] = Math.Max(Convert.ToInt32(mostRecentStats["LargestWeightedArmy"]), weightedUnits);
                    continue;
                }

                string type = (string)entry.Value["type"];
                object value = Reader.ReadType(addr, type);

                if (entry.Key == "Units")
                {
                    mostRecentStats["LargestArmy"] = Math.Max(Convert.ToInt32(mostRecentStats["LargestArmy"]), Convert.ToInt32(value));
                }

                if ((string)entry.Value["category"] == "resource")
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


        public static void Update(LinkedList<Dictionary<string, object>> gameData)
        {
            UpdateBuildings(gameData);
            UpdateScore(gameData);
        }

        public static void UpdateBuildings(LinkedList<Dictionary<string, object>> gameData)
        {
            int addr = Convert.ToInt32(buildingData["Buildings"]["address"], 16);
            int offset = Convert.ToInt32(buildingData["Buildings"]["offset"], 16);
            int ownerOffset = Convert.ToInt32(buildingData["Buildings"]["owneroffset"], 16);
            int workersNeededOffset = Convert.ToInt32(buildingData["Buildings"]["workersneededoffset"], 16);
            int workersWorkingOffset = Convert.ToInt32(buildingData["Buildings"]["workersworkingoffset"], 16);
            int workersMissingOffset = Convert.ToInt32(buildingData["Buildings"]["workersmissingoffset"], 16);
            int snoozedOffset = Convert.ToInt32(buildingData["Buildings"]["snoozedoffset"], 16);
            int totalBuildings = Reader.ReadInt(Convert.ToInt32(buildingData["Buildings"]["total"], 16), 4);

            for (var i = 0; i < gameData.Count; i++)
            {
                gameData.ElementAt(i)["Buildings"] = new Dictionary<string, int>();
                gameData.ElementAt(i)["CurrentWeightedBuildings"] = 0;
                gameData.ElementAt(i)["WeightedActiveBuildings"] = 0;
                gameData.ElementAt(i)["CurrentWorkersNeeded"] = 0;
                gameData.ElementAt(i)["CurrentWorkersWorking"] = 0;
                gameData.ElementAt(i)["CurrentWorkersMissing"] = 0;
            }
            byte[] buildingArray = Reader.ReadBytes(addr, 0x490 * 10000);
            int count = 0;
            for (var i = 0; i < totalBuildings;)
            {
                int buildingID = BitConverter.ToInt32(buildingArray, count * offset);
                int owner = buildingArray[count * offset + ownerOffset];
                int workers = buildingArray[count * offset + workersNeededOffset];
                int workersWorking = buildingArray[count * offset + workersWorkingOffset];
                int workersMissing = buildingArray[count * offset + workersMissingOffset];
                bool snoozed = buildingArray[count * offset + snoozedOffset] == 0;

                if (buildingID != 0 || count > 2 * i)
                {
                    if (owner > 0 && owner < 9)
                    {
                        int playerPos = 0;
                        while (playerPos < gameData.Count && Convert.ToInt32(gameData.ElementAt(playerPos)["PlayerNumber"]) != owner)
                        {
                            playerPos++;
                        }
                        if (playerPos >= gameData.Count || buildingID == 0)
                        {
                            i++;
                            count++;
                            continue;
                        }
                        if (!snoozed)
                        {
                            gameData.ElementAt(playerPos)["WeightedActiveBuildings"] = Convert.ToInt32(gameData.ElementAt(playerPos)["WeightedActiveBuildings"]) + weights["Buildings"][buildingID.ToString()];
                        }
                        gameData.ElementAt(playerPos)["CurrentWeightedBuildings"] = Convert.ToInt32(gameData.ElementAt(playerPos)["CurrentWeightedBuildings"]) + weights["Buildings"][buildingID.ToString()];
                        gameData.ElementAt(playerPos)["CurrentWorkersNeeded"] = Convert.ToInt32(gameData.ElementAt(playerPos)["CurrentWorkersNeeded"]) + workers;
                        gameData.ElementAt(playerPos)["CurrentWorkersWorking"] = Convert.ToInt32(gameData.ElementAt(playerPos)["CurrentWorkersWorking"]) + workersWorking;
                        gameData.ElementAt(playerPos)["CurrentWorkersMissing"] = Convert.ToInt32(gameData.ElementAt(playerPos)["CurrentWorkersMissing"]) + workersMissing;

                        if (!names["Buildings"].ContainsKey(buildingID.ToString()))
                        {
                            if (!((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"]).ContainsKey(buildingID.ToString()))
                            {
                                ((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"])["Unknown"] = 0;
                            }
                            ((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"])["Unknown"] = ((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"])["Unknown"] + 1;
                        }
                        else
                        {
                            string buildingName = names["Buildings"][buildingID.ToString()];
                            if (!((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"]).ContainsKey(buildingName))
                            {
                                ((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"])[buildingName] = 0;
                            }
                            ((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"])[buildingName] = ((Dictionary<string, int>)gameData.ElementAt(playerPos)["Buildings"])[buildingName] + 1;
                        }
                    }
                    i++;
                }
                count++;
            }
        }

        private static void UpdateScore(LinkedList<Dictionary<string, object>> gameData)
        {
            for (var i = 0; i < gameData.Count; i++)
            {
                if (!Convert.ToBoolean(gameData.ElementAt(i)["Alive"]))
                {
                    continue;
                }
                int economyScore = 0;
                int militaryScore = 0;
                militaryScore += Convert.ToInt32(gameData.ElementAt(i)["WeightedTroopsKilled"]);
                militaryScore += 5 * Convert.ToInt32(gameData.ElementAt(i)["WeightedBuildingsDestroyed"]);
                economyScore += (Convert.ToInt32(gameData.ElementAt(i)["Resources"]) + Convert.ToInt32(gameData.ElementAt(i)["GoodsSent"]) + Convert.ToInt32(gameData.ElementAt(i)["Gold"])) / 10;
                militaryScore += Convert.ToInt32(gameData.ElementAt(i)["WeightedUnits"]);
                militaryScore += 5 * Convert.ToInt32(gameData.ElementAt(i)["CurrentWeightedBuildings"]);
                militaryScore += Convert.ToInt32(gameData.ElementAt(i)["Population"]);
                gameData.ElementAt(i)["EconomyScore"] = economyScore;
                gameData.ElementAt(i)["MilitaryScore"] = militaryScore;
                gameData.ElementAt(i)["Score"] = economyScore + militaryScore;
            }
        }
    }
}
