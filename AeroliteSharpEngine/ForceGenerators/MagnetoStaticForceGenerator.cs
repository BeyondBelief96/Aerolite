using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators;

/// <summary>
/// Generates forces on charged particles moving in a magnetic field
/// </summary>
public class MagnetoStaticForceGenerator : IForceGenerator
{
    private readonly float _fieldStrength; // Magnitude of the magnetic field
    private readonly float _scaleFactor; // To scale the force for game purposes

    public MagnetoStaticForceGenerator(float fieldStrength, float scaleFactor = 1f)
    {
        _fieldStrength = fieldStrength;
        _scaleFactor = scaleFactor;
    }

    public void UpdateForce(IPhysicsObject2D physicsObject2D, float dt)
    {
        if (physicsObject2D is not IChargedObject chargeableObject || 
            physicsObject2D.IsStatic) return;

        // Calculate Lorentz force: F = qv × B
        // In 2D, this creates a force perpendicular to both velocity and magnetic field
        float forceMagnitude = chargeableObject.Charge * _fieldStrength * 
                               physicsObject2D.Velocity.Magnitude * _scaleFactor;

        // In 2D, the cross product with B field (pointing out of plane) 
        // results in a force perpendicular to velocity
        AeroVec2 force = new(
            -physicsObject2D.Velocity.Y * forceMagnitude,
            physicsObject2D.Velocity.X * forceMagnitude
        );

        physicsObject2D.ApplyForce(force);
    }
}