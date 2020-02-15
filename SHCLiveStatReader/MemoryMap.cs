using System;
using System.Collections.Generic;
using System.Text;

namespace SHC
{
    static class MemoryMap
    {
        public static Dictionary<string, Data> playerData;
        public static Dictionary<string, Data> leaderBoard;

        static MemoryMap()
        {

            playerData = new Dictionary<string, Data>
            {
                {
                    "Player1", new Data("Player1", CreatePlayerDataDictionary(0x024BA286,0x011F2938,0x011F2968,0x011F2870,0x011F24A4,0x011F24A0))
                },

                {
                    "Player2", new Data("Player2", CreatePlayerDataDictionary(0x024BA2E0,0x011F632C,0x011F635C,0x011F6264,0x011F5E98,0x011F5E94))
                },

                {
                    "Player3", new Data("Player3", CreatePlayerDataDictionary(0x024BA33A,0x011F9D20,0x011F9D50,0x011F9C58,0x011F988C,0x011F9888))
                },

                {
                    "Player4", new Data("Player4", CreatePlayerDataDictionary(0x024BA394,0x011FD714,0x011FD744,0x011FD64C,0x011FD280,0x011FD27C))
                },

                {
                    "Player5", new Data("Player5", CreatePlayerDataDictionary(0x024BA3EE,0x01201108,0x01201138,0x01201040,0x01200C74,0x01200C70))
                },

                {
                    "Player6", new Data("Player6", CreatePlayerDataDictionary(0x024BA448,0x01204AFC,0x01204B2C,0x01204A34,0x01204668,0x01204664))
                },

                {
                    "Player7", new Data("Player7", CreatePlayerDataDictionary(0x024BA4A2,0x012084F0,0x01208520,0x01208428,0x0120805C,0x01208058))
                },

                {
                    "Player8", new Data("Player8", CreatePlayerDataDictionary(0x024BA4FC,0x0120BEE4,0x0120BF14,0x0120BE1C,0x0120BA50,0x120BA4C))
                }
            };
            leaderBoard = new Dictionary<string, Data>
            {
                {
                    "Player1", new Data("Player1", CreateLeaderBoardDictionary(0x024BA286,0x024BA564, 0x024BA888, 0x024BA730, 0x024BA778, 0x024BA754, 0x024BA79C, 0x024BA81C, 0x024BA70C, 0x024BA586, 0x011F24A0))
                },

                {
                    "Player2", new Data("Player2", CreateLeaderBoardDictionary(0x024BA2E0, 0x024BA568, 0x024BA88C, 0x024BA734, 0x024BA77C, 0x024BA758, 0x024BA7A0, 0x024BA820, 0x024BA710, 0x024BA588, 0x011F5E94))
                },

                {
                    "Player3", new Data("Player3", CreateLeaderBoardDictionary(0x024BA33A, 0x024BA56C, 0x024BA890, 0x024BA738, 0x024BA780, 0x024BA75C, 0x024BA7A4, 0x024BA824, 0x024BA714, 0x024BA58A, 0x011F9888))
                },

                {
                    "Player4", new Data("Player4", CreateLeaderBoardDictionary(0x024BA394, 0x024BA570, 0x024BA894, 0x024BA73C, 0x024BA784, 0x024BA760, 0x024BA7A8, 0x024BA828, 0x024BA718, 0x024BA58C, 0x011FD27C))
                },

                {
                    "Player5", new Data("Player5", CreateLeaderBoardDictionary(0x024BA448, 0x024BA574, 0x024BA898, 0x024BA740, 0x024BA788, 0x024BA764, 0x024BA7AC, 0x024BA82C, 0x024BA71C, 0x024BA58E, 0x01200C70))
                },

                {
                    "Player6", new Data("Player6", CreateLeaderBoardDictionary(0x024BA448, 0x024BA578, 0x024BA89C, 0x024BA744, 0x024BA78C, 0x024BA768, 0x024BA7B0, 0x024BA830, 0x024BA720, 0x024BA590, 0x01204664))
                },

                {
                    "Player7", new Data("Player7", CreateLeaderBoardDictionary(0x024BA4A2, 0x024BA57C, 0x024BA8A0, 0x024BA748, 0x024BA790, 0x024BA76C, 0x024BA7B4, 0x024BA834, 0x024BA724, 0x024BA592, 0x01208058))
                },

                {
                    "Player8", new Data("Player8", CreateLeaderBoardDictionary(0x024BA4FC, 0x024BA580, 0x024BA8A4, 0x024BA74C, 0x024BA794, 0x024BA770, 0x024BA7B8, 0x024BA838, 0x024BA728, 0x024BA594, 0x120BA4C))
                }
            };
        }

        static Dictionary<string, Dictionary<String, Object>> CreatePlayerDataDictionary(Int32 name, Int32 gold, Int32 units, Int32 popularity, Int32 population, Int32 housing)
        {
            return new Dictionary<string, Dictionary<String, Object>>
                {
                    {"Name", CreateValueDictionary(name, 32)},
                    {"Gold", CreateValueDictionary(gold, 4)},
                    {"Units", CreateValueDictionary(units, 4)},
                    {"Popularity", CreateValueDictionary(popularity, 4)},
                    {"Population", CreateValueDictionary(population, 2)},
                    {"Housing", CreateValueDictionary(housing, 2)}
                };
        }

        static Dictionary<string, Dictionary<String, Object>> CreateLeaderBoardDictionary(Int32 name, Int32 gold, Int32 units, Int32 food, Int32 stone, Int32 iron, Int32 wood, Int32 buildings, Int32 razed, Int32 population, Int32 active)
        {
            return new Dictionary<string, Dictionary<String, Object>>
                {
                    {"Name", CreateValueDictionary(name, 32)},
                    {"Total Gold", CreateValueDictionary(gold, 4)},
                    {"Troops Produced", CreateValueDictionary(units, 4)},
                    {"Food Produced", CreateValueDictionary(food, 4)},
                    {"Stone Produced", CreateValueDictionary(stone, 4)},
                    {"Iron Produced", CreateValueDictionary(iron, 4)},
                    {"Wood Produced", CreateValueDictionary(wood, 4)},
                    {"Buildings Lost", CreateValueDictionary(buildings, 4)},
                    {"Enemy Building Destroyed", CreateValueDictionary(razed, 4)},
                    {"Highest Population", CreateValueDictionary(population, 2)},
                    {"Housing", CreateValueDictionary(active, 2)}
                };
        }

        static Dictionary<String, Object> CreateValueDictionary(Int32 addr, Int32 size)
        {
            return new Dictionary<string, Object> { { "Address", addr }, { "Value", 0 }, { "Size", size } };
        }

        public static String PlayerDataString()
        {
            String str = "{\n";
            foreach (KeyValuePair<String, Data> data in playerData)
            {
                String dataString = data.Value.ToString();
                if (dataString == "") break;
                str += dataString + ",\n";
            }
            str = str.Remove(str.Length - 2);
            str += "\n}";
            return str;
        }

        public static String LeaderboardString()
        {
            bool active = false;
            foreach (KeyValuePair<String, Data> data in leaderBoard)
            {
                if ((Int32)(data.Value.GameData()["Housing"]["Value"]) != 0)
                {
                    active = true;
                }
            }
            if (!active) return "{}";

            String str = "{\n";
            foreach (KeyValuePair<String, Data> data in leaderBoard)
            {
                String dataString = data.Value.ToString();
                if (dataString == "") break;
                str += dataString + ",\n";
            }
            str = str.Remove(str.Length - 2);
            str += "\n}";
            return str;
        }

    }
}
