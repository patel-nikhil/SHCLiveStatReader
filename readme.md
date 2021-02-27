# What it is

SHCLiveStatReader is a program designed to read and output game statistics from the game [Stronghold Crusader Extreme](https://fireflyworlds.com/games/strongholdcrusader/) made by Firefly Studios.

# How it works

SHCLiveStatReader reads the computer memory owned by the game's process and writes out relevant player and match data to two text files: `SHCPlayerData.json` and `GreatestLord.json`. 

The `SHCPlayerData.json` file contains statistics such as unit count, building count, resource counts for each player active in the current game. The `GreatestLord.json` contains cumulative statistics for the current match such as total gold (and other goods) produced, units killed and lost, and more.


# What do you need to use it

You will need the executable `SHCLiveStatReader.exe`, the dll `Newtonsoft.Json.dll`, and the `memory` folder and all the json files inside. Simply start the executable, and then start Stronghold Crusader Extreme. When you start a match (skirmish, multiplayer, campaign) it will show `Switched to state: game` in the window and the stats will be read.

# I want new data, or to change the data being read

These json files can be modified to change what data is being read by SHCLiveStatReader so long as you follow the same format. Deleting the files may cause the executable to break. Remove fields at your own risk - certain fields such as `Active` in `player.json ` are how SHCLiveStatReader knows what player numbers are participating in a game.

**`core.json`** should NOT be modified unless you are using a non 1.41 version or a mod that is not the Unofficial Crusader Patch and SHCLiveStatReader is not working.

# Compatibility

`SHCLiveStatReader` is targeted to work with the 1.41 English version of Stronghold Crusader Extreme. When launching the game you can check this by looking at the bottom left corner by the exit door - you should see the text V1.41-E or V1.41.1-E (this indicates game version number, and `E` indicates it is `Stronghold Crusader Extreme`, not regular `Stronghold Crusader`)

# Disclaimer

Stronghold Crusader is the legal property of Firefly Studios. This project has no affiliation and is not endorsed or supported by Firefly Studios. This content provided in this project does not modify the executable or other game files in any way.