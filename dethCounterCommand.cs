using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using dethCounter;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using tModPorter;
using dethCounter.Common.Config;
using Terraria.GameContent;

namespace dethCounter
{
	public class dethCounterCommand : ModCommand
	{

        public override CommandType Type
            => CommandType.World;

        public override string Command
            => "lb";

        public override string Usage
            => "/lb kills|deaths|playtime|killsstatues|help";

        public override string Description
            => "Shows a leaderboard of some stats";


        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var conf = ModContent.GetInstance<dethCounterConfig>();

            int player;
            string replyText = "";
            int x = 0;
            int y = 0;
            int displayNum = 0;
            int maxDiplay = 10;
            int counter = 0;
            int totalDeaths = 0;

            IDictionary<int, int> killsDict = new Dictionary<int, int>();
            IDictionary<int, int> deathsDict = new Dictionary<int, int>();
            //IDictionary<int, string> ptDict = new Dictionary<int, string>();
            IDictionary<int, int> ptDict = new Dictionary<int, int>();
            IDictionary<int, int> kCNSDict = new Dictionary<int, int>();

            if (args.Length > 1)
            {
                throw new UsageException("Usage: /lb kills|deaths|playtime|killsstatues|help");
            }

            if (args[0] == "kills")
            {
                for (player = 0; player < conf.leaderboardShowNum; player++)
                {
                    Player playerr = Main.player[player];
                    if (playerr.active)
                    {
                        dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();

                        killsDict[player] = modPlayer.killCount;
                    }
                }

                //Sort Array
                var sortedKills = from entry in killsDict orderby entry.Value descending select entry;
                y = sortedKills.Count();
                foreach (KeyValuePair<int,int> de in sortedKills)
                {
                    if (displayNum < maxDiplay)
                    {
                        counter++;
                        replyText += counter + ". " + Main.player[de.Key].name + " has " + de.Value + " kills.";
                        if (++x < y)
                            replyText += "\n";

                        displayNum++;
                    }
                }
                caller.Reply(replyText, new Color(0,255,255));
                return;
            }


            if (args[0] == "deaths")
            {
                for (player = 0; player < conf.leaderboardShowNum; player++)
                {
                    Player playerr = Main.player[player];
                    if (playerr.active)
                    {
                        dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();

                        deathsDict[player] = modPlayer.deathCount;
                    }
                }

                //Sort Array
                var sortedDeaths = from entry in deathsDict orderby entry.Value descending select entry;
                y = sortedDeaths.Count();
                foreach (KeyValuePair<int, int> de in sortedDeaths)
                {
                    if (displayNum < maxDiplay)
                    {
                        counter++;
                        replyText += counter + ". " + Main.player[de.Key].name + " has " + de.Value + " deaths.";
                        if (++x < y)
                            replyText += "\n";

                        displayNum++;
                    }
                }
                caller.Reply(replyText, new Color(0, 255, 255));
                return;

            }

            if (args[0] == "playtime")
            {
                for (player = 0; player < conf.leaderboardShowNum; player++)
                {
                    Player playerr = Main.player[player];
                    if (playerr.active)
                    {
                        dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();

                        ptDict[player] = modPlayer.playTimeSeconds;
                    }
                }

                //Sort Array
                var sortedPT = from entry in ptDict orderby entry.Value descending select entry;
                y = sortedPT.Count();
                foreach (KeyValuePair<int, int> de in sortedPT)
                {
                    if (displayNum < maxDiplay)
                    {
                        counter++;
                        TimeSpan time = TimeSpan.FromSeconds(de.Value);
                        string str = string.Format("{0}:{1:mm}:{1:ss}", (int)time.TotalHours, time);

                        replyText += counter + ". " + Main.player[de.Key].name + " has a playtime of " + str;
                        if (++x < y)
                            replyText += "\n";

                        displayNum++;
                    }
                }
                caller.Reply(replyText, new Color(0, 255, 255));
                return;
            }

            if (args[0] == "timedead")
            {
                for (player = 0; player < conf.leaderboardShowNum; player++)
                {
                    Player playerr = Main.player[player];
                    if (playerr.active)
                    {
                        dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();

                        ptDict[player] = modPlayer.deathTime;
                    }
                }

                //Sort Array
                var sortedDT = from entry in ptDict orderby entry.Value descending select entry;
                y = sortedDT.Count();
                foreach (KeyValuePair<int, int> de in sortedDT)
                {
                    if (displayNum < maxDiplay)
                    {
                        counter++;
                        TimeSpan time = TimeSpan.FromSeconds(de.Value);
                        string str = string.Format("{0}:{1:mm}:{1:ss}", (int)time.TotalHours, time);

                        replyText += counter + ". " + Main.player[de.Key].name + " has been contemplating their mistakes for " + str;
                        if (++x < y)
                            replyText += "\n";

                        displayNum++;
                    }
                }
                caller.Reply(replyText, new Color(0, 255, 255));
                return;
            }

            if (args[0] == "totaldeaths")
            {
                for (player = 0; player < Main.maxPlayers; player++)
                {
                    Player playerr = Main.player[player];
                    if (playerr.active)
                    {
                        dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();

                        totalDeaths += modPlayer.deathCount;
                    }
                }
                caller.Reply("Total Deaths: " + totalDeaths, new Color(0, 255, 255));
                return;
            }

            if (args[0] == "killsstatues")
            {
                for (player = 0; player < conf.leaderboardShowNum; player++)
                {
                    Player playerr = Main.player[player];
                    if (playerr.active)
                    {
                        dethCounterPlayer modPlayer = Main.player[player].GetModPlayer<dethCounterPlayer>();

                        kCNSDict[player] = modPlayer.killCountNoStatue;
                    }
                }

                //Sort Array
                var sortedkCNS = from entry in kCNSDict orderby entry.Value descending select entry;
                y = sortedkCNS.Count();
                foreach (KeyValuePair<int, int> de in sortedkCNS)
                {
                    counter++;
                    if (displayNum < maxDiplay)
                    {
                        replyText += counter + ". " + Main.player[de.Key].name + " has " + de.Value + " kills (Not Including Statues).";
                        if (++x < y)
                            replyText += "\n";

                        displayNum++;
                    }
                }
                caller.Reply(replyText, new Color(0, 255, 255));
                return;
            }

            if (args[0] == "help")
            {
                replyText = "Help:\n'/lb kills' - Shows a leaderboard for kills.\n'/lb deaths' - Shows a leaderboard for deaths.\n'/lb playtime' - Shows a leaderboard for playtime.\n'/lb killsstatues' - Shows a leaderboard for kills not including statues.\n'/lb timedead' shows a leaderboard for time spent on the respawn screen\n'/lb totaldeaths' shows the total player deaths\n Additionally, bind a key (default 'P') to open a leaderboard GUI.";
                caller.Reply(replyText, Color.Green);
                return;
            }

            else
            {
                throw new UsageException("Usage: /lb kills|deaths|playtime|killsstatues|help");
            }
        }
    }
}