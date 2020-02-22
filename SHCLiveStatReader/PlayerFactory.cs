using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHC
{
    class PlayerFactory
    {
        static int maxPlayers = 8;
        public static List<Player> PlayerList { get; }

        static PlayerFactory()
        {
            Player.Data = 
                JsonConvert.DeserializeObject<Dictionary<String,Dictionary<String, String>>>(File.ReadAllText("memory/player.json"));

            PlayerList = new List<Player>();

            for (int i = 0; i < maxPlayers; i++)
            {
                PlayerList.Add(new Player(i + 1));
            }
        }
    }
}
