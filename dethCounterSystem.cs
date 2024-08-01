using System;
using System.Collections.Generic;
using dethCounter.Common.Config;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace dethCounter
{
    public class dethCounterSystem : ModSystem
    {
        public int totalPlayers = 0;
        public List<string> playerNames = new List<string>();
        public List<string> playerData = new List<string>();
        public int[] playerSaveData = Array.Empty<int>();

        public override void SaveWorldData(TagCompound tag)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            // NYI
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            // NYI
        }

        public void SavePlayerData(TagCompound tag)
        {
            // Add all data to array then save the array
            for (int i = 0; i < playerNames.Count - 1; i++)
            {

            }

        }


        // Keybind
        public static ModKeybind lbKeybind { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            lbKeybind = KeybindLoader.RegisterKeybind(Mod, "OpenMenu", "P");
        }

        // Unload the Keybind
        public override void Unload()
        {
            lbKeybind = null;
        }

    }
}
