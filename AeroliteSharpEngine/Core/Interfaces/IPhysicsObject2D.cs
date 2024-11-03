using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core.Interfaces
{
    /// <summary>
    /// Represents a basic physics object (body or particle) that interacts
    /// with other physics objects in the engine.
    /// </summary>
    public interface IPhysicsObject2D : IEquatable<IPhysicsObject2D>
    {
        /// <summary>
        /// Unique identifier for this physics object.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Damping constant to help mitigate numerical errors.
        /// </summary>
        float Damping { get; set; }

        /// <summary>
        /// The position of the objects center of mass.
        /// </summary>
        AeroVec2 Position { get; set; }

        /// <summary>
        /// Stores the previous position of the object. Used for vertlet integration.
        /// </summary>
        AeroVec2 PreviousPosition { get; set; }

        /// <summary>
        /// The linear velocity of the objects center of mass.
        /// </summary>
        AeroVec2 Velocity { get; set; }


        /// <summary>
        /// The linear acceleration of the objects center of mass.
        /// </summary>
        AeroVec2 Acceleration { get; set; }

        /// <summary>
        /// The net force acting on this objects center of mass.
        /// </summary>
        AeroVec2 NetForce { get; set; }

        /// <summary>
        /// Represents the geometrical shape of the object in the physics simulation world.
        /// </summary>
        AeroShape2D Shape { get; }
        
        /// <summary>
        /// The mass of the object, concentrated at its center.
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// 1 / Mass. Cached to speed up calculations.
        /// </summary>
        float InverseMass { get; }

        /// <summary>
        /// Returns whether the object is considered static. Static objects are created by setting the mass
        /// to zero exactly. Static objects do not participate in integration and collision resolution.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Returns whether this object has a non-infinite mass (0.0 is infinite).
        /// </summary>
        bool HasFiniteMass { get; }

        /// <summary>
        /// Adds the supplied force to the objects current net force.
        /// </summary>
        /// <param name="force"></param>
        void ApplyForce(AeroVec2 force);

        /// <summary>
        /// Removes all forces from the object.
        /// </summary>
        void ClearForces();

        /// <summary>
        /// Updates the underlying shape geometry that needs to be transformed based on the current simulation state.
        /// </summary>
        void UpdateGeometry();
    }
}
