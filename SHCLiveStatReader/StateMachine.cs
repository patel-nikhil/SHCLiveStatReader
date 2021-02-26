using System;
using System.Collections.Generic;
using System.IO;
using static SHC.Constants;
using static SHC.Util;

namespace SHC
{
    class StateMachine
    {
        static readonly Dictionary<string, State> stateList = new Dictionary<string, State>();
        static State currentState;

        static readonly LinkedList<Dictionary<string, object>> playerStats = new LinkedList<Dictionary<string, object>>();

        static StateMachine()
        {
            stateList[LOBBY] = new State(LOBBY, () => Reader.TestZero(MAP_START_YEAR, 4));
            stateList[GAME] = new State(GAME, () => !Reader.TestZero(MAP_START_YEAR, 4) && !Reader.ReadString(BACKGROUND_PATH_ADDRESS).Equals(LOBBY_BACKGROUND_FILE));
            stateList[STATS] = new State(STATS, () => !Reader.TestZero(MAP_START_YEAR, 4));

            currentState = stateList[LOBBY];
        }

        public static bool Lobby() => currentState == stateList[LOBBY];
        public static bool Game() => currentState == stateList[GAME];
        public static bool Stats() => currentState == stateList[STATS];

        public static void Reset()
        {
            currentState = stateList[LOBBY];
        }

        public static void Update()
        {
            if (!currentState.IsActive())
            {
                State prevState = StateMachine.currentState;
                currentState = StateMachine.Next();
                Console.WriteLine(MSG_UPDATE_STATE + currentState.ToString());

                if (Stats())
                {
                    WriteData(GREATEST_LORD_FILENAME, GreatestLord.Update(playerStats));
                } else if (Game() && prevState == stateList[LOBBY])
                {
                    ArchiveGreatestLordStatFile();
                }
            }

            if (Game())
            {
                UpdatePlayerStats(playerStats);
                WriteData(PLAYERDATA_FILENAME, playerStats);
                WriteData(GREATEST_LORD_FILENAME, GreatestLord.Update(playerStats));
            }

            if (Lobby())
            {
                ResetWeightedStats();
            }
        }

        private static void UpdatePlayerStats(LinkedList<Dictionary<string, object>> playerStats)
        {
            playerStats.Clear();
            for (int i = 0; i < PlayerFactory.PlayerList.Count; i++)
            {
                Player player = PlayerFactory.PlayerList[i];
                if (IsPlayerActive(i))
                {
                    playerStats.AddLast(player.Update());
                }
            }
            Player.Update(playerStats);
        }

        private static void ArchiveGreatestLordStatFile()
        {
            if (File.Exists(GREATEST_LORD_FILENAME))
            {
                string saveFileName = GetFilename(GREATEST_LORD_FILE_PREFIX);
                File.Move(GREATEST_LORD_FILENAME, saveFileName);
            }
        }

        private static bool IsPlayerActive(int playerNumber)
        {
            int baseAddress = Convert.ToInt32(Player.Data["Active"]["address"].ToString(), 16);
            int playerOffset = Convert.ToInt32(Player.Data["Active"]["offset"].ToString(), 16) * playerNumber;
            object active = Reader.ReadType(baseAddress + playerOffset, "boolean");

            if (active.ToString().ToLowerInvariant() == "false")
            {
                return false;
            }
            return true;
        }

        private static State Next()
        {
            if (currentState == stateList[LOBBY])
            {
                return stateList[GAME];
            }
            else if (currentState == stateList[GAME])
            {
                if (Reader.TestZero(MAP_START_YEAR, 4))
                {
                    return stateList[LOBBY];
                }
                else
                {
                    return stateList[STATS];
                }
            }
            else if (currentState == stateList[STATS])
            {
                if (Reader.TestZero(MAP_START_YEAR, 4))
                {
                    return stateList[LOBBY];
                }
                else
                {
                    return stateList[GAME];
                }
            }
            return stateList[LOBBY];
        }

        private static void ResetWeightedStats()
        {
            foreach (Player player in PlayerFactory.PlayerList)
            {
                player.ResetWeightedStats();
            }
        }
    }
}
