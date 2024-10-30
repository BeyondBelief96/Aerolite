using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
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

        public void UpdateForce(IPhysicsObject2D physicsObject2D, float dt)
        {
            if (physicsObject2D.IsStatic) return;

            AeroVec2 dragForce = new AeroVec2();

            // Calculate the drag coefficient
            float dragCoefficient = k1 * physicsObject2D.Velocity.Magnitude + k2 * physicsObject2D.Velocity.MagnitudeSquared;

            // Apply force to object.
            dragForce.Normalize();
            dragForce *= -dragCoefficient;
            physicsObject2D.ApplyForce(dragForce);
        }
    }
}
