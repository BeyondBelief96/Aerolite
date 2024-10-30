using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Integrators
{
    public class EulerIntegrator : IIntegrator
    {
        public void IntegrateAngular(IPhysicsObject2D physicsObject2D, float dt)
        {
            // Integration for 2-D bodies (angular motion)
            if (physicsObject2D is IBody2D body)
            {
                body.AngularAcceleration = body.NetTorque * body.InverseInertia;
                body.AngularVelocity += body.AngularAcceleration * dt;
                body.AngularVelocity *= MathF.Pow(body.Damping, dt);
                body.Angle += body.AngularVelocity * dt;

                body.ClearTorque();
            }
        }

        public void IntegrateLinear(IPhysicsObject2D physicsObject2D, float dt)
        {
            if (physicsObject2D.IsStatic) return;

            physicsObject2D.Acceleration = physicsObject2D.NetForce * physicsObject2D.InverseMass;
            physicsObject2D.Velocity += physicsObject2D.Acceleration * dt;
            physicsObject2D.Position += physicsObject2D.Velocity * dt;
            physicsObject2D.Velocity *= MathF.Pow(physicsObject2D.Damping, dt);
            physicsObject2D.ClearForces();
        }
    }
}
