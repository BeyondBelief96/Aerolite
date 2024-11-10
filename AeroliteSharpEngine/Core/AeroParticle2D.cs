using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core
{
    /// <summary>
    /// Represents a point-like particle in 2-Dimensions in our physics world.
    /// Particles have some mass, position, velocity, acceleration and can experience
    /// forces from other particles. Particles are point like masses with all of their
    /// mass concentrated at the center of the particle.
    /// </summary>
    public class AeroParticle2D : Physics2DObject2DBase
    {
        public AeroParticle2D(float x, float y, float mass, 
            float restitution = 0.5f, float friction = 0.5f, float radius = 5.0f) : base(mass, new AeroCircle(radius), restitution, friction)
        {
            Position = new AeroVec2(x, y);
            PreviousPosition = new AeroVec2(x, y);
        }
    }
}
