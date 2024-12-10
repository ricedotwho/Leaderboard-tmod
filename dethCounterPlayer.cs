using dethCounter.Common.Config;
using dethCounter.Common.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace dethCounter
{
    public class dethCounterPlayer : ModPlayer
    {
        // Counts
        public int killCount = 0;
        public int killCountNoStatue = 0;
        public int deathCount = 0;
        public int deathCountBoss = 0;
        public int deathTime = 0;
        public int playTimeSeconds = 0;

        // Timers
        public int timer = 60;
        public int timer2 = 60;
        public int timer3 = 60;
        public int totalTimeS = 0;
        public int tick = 0;
        public int deadTimer = 0;
        public int syncTimer = 0;

        // Strings
        public string playerName = "";
        public string timePlayed;
        public string playTime = "";

        // Boolean
        public bool UIVisible = false;
        public bool bossAlive = false;
        public bool userHasID = false;
        public bool needUpdate = false;
        // Temp var for NYI code
        public bool saveWorldSide = false;

        // Arrays
        string[] saveVars = { "killCount", "killCountNoStatue", "deathCount", "playTimeSeconds" };


        // Data Display and Calculation Variables
        // Kills
        public string killsDataName = "";
        public string killsDataKills = "";
        public string killsDataDeaths = "";
        public string killsDataKD = "";
        // Statue
        public string killsDataNameStatue = "";
        public string killsDataKillsStatue = "";
        public string killsDataDeathsStatue = "";
        public string killsDataKDStatue = "";

        // Deaths
        public string deathsDataName = "";
        public string deathsDataDeaths = "";
        public string deathsDataBossDeaths = "";

        // Playtime
        public string playtimeDataName = "";
        public string playtimeDataPlaytime = "";
        public string playtimeDataTimedead = "";


        // Updates once a tick, 1tick = 1frame (game is forced at 60tps) *DOES NOT UPDATE WHILE DEAD (in singleplayer)*
        public override void PostUpdate()
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();

            // Every Second
            if (++tick >= 60)
            {
                // Everything here runs once a second
                tick = 0;
                totalTimeS++;
                playTimeSeconds++;
                timer++;
                timer2++;
                timer3++;
                syncTimer++;

                // Check for active boss (within 4000 tiles)
                if (Main.netMode != NetmodeID.Server)
                {
                    bossAlive = false;
                    for (int k = 0; k < 200; k++)
                    {
                        if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == NPCID.EaterofWorldsHead || Main.npc[k].type == NPCID.EaterofWorldsBody || Main.npc[k].type == NPCID.EaterofWorldsTail) && Math.Abs(Player.Center.X - Main.npc[k].Center.X) + Math.Abs(Player.Center.Y - Main.npc[k].Center.Y) < 4000f)
                        {
                            bossAlive = true;
                            break;
                        }
                    }
                    // Set the (boss) respawn time to the (normal) respawn time when the boss despawns
                    if (Player.respawnTimer > (conf.respawnSet * 60) && !bossAlive) Player.respawnTimer = (conf.respawnSet * 60);
                }

            }

            // Update every 5 seconds on mutiplayer, 1 second on singleplayer.
            // Bug: Something causes my other test client to update 1s after this runs! Not sure why
            if (timer2 >= conf.updateTime && Main.netMode == NetmodeID.Server || timer2 >= 1 && Main.netMode == NetmodeID.SinglePlayer)
            {

                // if (saveWorldSide) { ModContent.GetInstance<dethCounterSystem>().SavePlayerData(null); } 

                // Kills
                killsDataName = GetPlayerData("kills", "name");
                killsDataKills = GetPlayerData("kills", "kills");
                killsDataDeaths = GetPlayerData("kills", "deaths");
                killsDataKD = GetPlayerData("kills", "k/d");
                // Statue
                killsDataNameStatue = GetPlayerData("kills", "nameStatue");
                killsDataKillsStatue = GetPlayerData("kills", "killsStatue");
                killsDataDeathsStatue = GetPlayerData("kills", "deathsStatue");
                killsDataKDStatue = GetPlayerData("kills", "k/dStatue");

                // Deaths
                deathsDataName = GetPlayerData("deaths", "name");
                deathsDataDeaths = GetPlayerData("deaths", "deaths");
                deathsDataBossDeaths = GetPlayerData("deaths", "bossDeaths");


                // Playtime
                playtimeDataName = GetPlayerData("playtime", "name");
                playtimeDataPlaytime = GetPlayerData("playtime", "playtime");
                playtimeDataTimedead = GetPlayerData("playtime", "timeDead");

                // Timer
                timer2 = 0;

                if (Main.netMode == NetmodeID.Server)
                {
                    // Forward to Clients
                    var packet = Mod.GetPacket();
                    packet.Write((byte)DeathCountModMessageType.SendClientPacket);

                    // Kills
                    packet.Write(killsDataName);
                    packet.Write(killsDataKills);
                    packet.Write(killsDataDeaths);
                    packet.Write(killsDataKD);
                    // Statue
                    packet.Write(killsDataNameStatue);
                    packet.Write(killsDataKillsStatue);
                    packet.Write(killsDataDeathsStatue);
                    packet.Write(killsDataKDStatue);

                    // Deaths
                    packet.Write(deathsDataName);
                    packet.Write(deathsDataDeaths);
                    packet.Write(deathsDataBossDeaths);

                    // Playtime
                    packet.Write(playtimeDataName);
                    packet.Write(playtimeDataPlaytime);
                    packet.Write(playtimeDataTimedead);

                    // Send (Sends to all connected clients)
                    packet.Send(-1, -1);
                }
            }
        }

        // Updates only while player is dead
        public override void UpdateDead()
        {
            base.UpdateDead();

            // Count Seconds
            if (++deadTimer >= 60)
            {
                deadTimer = 0;
                deathTime++;

                // Set spawn to lower when the boss despawns
                var conf = ModContent.GetInstance<dethCounterConfig>();
                if (Main.netMode != NetmodeID.Server && conf.enableShorterRespawn)
                {
                    bossAlive = false;
                    for (int k = 0; k < 200; k++)
                    {
                        // If npc is boss, or segments of EOW
                        if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == NPCID.EaterofWorldsHead || Main.npc[k].type == NPCID.EaterofWorldsBody || Main.npc[k].type == NPCID.EaterofWorldsTail) && Math.Abs(Player.Center.X - Main.npc[k].Center.X) + Math.Abs(Player.Center.Y - Main.npc[k].Center.Y) < 4000f)
                        {
                            bossAlive = true;
                            break;
                        }
                    }
                    // Set respawn time to lower
                    if (Player.respawnTimer > (conf.respawnSet * 60) && !bossAlive) Player.respawnTimer = (conf.respawnSet * 60);
                }
            }
        }

        // On hit Any NPC
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0) // Includes Friendly :skull:
            {
                // We dont want statues to count for at least one Var, so we know the not-cheesed kill count and k/d
                if (!target.SpawnedFromStatue)
                {
                    killCount++;
                    killCountNoStatue++;
                }
                else
                {
                    killCount++;
                }
            }
        }

        // Run on Player Death, not kill. Odd naming TML devs
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            // Get config instance
            var conf = ModContent.GetInstance<dethCounterConfig>();

            deathCount++;

            // If a boss is alive, add to the deathCountBoss var
            if (bossAlive)
            {
                deathCountBoss++;
                if (conf.enableShorterRespawn)
                    Player.respawnTimer = (conf.respawnSetBoss * 60); // Set boss Respawn time
            }
            else
            {
                if (conf.enableShorterRespawn)
                    Player.respawnTimer = (conf.respawnSet * 60); // Set normal respawn time
            }
        }

        // On autosave and save & quit
        public override void SaveData(TagCompound tag)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();

            // This are only saved locally, world side saving is NYI and is looking rough right now
            // saveWorldSide is hardset false right now
            if (!saveWorldSide)
            {
                tag["killCount"] = killCount;
                tag["killCountNoStatue"] = killCountNoStatue;
                tag["deathCount"] = deathCount;
                tag["deathCountBoss"] = deathCountBoss;
                tag["deathTime"] = deathTime;
                tag["playTimeSeconds"] = playTimeSeconds;
            }
        }
        // On load player *probably*
        public override void LoadData(TagCompound tag)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();

            // saveWorldSide is hardset false right now
            if (!saveWorldSide)
            {
                killCount = tag.GetInt("killCount");
                killCountNoStatue = tag.GetInt("killCountNoStatue");
                deathCount = tag.GetInt("deathCount");
                deathCountBoss = tag.GetInt("deathCountBoss");
                deathTime = tag.GetInt("deathTime");
                playTimeSeconds = tag.GetInt("playTimeSeconds");
            }
        }


        public override void CopyClientState(ModPlayer clientclone)
        {
            dethCounterPlayer clone = clientclone as dethCounterPlayer;
            for (int i = 0; i < (saveVars.Count() - 1); i++)
            {
                clone.saveVars[i] = saveVars[i];
            }
        }

        // It probably sends to other clients, Not sure, on a timer now though
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();
            if (syncTimer >= conf.updateTime)
            {
                syncTimer = 0;
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)DeathCountModMessageType.SyncPlayerDeath);
                packet.Write((byte)Player.whoAmI);
                packet.Write(Player.name);
                packet.Write(killCount);
                packet.Write(killCountNoStatue);
                packet.Write(deathCount);
                packet.Write(deathCountBoss);
                packet.Write(deathTime);
                packet.Write(playTimeSeconds);
                packet.Send(toWho, fromWho);
            }
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            // Only runs when on a server
            // WIP (world side, exclusive to world, data storage *does not work*)
            dethCounterPlayer clone = clientPlayer as dethCounterPlayer;
            var conf = ModContent.GetInstance<dethCounterConfig>();

            // Per Client Data

            if (totalTimeS >= conf.updateTime)
            {
                totalTimeS = 0;
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)DeathCountModMessageType.UpdateData);
                packet.Write((byte)Player.whoAmI);
                packet.Write(Player.name);
                packet.Write(killCount);
                packet.Write(killCountNoStatue);
                packet.Write(deathCount);
                packet.Write(deathCountBoss);
                packet.Write(deathTime);
                packet.Write(playTimeSeconds);
                packet.Send();
            }
        }

        // Keybind setup
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // This is the OpenMenu Keybind default: "P"
            if (dethCounterSystem.lbKeybind.JustPressed)
            {

                if (!UIVisible) { ModContent.GetInstance<UISystem>().ShowMyUI(); UIVisible = true; }
                else { ModContent.GetInstance<UISystem>().HideMyUI(); UIVisible = false; }
            }
        }

        // This is only called on the server side!
        public string GetPlayerData(string type, string type2)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();

            int player;
            string replyText = "";
            int x = 0;
            int y = 0;
            int counter = 0;

            IDictionary<int, int[]> infoDict = new Dictionary<int, int[]>();

            /* Each player that joins is assigned a number starting at 0, but if you have two people and the person assigned 0 leaves,
             * the foreach will iterate through 0 only and not 1, causing the leaderboard to be blank like "1. " insead of "1. {name}".
             * We can fix this by setting a hard amount to iterate through, which can be changes and enabled/disabled in the config.
             * 
             * ALT:
             * Each client sends their number to the server, so the server adds it to an array, this loop goes through each array value.
             * 
             * better than anyhting else: playerr.active
             */ 

            for (player = 0; player < conf.leaderboardShowNum; player++)
            {
                // Check that the player exists
                Player playerr = Main.player[player];
                if (playerr.active)
                {
                    dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();
                    // Check the variable 
                    // kills = 0. killsNoS = 1. deaths = 2. playtime = 3. deathsBoss = 4. timeDead = 5
                    infoDict.Add(player, [modPlayer.killCount, modPlayer.killCountNoStatue, modPlayer.deathCount, modPlayer.playTimeSeconds, modPlayer.deathCountBoss, modPlayer.deathTime]);
                }
            }

            // Kills
            if (type == "kills")
            {
                // Sort by descending
                var sortedKills = from entry in infoDict orderby entry.Value[0] descending select entry;
                var sortedKillsStatue = from entry in infoDict orderby entry.Value[1] descending select entry;
                // x and y are used for making sure that there is no blank space printed after each line
                y = sortedKills.Count();

                // For each value
                foreach (KeyValuePair<int, int[]> de in sortedKills)
                {
                    // Counter is used for lb positions
                    counter++;

                    // Name
                    if (type2 == "name")
                    {
                        replyText += counter + ". " + Main.player[de.Key].name;
                        if (++x < y)
                            replyText += "\n";
                    }

                    // Kills
                    else if (type2 == "kills")
                    {
                        replyText += de.Value[0];
                        if (++x < y)
                            replyText += "\n";
                    }

                    // Deaths
                    else if (type2 == "deaths")
                    {
                        replyText += de.Value[2];
                        if (++x < y)
                            replyText += "\n";
                    }

                    // Kill / Death
                    else if (type2 == "k/d")
                    {
                        replyText += Math.Round(de.Value[0] / (double)de.Value[2], 1).ToString();
                        if (++x < y)
                            replyText += "\n";
                    }
                }

                // Reset vars
                x = 0;
                counter = 0;
                foreach (KeyValuePair<int, int[]> de in sortedKillsStatue)
                {
                    // Name Statue
                    counter++;
                    if (type2 == "nameStatue")
                    {
                        replyText += counter + ". " + Main.player[de.Key].name;
                        if (++x < y)
                            replyText += "\n";
                    }

                    // Kills Statue
                    else if (type2 == "killsStatue")
                    {
                        replyText += de.Value[1];
                        if (++x < y)
                            replyText += "\n";
                    }

                    // Deaths Statue
                    else if (type2 == "deathsStatue")
                    {
                        replyText += de.Value[2];
                        if (++x < y)
                            replyText += "\n";
                    }

                    // Kill / Death Statue
                    else if (type2 == "k/dStatue")
                    {
                        replyText += Math.Round(de.Value[1] / (double)de.Value[2], 1).ToString();
                        if (++x < y)
                            replyText += "\n";
                    }
                }

            }

            // Deaths (I dont need to comment this right)
            if (type == "deaths")
            {
                var sortedDeaths = from entry in infoDict orderby entry.Value[2] descending select entry;
                y = sortedDeaths.Count();

                foreach (KeyValuePair<int, int[]> de in sortedDeaths)
                {
                    counter++;
                    if (type2 == "name")
                    {
                        replyText += counter + ". " + Main.player[de.Key].name;
                        if (++x < y)
                            replyText += "\n";
                    }
                    else if (type2 == "deaths")
                    {
                        replyText += de.Value[2];
                        if (++x < y)
                            replyText += "\n";
                    }
                    else if (type2 == "bossDeaths")
                    {
                        replyText += de.Value[4];
                        if (++x < y)
                            replyText += "\n";
                    }
                }
            }

            // Playtime
            if (type == "playtime")
            {
                var sortedPlaytime = from entry in infoDict orderby entry.Value[3] descending select entry;
                y = sortedPlaytime.Count();

                foreach (KeyValuePair<int, int[]> de in sortedPlaytime)
                {
                    counter++;
                    if (type2 == "name")
                    {
                        replyText += counter + ". " + Main.player[de.Key].name;
                        if (++x < y)
                            replyText += "\n";
                    }
                    else if (type2 == "playtime")
                    {
                        // Timespan puts into format 00:00:00
                        TimeSpan time = TimeSpan.FromSeconds(de.Value[3]);
                        string strpt = string.Format("{0}:{1:mm}:{1:ss}", (int)time.TotalHours, time);

                        replyText += strpt;
                        if (++x < y)
                            replyText += "\n";
                    }
                    else if (type2 == "timeDead")
                    {
                        TimeSpan time2 = TimeSpan.FromSeconds(de.Value[5]);
                        string strdt = string.Format("{0}:{1:mm}:{1:ss}", (int)time2.TotalHours, time2);

                        replyText += strdt;
                        if (++x < y)
                            replyText += "\n";
                    }
                }

            }
            // Return the Result
            return replyText;
        }
    }
}

