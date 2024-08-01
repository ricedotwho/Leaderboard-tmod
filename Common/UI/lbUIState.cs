using dethCounter.Common.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace dethCounter.Common.UI
{
    public class lbUIState : UIState
    {
        public DraggableUIPanel leaderboardPanel;
        public UIPanel button1;
        public UIPanel button2;
        public UIPanel button3;

        public UIText nameHeader;
        public UIText statHeader1;
        public UIText statHeader2;

        private UIText name;
        private UIText statText1;
        private UIText statText2;

        public bool Visible = false;
        public bool killType = false;

        private int timer = 0;

        private string activeDisplay = "kills";
        public string nameString = "Loading...";
        public string statString1 = "Loading...";
        public string statString2 = "Loading...";


        public override void OnInitialize()
        {
            var modPlayer = ModContent.GetInstance<dethCounterPlayer>();


            // Create Panel (The whole layer be draggable)
            leaderboardPanel = new DraggableUIPanel();
            leaderboardPanel.SetPadding(0);
            SetRectangle(leaderboardPanel, left: 1200f, top: 400f, width: 508f, height: 600f);
            leaderboardPanel.BackgroundColor = new Color(73, 94, 171);


            // Close Button
            Asset<Texture2D> buttonDeleteTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete");
            UIHoverImageButton closeButton = new UIHoverImageButton(buttonDeleteTexture, Language.GetTextValue("LegacyInterface.52")); // Localized text for "Close"
            SetRectangle(closeButton, left: 481f, top: 30f, width: 22f, height: 22f);
            closeButton.OnLeftClick += new MouseEvent(CloseButtonClicked);
            leaderboardPanel.Append(closeButton);

            // Text button "kills"
            UIPanel button1 = new UIPanel();
            button1.Width.Set(0, 0.3f);
            button1.Height.Set(25, 0);
            button1.HAlign = 0.02f;
            button1.Top.Set(5, 0);
            button1.OnLeftClick += new MouseEvent(KillsButtonClicked);
            leaderboardPanel.Append(button1);

            UIText kills = new UIText("Kills");
            kills.VAlign = 0.5f;
            kills.HAlign = 0.5f;
            button1.Append(kills);


            // Text button "deaths"
            UIPanel button2 = new UIPanel();
            button2.Width.Set(0, 0.3f);
            button2.Height.Set(25, 0);
            button2.HAlign = 0.5f;
            button2.Top.Set(5, 0);
            button2.OnLeftClick += new MouseEvent(DeathsButtonClicked);
            leaderboardPanel.Append(button2);

            UIText deaths = new UIText("Deaths");
            deaths.VAlign = 0.5f;
            deaths.HAlign = 0.5f;
            button2.Append(deaths);


            // Text button "playtime"
            UIPanel button3 = new UIPanel();
            button3.Width.Set(0, 0.3f);
            button3.Height.Set(25, 0);
            button3.HAlign = 0.98f;
            button3.Top.Set(5, 0);
            button3.OnLeftClick += new MouseEvent(PlaytimeButtonClicked);
            leaderboardPanel.Append(button3);

            UIText playtime = new UIText("Playtime");
            playtime.VAlign = 0.5f;
            playtime.HAlign = 0.5f;
            button3.Append(playtime);


            // Name Column
            UIPanel nameColumn = new UIPanel();
            nameColumn.Width.Set(250, 0f);
            nameColumn.Height.Set(0, 0.85f);
            nameColumn.HAlign = 0.05f;
            nameColumn.VAlign = 0.9f;
            nameColumn.BackgroundColor = new Color(48, 72, 166);
            leaderboardPanel.Append(nameColumn);

            // Headers
            nameHeader = new UIText("Name:", 1.5f, false);
            nameHeader.HAlign = 0.05f;
            nameHeader.VAlign = 0.075f;
            leaderboardPanel.Append(nameHeader);

            statHeader1 = new UIText("Kills:", 1.5f, false);
            statHeader1.Left.Set(278, 0f);
            statHeader1.VAlign = 0.075f;
            leaderboardPanel.Append(statHeader1);

            statHeader2 = new UIText("K/D:", 1.5f, false);
            statHeader2.Left.Set(400, 0f);
            statHeader2.VAlign = 0.075f;
            leaderboardPanel.Append(statHeader2);


            // Stat Column 1
            UIPanel statColumn1 = new UIPanel();
            statColumn1.Width.Set(100, 0f);
            statColumn1.Height.Set(0, 0.85f);
            statColumn1.Left.Set(273, 0f);
            statColumn1.VAlign = 0.9f;
            statColumn1.BackgroundColor = new Color(48, 72, 166);
            leaderboardPanel.Append(statColumn1);


            // Stat Column 2
            UIPanel statColumn2 = new UIPanel();
            statColumn2.Width.Set(100, 0f);
            statColumn2.Height.Set(0, 0.85f);
            statColumn2.Left.Set(395, 0f);
            statColumn2.VAlign = 0.9f;
            statColumn2.BackgroundColor = new Color(48, 72, 166);
            leaderboardPanel.Append(statColumn2);


            // Info columns
            // This all needs to have looped info from .OnActivate! (To past me: used another method l bozo)
            name = new UIText("Loading...");
            name.Left.Set(2, 0);
            name.Top.Set(0, 0);
            nameColumn.Append(name);

            statText1 = new UIText("Loading...");
            statText1.HAlign = 0.5f;
            statText1.Top.Set(0, 0);
            statColumn1.Append(statText1);

            statText2 = new UIText("Loading...");
            statText2.HAlign = 0.5f;
            statText2.Top.Set(0, 0);
            statColumn2.Append(statText2);

            // Toggle killtype button
            Asset<Texture2D> tkTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete");
            UIHoverImageButton ToggleKill = new UIHoverImageButton(tkTexture, Language.GetTextValue("Toggle No Statue (Only for kills!)"));
            SetRectangle(ToggleKill, left: 140f, top: 50f, width: 22f, height: 22f);
            ToggleKill.OnLeftClick += new MouseEvent(ToggleKillType);
            leaderboardPanel.Append(ToggleKill);

            // Append Panel
            Append(leaderboardPanel);
        }

        // This is from examplemod ig it helps shorten code
        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        // Close Button
        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<UISystem>().HideMyUI();
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            modPlayer.UIVisible = false;
        }

        // toggle killtype button
        private void ToggleKillType(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();

            if (activeDisplay == "kills")
            {
                if (killType)
                {
                    killType = false;
                    statHeader1.SetText("Kills:", 1.5f, false);
                    name.SetText(modPlayer.killsDataName);
                    statText1.SetText(modPlayer.killsDataKills);
                    statText2.SetText(modPlayer.killsDataKD);
                }
                else
                {
                    killType = true;
                    statHeader1.SetText("Kills (NS):", 1.3f, false);
                    name.SetText(modPlayer.killsDataNameStatue);
                    statText1.SetText(modPlayer.killsDataKillsStatue);
                    statText2.SetText(modPlayer.killsDataKDStatue);
                }
            }
        }

        // Kills button
        public void KillsButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);

            statHeader2.SetText("K/D:", 1.5f, false);

            // Set StatsText
            if (!killType)
            {
                statHeader1.SetText("Kills:", 1.5f, false);
                name.SetText(modPlayer.killsDataName);
                statText1.SetText(modPlayer.killsDataKills);
                statText2.SetText(modPlayer.killsDataKD);
            }
            else
            {
                statHeader1.SetText("Kills (NS):", 1.3f, false);
                name.SetText(modPlayer.killsDataNameStatue);
                statText1.SetText(modPlayer.killsDataKillsStatue);
                statText2.SetText(modPlayer.killsDataKDStatue);
            }

            activeDisplay = "kills";
        }

        // deaths button
        public void DeathsButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);

            statHeader1.SetText("Deaths:", 1.5f, false);
            statHeader2.SetText("Boss:", 1.5f, false);

            name.SetText(modPlayer.deathsDataName);
            statText1.SetText(modPlayer.deathsDataDeaths);
            statText2.SetText(modPlayer.deathsDataBossDeaths);

            activeDisplay = "deaths";
        }

        // playtime button
        public void PlaytimeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            SoundEngine.PlaySound(SoundID.MenuTick);

            statHeader1.SetText("Playtime:", 1.5f, false);
            statHeader2.SetText("Dead:", 1.5f, false);

            name.SetText(modPlayer.playtimeDataName);
            statText1.SetText(modPlayer.playtimeDataPlaytime);
            statText2.SetText(modPlayer.playtimeDataTimedead);

            activeDisplay = "playtime";
        }

        // on .Update (every tick (60 times a second))
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Player player = Main.player[Main.myPlayer];
            dethCounterPlayer modPlayer = player.GetModPlayer<dethCounterPlayer>();
            var conf = ModContent.GetInstance<dethCounterConfig>();

            if (Visible && ++timer > conf.updateTime * 60)
            {
                if (activeDisplay == "deaths")
                {
                    nameString = modPlayer.deathsDataName;
                    statString1 = modPlayer.deathsDataDeaths;
                    statString2 = modPlayer.deathsDataBossDeaths;
                }
                else if (activeDisplay == "playtime")
                {
                    nameString = modPlayer.playtimeDataName;
                    statString1 = modPlayer.playtimeDataPlaytime;
                    statString2 = modPlayer.playtimeDataTimedead;
                }
                else
                {
                    if (!killType)
                    {
                        nameString = modPlayer.killsDataName;
                        statString1 = modPlayer.killsDataKills;
                        statString2 = modPlayer.killsDataKD;
                    }
                    else
                    {
                        nameString = modPlayer.killsDataNameStatue;
                        statString1 = modPlayer.killsDataKillsStatue;
                        statString2 = modPlayer.killsDataKDStatue;
                    }
                }

                name.SetText(nameString);
                statText1.SetText(statString1);
                statText2.SetText(statString2);

                timer = 0;
            }
        }

        // when the ui activates
        public override void OnActivate()
        {
            base.OnActivate();
            Visible = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
        }

        // when the vio deactiveates
        public override void OnDeactivate()
        {
            Visible = false;
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}

