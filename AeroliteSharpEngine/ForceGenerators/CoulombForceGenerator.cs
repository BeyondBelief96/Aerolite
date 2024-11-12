using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators;

/// <summary>
/// Generates Coulomb forces between charged particles
/// </summary>
public class CoulombForceGenerator : IForceGenerator
{
    private readonly IChargedObject _other;
    private const float COULOMB_CONSTANT = 8.99e9f; // k = 1/(4πε₀) in N⋅m²/C²
    private readonly float _scaleFactor; // To scale the force for game purposes
    private const float MIN_DISTANCE = 1f; // Minimum distance to prevent extreme forces
    private const float MIN_DISTANCE_SQUARED = MIN_DISTANCE * MIN_DISTANCE;

    public CoulombForceGenerator(IChargedObject other, float scaleFactor = 1f)
    {
        _other = other;
        _scaleFactor = scaleFactor;
    }

    public void UpdateForce(IPhysicsObject2D physicsObject2D, float dt)
    {
        if (physicsObject2D is not IChargedObject chargeableObject || 
            physicsObject2D.IsStatic) return;

        // Calculate direction vector between charges (from other to this)
        AeroVec2 direction = physicsObject2D.Position - ((IPhysicsObject2D)_other).Position;
        float distanceSquared = direction.MagnitudeSquared;

        // Skip if objects are too close or same object
        if (AeroMathExtensions.IsNearlyZero(distanceSquared)) return;

        // Clamp the distance to prevent extreme forces
        distanceSquared = Math.Max(distanceSquared, MIN_DISTANCE_SQUARED);

        // Calculate Coulomb force magnitude: F = k(q1*q2)/r²
        float forceMagnitude = COULOMB_CONSTANT * _scaleFactor * 
            (chargeableObject.Charge * _other.Charge) / distanceSquared;

        // Calculate force direction (normalized)
        float distance = (float)Math.Sqrt(distanceSquared);
        AeroVec2 force = new(
            direction.X / distance * forceMagnitude,
            direction.Y / distance * forceMagnitude
        );

        physicsObject2D.ApplyForce(force);
    }
}