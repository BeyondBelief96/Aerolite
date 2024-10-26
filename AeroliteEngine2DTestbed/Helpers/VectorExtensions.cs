using AeroliteSharpEngine.AeroMath;
using Microsoft.Xna.Framework;

namespace AeroliteEngine2DTestbed.Helpers
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this AeroVec2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static AeroVec2 ToAeroVec2(this Vector2 vec)
        {
            return new AeroVec2(vec.X, vec.Y);
        }
    }
}
