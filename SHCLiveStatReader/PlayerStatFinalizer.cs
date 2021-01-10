using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHC
{
    class PlayerStatFinalizer
    {
        static Dictionary<String, Dictionary<String, String>> names;
        static Dictionary<String, Dictionary<String, Int32>> weights;
        static Dictionary<String, Dictionary<String, String>> buildingData;

        static PlayerStatFinalizer()
        {
            buildingData = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, String>>>(File.ReadAllText("memory/buildings.json"));
            weights = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, Int32>>>(File.ReadAllText("memory/weights.json"));
            names = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, String>>>(File.ReadAllText("memory/names.json"));
        }

        public static LinkedList<Dictionary<String, Object>> ReadAndComputeScore(LinkedList<Dictionary<String, Object>> gameData) {

            Int32 addr = Convert.ToInt32(buildingData["Buildings"]["address"], 16);
            Int32 offset = Convert.ToInt32(buildingData["Buildings"]["offset"], 16);
            Int32 ownerOffset = Convert.ToInt32(buildingData["Buildings"]["owneroffset"], 16);
            Int32 workersNeededOffset = Convert.ToInt32(buildingData["Buildings"]["workersneededoffset"], 16);
            Int32 workersWorkingOffset = Convert.ToInt32(buildingData["Buildings"]["workersworkingoffset"], 16);
            Int32 workersMissingOffset = Convert.ToInt32(buildingData["Buildings"]["workersmissingoffset"], 16);
            Int32 snoozedOffset = Convert.ToInt32(buildingData["Buildings"]["snoozedoffset"], 16);
            Int32 totalBuildings = Reader.ReadInt(Convert.ToInt32(buildingData["Buildings"]["total"], 16), 4);

            Dictionary<Int32, Dictionary<Int32, Dictionary<String, Int32>>> buildingCount = new Dictionary<Int32, Dictionary<Int32, Dictionary<String, Int32>>>();

            for (var i = 0; i < gameData.Count; i++)
            {
                gameData.ElementAt(i)["Buildings"] = new Dictionary<String, int>();
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
                Int32 buildingID = BitConverter.ToInt32(buildingArray, count * offset);
                Int32 owner = buildingArray[count * offset + ownerOffset];
                Int32 workers = buildingArray[count * offset + workersNeededOffset];
                Int32 workersWorking = buildingArray[count * offset + workersWorkingOffset];
                Int32 workersMissing = buildingArray[count * offset + workersMissingOffset];
                bool snoozed = buildingArray[count * offset + snoozedOffset] == 0;

                if (buildingID != 0 || count > 2 * i)
                {
                    if (owner > 0 && owner < 9)
                    {
                        int playerPos = 0;
                        while (playerPos < gameData.Count && Convert.ToInt32(gameData.ElementAt(playerPos)["PlayerNumber"]) != owner){
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

                        if (!names["Buildings"].ContainsKey(buildingID.ToString())){
                            if (!((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"]).ContainsKey(buildingID.ToString()))
                            {
                                ((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"])["Unknown"] = 0;
                            }
                            ((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"])["Unknown"] = ((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"])["Unknown"] + 1;
                        } 
                        else
                        {
                            String buildingName = names["Buildings"][buildingID.ToString()];
                            if (!((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"]).ContainsKey(buildingName))
                            {
                                ((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"])[buildingName] = 0;
                            }
                            ((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"])[buildingName] = ((Dictionary<String, int>)gameData.ElementAt(playerPos)["Buildings"])[buildingName] + 1;
                        }
                    }
                    i++;
                }
                count++;
            }

            for (var i = 0; i < gameData.Count; i++)
            {
                if (!Convert.ToBoolean(gameData.ElementAt(i)["Alive"]))
                {
                    continue;
                }
                Int32 economyScore = 0;
                Int32 militaryScore = 0;
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
            return gameData;
        }
    }
}
