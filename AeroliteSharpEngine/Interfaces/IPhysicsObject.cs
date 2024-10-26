using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// Represents a basic physics object (body or particle) that interacts
    /// with other physics objects in the engine.
    /// </summary>
    public interface IPhysicsObject
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
        AeroVec2 PreviousPosition { get; set;}  

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
        /// The mass of the object, concetrated at its center.
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// 1 / Mass. Cached to speed up calculations.
        /// </summary>
        float InverseMass { get; }

        /// <summary>
        /// Determines if this object is considered static. Static objects do not move and
        /// integration steps are not performed.
        /// </summary>
        bool IsStatic { get; set; }

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
        /// Removes all forces and/or torques from the object.
        /// </summary>
        void ClearForces();
    }
}
