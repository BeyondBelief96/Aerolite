using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core.Interfaces
{
    /// <summary>
    /// Represents a 2-Dimensional rigid body. This body can have both linear and angular motion.
    /// </summary>
    public interface IBody2D : IPhysicsObject2D
    {
        /// <summary>
        /// Angle of the body with respect to horizontal axis.
        /// </summary>
        float Angle { get; set; }

        /// <summary>
        /// Previous angle of the body. Used for verlet integration.
        /// </summary>
        float PreviousAngle { get; set; }

        /// <summary>
        /// Angular velocity of the body. In 2D we represent this as a float intead of a vector.
        /// </summary>
        float AngularVelocity { get; set; }

        /// <summary>
        /// Angular acceleration of the body. In 2D we represent this as a float instead of a vector.
        /// </summary>
        float AngularAcceleration { get; set; }

        /// <summary>
        /// Represents the net amount of torque applied to the body. In 2D torque is represented as a scalar instead of a vector.
        /// </summary>
        float NetTorque { get; set; }

        /// <summary>
        /// The moment of inertia of the body.
        /// </summary>
        float Inertia { get; }

        /// <summary>
        /// 1 / Inertia. Caching this speeds up calculations.
        /// </summary>
        float InverseInertia { get; }

        /// <summary>
        /// Applies the given torque to the body changing its angular acceleration.
        /// </summary>
        /// <param name="torque"></param>
        void ApplyTorque(float torque);

        /// <summary>
        /// Removes all torque from the object.
        /// </summary>
        void ClearTorque();
    }
}
