using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Integrators
{
    public class EulerIntegrator : IIntegrator
    {
        public void Integrate(IPhysicsObject physicsObject, float dt)
        {
            if (physicsObject.IsStatic) return;

            // Basic physics integration for all objects
            physicsObject.Acceleration = physicsObject.NetForce * physicsObject.InverseMass;
            physicsObject.Velocity += physicsObject.Acceleration * dt;
            physicsObject.Position += physicsObject.Velocity * dt;
            physicsObject.Velocity *= MathF.Pow(physicsObject.Damping, dt);

            // Additional integration for bodies (angular motion)
            if (physicsObject is AeroBody2D body)
            {
                body.AngularAcceleration = body.NetTorque * body.InverseInertia;
                body.AngularVelocity += body.AngularAcceleration * dt;
                body.AngularVelocity *= MathF.Pow(body.Damping, dt);
                body.Angle += body.AngularVelocity * dt;
            }

            physicsObject.ClearForces();
        }
    }
}
