using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Integrators;

/// <summary>
/// Verlet integrator mainly to be used for simulating particles. Does not correctly simulate rotational motion for rigid bodies. 
/// </summary>
public class VerletIntegrator : IIntegrator
{
    public void IntegrateLinear(IPhysicsObject2D obj, float dt)
    {
        var acceleration = obj.NetForce * obj.InverseMass;
        var prevPos = obj.Position;
        
        // Verlet integration formula for position
        var newPos = 2 * obj.Position - obj.PreviousPosition + acceleration * dt * dt;
        
        // Update velocity (for external use/forces)
        obj.Velocity = (newPos - obj.Position) / dt;
        
        // Store positions for next step
        obj.PreviousPosition = prevPos;
        obj.Position = newPos;
        
        obj.Velocity *= MathF.Pow(obj.Damping, dt);
        obj.ClearForces();
    }

    public void IntegrateAngular(IPhysicsObject2D obj, float dt)
    {
        // No angular implementation yet. Only intended to be used with AeroParticles.
        return;
    }
}