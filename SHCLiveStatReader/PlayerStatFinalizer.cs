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
        public static LinkedList<Dictionary<String, Object>> ReadAndComputeScore(Int32 totalBuildings, LinkedList<Dictionary<String, Object>> gameData) {
            Dictionary<String, Dictionary<String, String>> buildingData = 
                JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, String>>>(File.ReadAllText("memory/buildings.json"));

            Dictionary<String, Dictionary<String, Int32>> buildingWeights =
                JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, Int32>>>(File.ReadAllText("memory/weights.json"));

            Int32 addr = Convert.ToInt32(buildingData["Buildings"]["address"], 16);
            Int32 offset = Convert.ToInt32(buildingData["Buildings"]["offset"], 16);
            Int32 ownerOffset = Convert.ToInt32(buildingData["Buildings"]["owneroffset"], 16);

            Dictionary<Int32, Dictionary<Int32, Dictionary<String, Int32>>> buildingCount = new Dictionary<Int32, Dictionary<Int32, Dictionary<String, Int32>>>();

            for (var i = 0; i < gameData.Count; i++)
            {
                gameData.ElementAt(i)["CurrentWeightedBuildings"] = 0;
            }
            int count = 0;
            for (var i = 0; i < totalBuildings;)
            {
                Int32 buildingID = Reader.ReadInt(addr + count * offset, 8);
                Int32 owner = Reader.ReadByte(addr + count * offset + ownerOffset);
                if (buildingID != 0 || count > 2 * i)
                {
                    if (owner > 0 && owner < 9)
                    {
                        int playerPos = 0;
                        while (Convert.ToInt32(gameData.ElementAt(playerPos)["PlayerNumber"]) != owner && playerPos < gameData.Count){
                            playerPos++;
                        }
                        gameData.ElementAt(playerPos - 1)["CurrentWeightedBuildings"] = Convert.ToInt32(gameData.ElementAt(playerPos - 1)["CurrentWeightedBuildings"]) + buildingWeights["Buildings"][buildingID.ToString()];
                    }
                    i++;
                }
                count++;
            }

            for (var i = 0; i < gameData.Count; i++)
            {
                Int32 score = 0;
                score = Convert.ToInt32(gameData.ElementAt(i)["WeightedTroopsKilled"]);
                score += 5 * Convert.ToInt32(gameData.ElementAt(i)["WeightedBuildingsDestroyed"]);
                score += (Convert.ToInt32(gameData.ElementAt(i)["Resources"]) + Convert.ToInt32(gameData.ElementAt(i)["GoodsSent"]) + Convert.ToInt32(gameData.ElementAt(i)["Gold"])) / 10;
                score += Convert.ToInt32(gameData.ElementAt(i)["WeightedUnits"]);
                score += 5 * Convert.ToInt32(gameData.ElementAt(i)["CurrentWeightedBuildings"]);
                score += Convert.ToInt32(gameData.ElementAt(i)["Population"]);
                gameData.ElementAt(i)["Score"] = score;
            }

                return gameData;
        }
    }
}
