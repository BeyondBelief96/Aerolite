using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators;

public class GravitationalForceGenerator : IForceGenerator
{
    private IPhysicsObject2D _other;
    private float _gravitationalConstant;
    private const float MIN_DISTANCE = 50f; // Increased minimum distance
    private const float MIN_DISTANCE_SQUARED = MIN_DISTANCE * MIN_DISTANCE;

    public GravitationalForceGenerator(IPhysicsObject2D other, float gravitationalConstant)
    {
        _other = other;
        _gravitationalConstant = gravitationalConstant;
    }

    public void UpdateForce(IPhysicsObject2D physicsObject2D, float dt)
    {
        if (physicsObject2D.IsStatic || _other.IsStatic) return;

        AeroVec2 direction = _other.Position - physicsObject2D.Position;
        float distanceSquared = direction.MagnitudeSquared;

        // If objects are too close, skip the force calculation
        if (AeroMathExtensions.IsNearlyZero(distanceSquared)) return;

        // Clamp the distance to prevent extreme forces
        distanceSquared = Math.Max(distanceSquared, MIN_DISTANCE_SQUARED);

        float forceMagnitude = _gravitationalConstant * (physicsObject2D.Mass * _other.Mass) / distanceSquared;

        // Calculate force direction
        float distance = (float)Math.Sqrt(distanceSquared);
        AeroVec2 force = new AeroVec2(
            direction.X / distance * forceMagnitude,
            direction.Y / distance * forceMagnitude
        );

        physicsObject2D.ApplyForce(force);
    }
}