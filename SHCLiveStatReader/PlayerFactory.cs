using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using static SHC.Constants;

namespace SHC
{
    class PlayerFactory
    {
        public static List<Player> PlayerList { get; }

        static PlayerFactory()
        {
            Player.Data = 
                JsonConvert.DeserializeObject<Dictionary<string,Dictionary<string, object>>>(File.ReadAllText("memory/player.json"));

            PlayerList = new List<Player>();

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                PlayerList.Add(new Player(i + 1));
            }
        }
    }
}
