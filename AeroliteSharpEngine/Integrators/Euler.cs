using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Integrators
{
    public class EulerIntegrator : IIntegrator
    {
        public void IntegrateAngular(IPhysicsObject physicsObject, float dt)
        {
            // Integration for 2-D bodies (angular motion)
            if (physicsObject is IBody2D body)
            {
                body.AngularAcceleration = body.NetTorque * body.InverseInertia;
                body.AngularVelocity += body.AngularAcceleration * dt;
                body.AngularVelocity *= MathF.Pow(body.Damping, dt);
                body.Angle += body.AngularVelocity * dt;

                body.ClearTorque();
            }
        }

        public void IntegrateLinear(IPhysicsObject physicsObject, float dt)
        {
            if (physicsObject.IsStatic) return;

            physicsObject.Acceleration = physicsObject.NetForce * physicsObject.InverseMass;
            physicsObject.Velocity += physicsObject.Acceleration * dt;
            physicsObject.Position += physicsObject.Velocity * dt;
            physicsObject.Velocity *= MathF.Pow(physicsObject.Damping, dt);
            physicsObject.ClearForces();
        }
    }
}
