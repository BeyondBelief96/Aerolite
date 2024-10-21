using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators
{
    public class DragForceGenerator : IForceGenerator
    {
        /// <summary>
        /// Holds the velocity drag coefficient
        /// </summary>
        public float k1;

        /// <summary>
        /// Holds the velocity squared drag coefficient
        /// </summary>
        public float k2;

        public DragForceGenerator(float k1, float k2)
        {
            this.k1 = k1;
            this.k2 = k2;
        }

        public void UpdateForce(IPhysicsObject physicsObject, float dt)
        {
            if (physicsObject.IsStatic) return;

            AeroVec2 dragForce = new AeroVec2();

            // Calculate the drag coefficient
            float dragCoefficient = k1 * physicsObject.Velocity.Magnitude + k2 * physicsObject.Velocity.MagnitudeSquared;

            // Apply force to object.
            dragForce.Normalize();
            dragForce *= -dragCoefficient;
            physicsObject.ApplyForce(dragForce);
        }
    }
}
