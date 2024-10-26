using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

public class GravitationalForceGenerator : IForceGenerator
{
    private IPhysicsObject _other;
    private float _gravitationalConstant;
    private const float MIN_DISTANCE = 50f; // Increased minimum distance
    private const float MIN_DISTANCE_SQUARED = MIN_DISTANCE * MIN_DISTANCE;

    public GravitationalForceGenerator(IPhysicsObject other, float gravitationalConstant)
    {
        _other = other;
        _gravitationalConstant = gravitationalConstant;
    }

    public void UpdateForce(IPhysicsObject physicsObject, float dt)
    {
        if (physicsObject.IsStatic || _other.IsStatic) return;

        AeroVec2 direction = _other.Position - physicsObject.Position;
        float distanceSquared = direction.MagnitudeSquared;

        // If objects are too close, skip the force calculation
        if (distanceSquared < float.Epsilon) return;

        // Clamp the distance to prevent extreme forces
        distanceSquared = Math.Max(distanceSquared, MIN_DISTANCE_SQUARED);

        float forceMagnitude = _gravitationalConstant * (physicsObject.Mass * _other.Mass) / distanceSquared;

        // Calculate force direction
        float distance = (float)Math.Sqrt(distanceSquared);
        AeroVec2 force = new AeroVec2(
            direction.X / distance * forceMagnitude,
            direction.Y / distance * forceMagnitude
        );

        physicsObject.ApplyForce(force);
    }
}