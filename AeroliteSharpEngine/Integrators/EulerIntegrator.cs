using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Integrators;

public class EulerIntegrator : IIntegrator
{
    public void IntegrateLinear(IPhysicsObject2D obj, float dt)
    {
        if (obj.IsStatic) return;

        // First calculate new velocity using current forces
        var acceleration = obj.NetForce * obj.InverseMass;
        obj.Velocity += acceleration * dt;
        
        // Apply damping to velocity
        obj.Velocity *= MathF.Pow(obj.Damping, dt);
        
        // Then update position using new velocity
        obj.Position += obj.Velocity * dt;
        
        // Clear forces for next frame
        obj.ClearForces();
    }

    public void IntegrateAngular(IPhysicsObject2D obj, float dt)
    {
        if (obj is not IBody2D body || body.IsStatic) return;

        // First calculate new angular velocity using current torque
        var angularAcceleration = body.NetTorque * body.InverseInertia;
        body.AngularVelocity += angularAcceleration * dt;
        
        // Apply damping to angular velocity
        body.AngularVelocity *= MathF.Pow(body.Damping, dt);
        
        // Then update angle using new angular velocity
        body.Angle += body.AngularVelocity * dt;
        
        // Clear torque for next frame
        body.ClearTorque();
    }
}