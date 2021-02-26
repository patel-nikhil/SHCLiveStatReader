using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SHC
{
    public class Constants
    {
        public static string PLAYERDATA_FILENAME = "SHCPlayerData.json";
        public static string GREATEST_LORD_FILENAME = "GreatestLord.json";
        public static string SHC_PROCESS_NAME = "Stronghold_Crusader_Extreme";

        public static string PLAYERDATA_FILE_PREFIX = "SHCPlayerData";
        public static string GREATEST_LORD_FILE_PREFIX = "GreatestLord";
        public static string DATA_FILE_SUFFIX = ".json";

        public static string MSG_UPDATE_STATE = "Switched to state: ";

        public static string GAME = "game";
        public static string LOBBY = "lobby";
        public static string STATS = "stats";

        public static int MAP_START_YEAR;
        public static int BACKGROUND_PATH_ADDRESS;
        public static string LOBBY_BACKGROUND_FILE;
        public static int PLAYER_DEATH_TIME;

        public static int MAX_PLAYERS = 8;

        static Constants()
        {
            Dictionary<string, string> coreVariables;
            if (File.Exists("memory/core.json"))
            {
                coreVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("memory/core.json"));
            }
            else
            {
                coreVariables = new Dictionary<string, string>();
            }

            if (coreVariables.TryGetValue("MapStartYear", out string mapStartYear))
            {
                MAP_START_YEAR = Convert.ToInt32(mapStartYear, 16);
            } else
            {
                MAP_START_YEAR = Convert.ToInt32("0x24BA938", 16);
            }

            if (!coreVariables.TryGetValue("BackgroundPathAddress", out string backgroundPathAddress))
            {
                BACKGROUND_PATH_ADDRESS = Convert.ToInt32(backgroundPathAddress, 16);
            }
            else
            {
                BACKGROUND_PATH_ADDRESS = Convert.ToInt32("0x1311607", 16);
            }

            if (!coreVariables.TryGetValue("LobbyBackgroundString", out LOBBY_BACKGROUND_FILE))
            {
                LOBBY_BACKGROUND_FILE = "shc_back.tgx";
            }

            if (!coreVariables.TryGetValue("SHCProcessName", out SHC_PROCESS_NAME))
            {
                SHC_PROCESS_NAME = "Stronghold_Crusader_Extreme";
            }

            if (!coreVariables.TryGetValue("PlayerDeathTime", out string playerDeathTime))
            {
                PLAYER_DEATH_TIME = Convert.ToInt32(playerDeathTime, 16);
            }
            else
            {
                PLAYER_DEATH_TIME = Convert.ToInt32("0x24BA918", 16);
            }
        }
    }
}
