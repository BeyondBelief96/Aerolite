using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Core
{
    /// <summary>
    /// Represents a point-like particle in 2-Dimensions in our physics world.
    /// Particles have some mass, position, velocity, acceleration and can experience
    /// forces from other particles. Particles are point like masses with all of their
    /// mass concentrated at the center of the particle.
    /// </summary>
    public class AeroParticle2D : Physics2DObjectBase
    {
        public float Radius { get; set; }

        public AeroParticle2D(float x, float y, float mass) : base(mass)
        {
            Position = new AeroVec2(x, y);
            PreviousPosition = new AeroVec2(x, y);
            Radius = 5;
        }
    }
}
