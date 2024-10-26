using AeroliteSharpEngine.AeroMath;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers
{
    public static class CoordinateSystem
    {
        /// <summary>
        /// Converts screen space coordinates (0,0 at top-left) to render space coordinates (0,0 at center)
        /// Used primarily for drawing with _shapes functions
        /// </summary>
        public static Vector2 ScreenToRender(Vector2 screenPos, int screenWidth, int screenHeight)
        {
            return new Vector2(
                screenPos.X - screenWidth / 2,     // Shift to make center 0
                screenHeight / 2 - screenPos.Y     // Invert Y and shift to make center 0
            );
        }

        /// <summary>
        /// Converts render space coordinates (0,0 at center) to screen space coordinates (0,0 at top-left)
        /// Used when you need to convert from _shapes coordinate system back to screen space
        /// </summary>
        public static Vector2 RenderToScreen(Vector2 renderPos, int screenWidth, int screenHeight)
        {
            return new Vector2(
                renderPos.X + screenWidth / 2,     // Shift back to screen space
                screenHeight / 2 - renderPos.Y     // Invert Y and shift back to screen space
            );
        }

        // Helper methods for AeroVec2
        public static Vector2 ScreenToRender(AeroVec2 screenPos, int screenWidth, int screenHeight)
        {
            return ScreenToRender(new Vector2(screenPos.X, screenPos.Y), screenWidth, screenHeight);
        }

        public static AeroVec2 RenderToScreen(AeroVec2 renderPos, int screenWidth, int screenHeight)
        {
            Vector2 result = RenderToScreen(new Vector2(renderPos.X, renderPos.Y), screenWidth, screenHeight);
            return new AeroVec2(result.X, result.Y);
        }

        /// <summary>
        /// Get the renderable bounds of the screen in render space coordinates
        /// Useful for knowing the limits of the render area
        /// </summary>
        public static (float left, float right, float bottom, float top) GetRenderBounds(int screenWidth, int screenHeight)
        {
            float halfWidth = screenWidth / 2.0f;
            float halfHeight = screenHeight / 2.0f;
            return (-halfWidth, halfWidth, -halfHeight, halfHeight);
        }
    }
}