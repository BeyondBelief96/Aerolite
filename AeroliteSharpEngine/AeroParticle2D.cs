using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine
{
    /// <summary>
    /// Represents a point-like particle in 2 Dimension in our physics world.
    /// Particles have some mass, position, velocity, acceleration and can experience
    /// forces from other particles.
    /// </summary>
    public class AeroParticle2D : IPhysicsObject
    {
        #region Properties
        public float Radius { get; set; }
        public AeroVec2 Position { get; set; }
        public AeroVec2 Velocity { get; set; }
        public AeroVec2 Acceleration { get; set; }
        public AeroVec2 NetForces { get; set; }
        public float Mass { get; }
        public float InverseMass { get; }
        public bool IsStatic { get; set; }

        bool IPhysicsObject.HasFiniteMass => InverseMass != 0.0;

        #endregion

        #region Constructor

        public AeroParticle2D(float x, float y, float mass)
        {
            Position = new AeroVec2(x, y);
            Velocity = new AeroVec2();
            Acceleration = new AeroVec2();
            NetForces = new AeroVec2();
            Mass = mass;

            if (Mass != 0.0)
            {
                InverseMass = 1.0f / mass;
            }
            else
            {
                InverseMass = 0.0f;
            }

            Radius = 5;
        }

        public void ApplyForce(AeroVec2 force)
        {
            NetForces += force;
        }

        public void ClearForces()
        {
            NetForces = new AeroVec2();
        }

        public void Integrate(float dt)
        {
            if (IsStatic) return;

            Acceleration = NetForces * InverseMass;

            Velocity += Acceleration * dt;

            Position += Velocity * dt + Acceleration * dt * dt / 2.0f;

            ClearForces();
        }

        #endregion
    }
}
