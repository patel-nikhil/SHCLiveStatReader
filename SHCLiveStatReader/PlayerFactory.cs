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
            Dictionary<String, Dictionary<String, String>> playerData = 
                JsonConvert.DeserializeObject<Dictionary<String,Dictionary<String, String>>>(File.ReadAllText("memory/player.json"));

            PlayerList = new List<Player>();

            for (int i = 0; i < maxPlayers; i++)
            {
                PlayerList.Add(
                new Player(i + 1,
                    new Dictionary<String, Dictionary<String, String>> {
                        { "Active", playerData["Active"] },
                        { "Name", playerData["Name"] },
                        { "Team", playerData["Team"] },
                        { "Gold", playerData["Gold"] },
                        { "Units", playerData["Units"] },
                        { "Popularity", playerData["Popularity"] },
                        { "Population", playerData["Population"] },
                        { "Housing", playerData["Housing"] },
                        { "Wood", playerData["Wood"] },
                        { "Stone", playerData["Stone"] },
                        { "Iron", playerData["Iron"] },
                        { "Pitch", playerData["Pitch"] }
                    }
                ));
            }
        }
    }
}
