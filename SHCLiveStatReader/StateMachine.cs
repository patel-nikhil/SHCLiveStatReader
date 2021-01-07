using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHC
{
    class StateMachine
    {
        static Dictionary<String, State> stateList = new Dictionary<string, State>();
        static State currentState;
        static List<int> ActivePlayers { get; }
        static Random gen = new Random();

        static String currentFilename = "GreatestLord.txt";

        static LinkedList<Dictionary<String, Object>> playerStats;

        static StateMachine()
        {
            State lobby = new State("Lobby", () => Reader.TestZero(0x024BA938, 4));
            State game = new State("Game", () => !Reader.TestZero(0x024BA938, 4) && !Reader.ReadString(0x1311607).Equals("shc_back.tgx"));
            State stats = new State("Stats", () => !Reader.TestZero(0x24BA938,4));

            stateList["Lobby"] = lobby;
            stateList["Game"] = game;
            stateList["Stats"] = stats;

            ActivePlayers = new List<int>();
            currentState = lobby;
        }

        public static void Reset()
        {
            currentState = stateList["Lobby"];
        }

        static State Next()
        {
            if (currentState == stateList["Lobby"])
            {
                return stateList["Game"];
            } else if (currentState == stateList["Game"])
            {
                if (Reader.TestZero(0x024BA938, 4))
                {
                    return stateList["Lobby"];
                }
                else
                {
                    return stateList["Stats"];
                }
            } else if (currentState == stateList["Stats"])
            {
                if (Reader.TestZero(0x024BA938, 4))
                {
                    return stateList["Lobby"];
                } else
                {
                    return stateList["Game"];
                }
            }
            return stateList["Lobby"];
        }

        public static bool Lobby() => currentState == stateList["Lobby"];
        public static bool Game() => currentState == stateList["Game"];
        public static bool Stats() => currentState == stateList["Stats"];


        public static void Update()
        {
            if (!currentState.isActive())
            {
                State prevState = StateMachine.currentState;
                currentState = StateMachine.Next();
                Console.WriteLine("Switched to state: " + currentState.ToString());

                if (Stats())
                {
                    File.WriteAllText(currentFilename, Newtonsoft.Json.JsonConvert.SerializeObject(GreatestLord.Update(playerStats), Newtonsoft.Json.Formatting.Indented));
                } else if (Game() && prevState == stateList["Lobby"])
                {
                    Func<String> GetFilename = () => { return "GreatestLord " + gen.Next().ToString() + ".txt"; };
                    if (File.Exists(currentFilename))
                    {
                        String saveFileName = GetFilename();
                        while (File.Exists(saveFileName))
                        {
                            saveFileName = GetFilename();
                        }
                        File.Move(currentFilename, saveFileName);
                    }
                }
            }

            if (Game())
            {
                LinkedList<Dictionary<String, Object>> gameData = new LinkedList<Dictionary<String, Object>>();
                for (int i = 0; i < PlayerFactory.PlayerList.Count; i++)
                {
                    Player player = PlayerFactory.PlayerList[i];
                    Object active = Reader.ReadType(Convert.ToInt32(Player.Data["Active"]["address"].ToString(), 16) 
                        + Convert.ToInt32(Player.Data["Active"]["offset"].ToString(), 16) * i, "boolean");

                    if (active.ToString().ToLowerInvariant() == "false")
                    {
                        continue;
                    }
                    gameData.AddLast(player.Update());
                }
                playerStats = PlayerStatFinalizer.ReadAndComputeScore(gameData);
                File.WriteAllText("SHCPlayerData.txt", Newtonsoft.Json.JsonConvert.SerializeObject(playerStats, Newtonsoft.Json.Formatting.Indented));
                File.WriteAllText(currentFilename, Newtonsoft.Json.JsonConvert.SerializeObject(GreatestLord.Update(playerStats), Newtonsoft.Json.Formatting.Indented));
            }

            //if (Lobby() || Stats())
            //{
            //    System.IO.File.WriteAllText("SHCPlayerData.txt", String.Empty);
            //}
        }

        public static String CurrentState()
        {
            return currentState.ToString();
        }

    }
}
