using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHC
{
    class GreatestLord
    {
        static int maxPlayers = 8;
        public static List<Player> PlayerList { get; }
        static Dictionary<String, Dictionary<String, Dictionary<String, String>>> playerData;
        static Dictionary<String, Object> statsDictionary;

        static GreatestLord()
        {
            statsDictionary = new Dictionary<String, Object>();
            playerData = 
                JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, Dictionary<String, String>>>>(File.ReadAllText("memory/greatestlord.json"));
        }

        public static Dictionary<String, Object> Update(LinkedList<Dictionary<string, object>> endingPlayerStats)
        {
            Dictionary<String, Int32> scoreDict = new Dictionary<string, int>();
            scoreDict["Gold"] = 0;
            scoreDict["WeightedTroopsKilled"] = 0;
            scoreDict["LordKills"] = 0;
            scoreDict["Map Start Year"] = 0;
            scoreDict["Map Start Month"] = 0;
            scoreDict["Map End Year"] = 0;
            scoreDict["Map End Month"] = 0;
            scoreDict["WeightedBuildingsDestroyed"] = 0;

            Dictionary<String, Object> mapStats = new Dictionary<String, Object>();

            foreach (KeyValuePair<String, Dictionary<String, String>> entry in playerData["Map"])
            {
                Int32 addr = Convert.ToInt32(entry.Value["address"], 16);
                Object value = Reader.ReadType(addr, entry.Value["type"].ToString());
                mapStats[entry.Key] = value;
                try
                {
                    scoreDict[entry.Key] = Convert.ToInt32(value);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if ((int)mapStats["Map Start Year"] == 0)
            {
                return statsDictionary;
            }

            statsDictionary["Map"] = mapStats;

            LinkedList<Dictionary<String, Object>> playerStats = new LinkedList<Dictionary<String, Object>>();
            for (var i = 0; i < GreatestLord.maxPlayers; i++)
            {
                Dictionary<String, Object> currentPlayer = new Dictionary<String, Object>();
                currentPlayer["PlayerNumber"] = i + 1;
                foreach (KeyValuePair<String, Dictionary<String, String>> entry in playerData["Player"])
                {
                    Int32 addr = Convert.ToInt32(entry.Value["address"], 16) + Convert.ToInt32(entry.Value["offset"], 16) * i;
                    String type = entry.Value["type"];

                    Object value = Reader.ReadType(addr, type);
                    currentPlayer[entry.Key] = value;

                    if (entry.Key == "Active" && value.ToString().ToLowerInvariant() == "false")
                    {
                        break;
                    }

                    try
                    {
                        scoreDict[entry.Key] = Convert.ToInt32(value);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if (currentPlayer["Active"].ToString().ToLowerInvariant() == "false")
                {
                    continue;
                }

                foreach (var player in endingPlayerStats)
                {
                    if ((int)player["PlayerNumber"] == (int)currentPlayer["PlayerNumber"])
                    {
                        currentPlayer["EconomyScore"] = player["EconomyScore"];
                        currentPlayer["MilitaryScore"] = player["MilitaryScore"];
                        currentPlayer["Score"] = player["Score"];
                        currentPlayer["LargestWeightedArmy"] = player["LargestWeightedArmy"];
                        currentPlayer["LargestArmy"] = player["LargestArmy"];
                    }
                }

                currentPlayer["VanillaScore"] = 
                    GreatestLord.CalculateScore(scoreDict["Gold"], scoreDict["LordKills"], scoreDict["WeightedTroopsKilled"], 
                    scoreDict["WeightedBuildingsDestroyed"], scoreDict["Map Start Year"], scoreDict["Map Start Month"],
                    scoreDict["Map End Year"], scoreDict["Map End Month"]);
                playerStats.AddLast(currentPlayer);
            }
            statsDictionary["PlayerStatistics"] = playerStats;
            return statsDictionary;
        }

        public static long CalculateScore
            (Int32 gold, Int32 lordKills, Int32 weightedKills, Int32 weightedBuildings, Int32 startYear, Int32 startMonth, Int32 endYear, Int32 endMonth)
        {
            const long multiplier = 0x66666667;
            long goldBonus = ((gold * multiplier) >> 32) / 4;
            long score = goldBonus + weightedKills + weightedBuildings * 100;
            score = score + (score * lordKills) / 4;

            Int32 dateBonus = (endYear - startYear) * 12;
            dateBonus -= startMonth;
            dateBonus += endMonth;

            if (dateBonus < 1)
            {
                dateBonus = 1;
            }
            Int32 bonusDivider = 200 + dateBonus;

            score = score * 200;
            score = score / bonusDivider;
            return score;
        }
    }
}
