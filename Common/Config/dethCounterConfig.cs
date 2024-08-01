using System;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace dethCounter.Common.Config
{
    internal class dethCounterConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        /*
        [Header("DataConfig")]
        // World side / Player Side
        [DefaultValue(false)]
        [ReloadRequired]
        public bool saveWorldSide;
        */

        [Header("ShorterRespawn")]

        // Enabled?
        [DefaultValue(false)]
        public bool enableShorterRespawn;

        // Spawn Reduction
        [Range(1, 15)]
        [DefaultValue(15)]
        [Increment(1)]
        public int respawnSet;

        // Spawn Reduction Boss
        [Range(1, 30)]
        [DefaultValue(30)]
        [Increment(1)]
        public int respawnSetBoss;

        [Header("Server")]

        // Time between data update on server
        [Range(1, 60)]
        [DefaultValue(5)]
        [Increment(5)]
        [Slider]
        public int updateTime;

        // Dont Let People Change Config On a Server
        [DefaultValue(false)]
        public bool cannotChangeConfigOnServer;

        [Header("LeaderboardDisplaySetting")]

        [Range(1, 20)]
        [DefaultValue(15)]
        [Increment(1)]
        public int leaderboardShowNum;


        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            if (Main.netMode == NetmodeID.Server && cannotChangeConfigOnServer == true)
            {
                message = "Clients cannot change config on Server! (You can disable this in the Config!)";
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
