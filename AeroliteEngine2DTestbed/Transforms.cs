using AeroliteSharpEngine.AeroMath;
using Flat.Graphics;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed
{
    public static class Transforms
    {
        public static Vector2 WorldToScreen(float worldX, float worldY, Screen screen)
        {
            return new Vector2(
            worldX - screen.Width / 2,   // Translate from world to screen coordinates
                screen.Height / 2 - worldY  // Invert Y and translate
            );
        }

        public static Vector2 WorldToScreen(AeroVec2 worldVector, Screen screen)
        {
            return new Vector2(
            worldVector.X - screen.Width / 2,   // Translate from world to screen coordinates
                screen.Height / 2 - worldVector.Y  // Invert Y and translate
            );
        }

        public static Vector2 ScreenToWorld(float screenX, float screenY, Screen screen)
        {
            return new Vector2(
            screenX + screen.Width / 2,   // Translate from screen to world coordinates
                -screenY + screen.Height / 2  // Invert Y and translate
            );
        }

        public static Vector2 ScreenToWorld(AeroVec2 screenVector, Screen screen)
        {
            return new Vector2(
            screenVector.X + screen.Width / 2,   // Translate from screen to world coordinates
                -screenVector.Y + screen.Height / 2  // Invert Y and translate
            );
        }

        public static Vector2 WorldToScreen(float worldX, float worldY, int screenWidth, int screenHeight)
        {
            return new Vector2(
            worldX - screenWidth / 2,   // Translate from world to screen coordinates
                screenHeight / 2 - worldY  // Invert Y and translate
            );
        }

        public static Vector2 WorldToScreen(AeroVec2 worldVector, int screenWidth, int screenHeight)
        {
            return new Vector2(
            worldVector.X - screenWidth / 2,   // Translate from world to screen coordinates
                screenHeight / 2 - worldVector.Y  // Invert Y and translate
            );
        }

        public static Vector2 ScreenToWorld(float screenX, float screenY, int screenWidth, int screenHeight)
        {
            return new Vector2(
            screenX + screenWidth / 2,   // Translate from screen to world coordinates
                -screenY + screenHeight / 2  // Invert Y and translate
            );
        }

        public static Vector2 ScreenToWorld(AeroVec2 screenVector, int screenWidth, int screenHeight)
        {
            return new Vector2(
            screenVector.X + screenWidth / 2,   // Translate from screen to world coordinates
                -screenVector.Y + screenHeight / 2  // Invert Y and translate
            );
        }
    }
}
