using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Linq;
using System.Collections;

namespace dethCounter.Common.UI
{
    [Autoload(Side = ModSide.Client)]
    public class UISystem : ModSystem
    {
        private UserInterface lbInterface;
        internal lbUIState lbUI;

        public void ShowMyUI()
        {
            lbInterface?.SetState(lbUI);
        }
        public void HideMyUI()
        {
            lbInterface?.SetState(null);
        }

        public override void Load()
        {
            lbInterface = new UserInterface();
            lbUI = new lbUIState();

            lbUI.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (lbInterface?.CurrentState != null)
                lbInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Leaderboard: Leaderboard Menu",
                    delegate
                    {
                        if (lbInterface?.CurrentState != null)
                            lbInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}

