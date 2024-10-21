using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators
{
    public class GravitationalForceGenerator : IForceGenerator
    {
        private IPhysicsObject _other;
        private float _gravitationalConstant;

        public GravitationalForceGenerator(IPhysicsObject other, float gravitationalConstant)
        {
            _other = other;
            _gravitationalConstant = gravitationalConstant;
        }

        public void UpdateForce(IPhysicsObject physicsObject, float dt)
        {
            // Don't generate force if either object is static
            if (physicsObject.IsStatic || _other.IsStatic) return;

            // Calculate the direction vector
            AeroVec2 direction = new AeroVec2(
                _other.Position.X - physicsObject.Position.X,
                _other.Position.Y - physicsObject.Position.Y
            );

            float distanceSquared = direction.MagnitudeSquared;

            // Check for very close objects to prevent extreme forces
            if (distanceSquared < 100f)
            {
                return;
            }

            // Calculate the force magnitude using Newton's law of universal gravitation
            float forceMagnitude = _gravitationalConstant * (physicsObject.Mass * _other.Mass) / distanceSquared;

            // Apply the force in the calculated direction
            AeroVec2 force = direction.UnitVector() * forceMagnitude;
            physicsObject.ApplyForce(force);
            _other.ApplyForce(-force);
        }
    }
}