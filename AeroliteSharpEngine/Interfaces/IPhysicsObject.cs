using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// Represents a basic physics object (body or particle) that interacts
    /// with other physics objects in the engine.
    /// </summary>
    public interface IPhysicsObject
    {
        AeroVec2 Position { get; set; }

        AeroVec2 Velocity { get; set; }

        AeroVec2 Acceleration { get; set; }

        AeroVec2 NetForces { get; set; }

        float Mass { get; }
        
        float InverseMass { get; }

        bool IsStatic { get; set; }

        bool HasFiniteMass { get; }

        void ApplyForce(AeroVec2 force);

        void Integrate(float dt);
    }
}
