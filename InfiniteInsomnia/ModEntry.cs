using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace InfiniteInsomnia
{
    internal sealed class ModEntry : Mod
    {
        public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

        private ModConfig Config;

        private bool endOfDayReached = false;
        private int subtractedMaxStamina = 0;

        public override void Entry(IModHelper helper)
        {
            // load the mods config file
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // event handlers
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // when late lock time
            if (Game1.timeOfDay >= Config.EndOfDayTime)
            {
                if (!endOfDayReached)
                {
                    endOfDayReached = true;

                    // crop the players max stamina, making sure to preserve the removed amount
                    subtractedMaxStamina = Game1.player.MaxStamina - Config.StaminaReducedTo;
                    Game1.player.MaxStamina -= subtractedMaxStamina;

                    // if players current stamina is above the new max it must also be cropped
                    Game1.player.Stamina = (Game1.player.Stamina <= Game1.player.MaxStamina) ? Game1.player.Stamina : Game1.player.MaxStamina;
                }

                Game1.timeOfDay = Utility.ModifyTime(Game1.timeOfDay, -10);
            }
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (endOfDayReached)
            {
                endOfDayReached = false;
                
                // return the cropped max stamina to the player
                Game1.player.MaxStamina += subtractedMaxStamina;
                subtractedMaxStamina = 0;
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Config.FullStaminaAfterSleep)
            {
                Game1.player.Stamina = Game1.player.MaxStamina;
            }
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (endOfDayReached)
            {
                DrawTimeBlock(23, 110, Game1.smallFont, "     Late     ");
            }
        }

        private void DrawTimeBlock(int x, int y, SpriteFont font, string text)
        {
            // get the current display batch
            SpriteBatch spriteBatch = Game1.spriteBatch;

            // get the size taken up by the input text
            Vector2 bounds = font.MeasureString(text);

            // calculate outer coordinates
            int outerWidth = (int)bounds.X + ButtonBorderWidth * 2;
            int outerHeight = (int)bounds.Y + Game1.tileSize / 3;

            // adjust x for right side of screen display
            x = Game1.uiViewport.Width - x - outerWidth;

            // draw texture for the box itself
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White, drawShadow: false);

            Vector2 drawPos = new Vector2(x + ButtonBorderWidth, y + ButtonBorderWidth);

            Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
        }
    }
}
