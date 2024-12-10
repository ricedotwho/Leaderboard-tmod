using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace dethCounter
{
    public class dethCounter : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            // Message Types
            DeathCountModMessageType msgType = (DeathCountModMessageType)reader.ReadByte();
            switch (msgType)
            {
                // This is sent from SyncPlayer
                case DeathCountModMessageType.SyncPlayerDeath:
                    byte playernumber = reader.ReadByte();
                    dethCounterPlayer modPlayer = Main.player[playernumber].GetModPlayer<dethCounterPlayer>();
                    modPlayer.playerName = reader.ReadString();
                    modPlayer.killCount = reader.ReadInt32();
                    modPlayer.killCountNoStatue = reader.ReadInt32();
                    modPlayer.deathCount = reader.ReadInt32();
                    modPlayer.deathCountBoss = reader.ReadInt32();
                    modPlayer.deathTime = reader.ReadInt32();
                    modPlayer.playTimeSeconds = reader.ReadInt32();

                    // Syncplayer will send this data automatically
                    break;

                // This is sent from SendClientChanges
                case DeathCountModMessageType.UpdateData:
                    byte playernumber2 = reader.ReadByte();
                    dethCounterPlayer modPlayer2 = Main.player[playernumber2].GetModPlayer<dethCounterPlayer>();
                    modPlayer2.playerName = reader.ReadString();
                    modPlayer2.killCount = reader.ReadInt32();
                    modPlayer2.killCountNoStatue = reader.ReadInt32();
                    modPlayer2.deathCount = reader.ReadInt32();
                    modPlayer2.deathCountBoss = reader.ReadInt32();
                    modPlayer2.deathTime = reader.ReadInt32();
                    modPlayer2.playTimeSeconds = reader.ReadInt32();

                    // Forward Data to clients
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var sys = ModContent.GetInstance<dethCounterSystem>();

                        var packet = GetPacket();
                        packet.Write((byte)DeathCountModMessageType.UpdateData);
                        packet.Write(playernumber2);
                        packet.Write(Main.player[playernumber2].name);
                        packet.Write(modPlayer2.killCount);
                        packet.Write(modPlayer2.killCountNoStatue);
                        packet.Write(modPlayer2.deathCount);
                        packet.Write(modPlayer2.deathCountBoss);
                        packet.Write(modPlayer2.deathTime);
                        packet.Write(modPlayer2.playTimeSeconds);
                        packet.Send(-1, playernumber2);

                        if (!sys.playerNames.Contains(Main.player[playernumber2].name))
                        {
                            sys.playerNames.Add(Main.player[playernumber2].name);
                        }
                    }
                    break;

                // Read the data sent by server
                case DeathCountModMessageType.SendClientPacket:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        dethCounterPlayer modPlayer3 = Main.player[Main.myPlayer].GetModPlayer<dethCounterPlayer>();

                        // Kills
                        modPlayer3.killsDataName = reader.ReadString();
                        modPlayer3.killsDataKills = reader.ReadString();
                        modPlayer3.killsDataDeaths = reader.ReadString();
                        modPlayer3.killsDataKD = reader.ReadString();
                        // Statue
                        modPlayer3.killsDataNameStatue = reader.ReadString();
                        modPlayer3.killsDataKillsStatue = reader.ReadString();
                        modPlayer3.killsDataDeathsStatue = reader.ReadString();
                        modPlayer3.killsDataKDStatue = reader.ReadString();

                        // Deaths
                        modPlayer3.deathsDataName = reader.ReadString();
                        modPlayer3.deathsDataDeaths = reader.ReadString();
                        modPlayer3.deathsDataBossDeaths = reader.ReadString();

                        // Playtime
                        modPlayer3.playtimeDataName = reader.ReadString();
                        modPlayer3.playtimeDataPlaytime = reader.ReadString();
                        modPlayer3.playtimeDataTimedead = reader.ReadString();
                    }
                    break;
                default:
                    Logger.WarnFormat("DethCounter: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
    internal enum DeathCountModMessageType : byte
    {
        SyncPlayerDeath,
        UpdateData,
        SendClientPacket,
    }
}
