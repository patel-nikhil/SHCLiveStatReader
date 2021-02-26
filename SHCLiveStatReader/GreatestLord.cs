using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static SHC.Constants;

namespace SHC
{
    class GreatestLord
    {
        readonly static Dictionary<string, Dictionary<string, Dictionary<string, string>>> playerData;
        readonly static Dictionary<string, object> statsDictionary;
        public static List<Player> PlayerList { get; }

        static GreatestLord()
        {
            statsDictionary = new Dictionary<string, object>();
            playerData = 
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(File.ReadAllText("memory/greatestlord.json"));
        }

        public static Dictionary<string, object> Update(LinkedList<Dictionary<string, object>> endingPlayerStats)
        {
            Dictionary<string, int> scoreDict = new Dictionary<string, int>();
            scoreDict["Gold"] = 0;
            scoreDict["WeightedTroopsKilled"] = 0;
            scoreDict["LordKills"] = 0;
            scoreDict["MapStartYear"] = 0;
            scoreDict["MapStartMonth"] = 0;
            scoreDict["MapEndYear"] = 0;
            scoreDict["MapEndMonth"] = 0;
            scoreDict["WeightedBuildingsDestroyed"] = 0;

            Dictionary<string, object> mapStats = new Dictionary<string, object>();

            foreach (KeyValuePair<string, Dictionary<string, string>> entry in playerData["Map"])
            {
                int addr = Convert.ToInt32(entry.Value["address"], 16);
                object value = Reader.ReadType(addr, entry.Value["type"].ToString());
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
            if ((int)mapStats["MapStartYear"] == 0)
            {
                return statsDictionary;
            }

            statsDictionary["Map"] = mapStats;

            LinkedList<Dictionary<string, object>> playerStats = new LinkedList<Dictionary<string, object>>();
            for (var i = 0; i < MAX_PLAYERS; i++)
            {
                Dictionary<string, object> currentPlayer = new Dictionary<string, object>();
                currentPlayer["PlayerNumber"] = i + 1;
                foreach (KeyValuePair<string, Dictionary<string, string>> entry in playerData["Player"])
                {
                    int addr = Convert.ToInt32(entry.Value["address"], 16) + Convert.ToInt32(entry.Value["offset"], 16) * i;
                    string type = entry.Value["type"];

                    object value = Reader.ReadType(addr, type);
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
                    scoreDict["WeightedBuildingsDestroyed"], scoreDict["MapStartYear"], scoreDict["MapStartMonth"],
                    scoreDict["MapEndYear"], scoreDict["MapEndMonth"]);
                playerStats.AddLast(currentPlayer);
            }
            statsDictionary["PlayerStatistics"] = playerStats;
            return statsDictionary;
        }

        public static long CalculateScore
            (int gold, int lordKills, int weightedKills, int weightedBuildings, int startYear, int startMonth, int endYear, int endMonth)
        {
            const long multiplier = 0x66666667;
            long goldBonus = ((gold * multiplier) >> 32) / 4;
            long score = goldBonus + weightedKills + weightedBuildings * 100;
            score = score + (score * lordKills) / 4;

            int dateBonus = (endYear - startYear) * 12;
            dateBonus -= startMonth;
            dateBonus += endMonth;

            if (dateBonus < 1)
            {
                dateBonus = 1;
            }
            int bonusDivider = 200 + dateBonus;

            score = score * 200;
            score = score / bonusDivider;
            return score;
        }
    }
}
