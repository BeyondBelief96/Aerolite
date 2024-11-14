using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Integrators;

public class RK4Integrator : IIntegrator
{
    public void IntegrateAngular(IPhysicsObject2D physicsObject2D, float dt)
    {
        if (physicsObject2D is not IBody2D body) return;

        // Current state
        float angle = body.Angle;
        float angularVel = body.AngularVelocity;
        float torque = body.NetTorque;
        float invI = body.InverseInertia;

        // k1 = f(t, y)
        float k1_vel = torque * invI;
        float k1_pos = angularVel;

        // k2 = f(t + dt/2, y + dt/2 * k1)
        float k2_vel = torque * invI;
        float k2_pos = angularVel + k1_vel * dt / 2;

        // k3 = f(t + dt/2, y + dt/2 * k2)
        float k3_vel = torque * invI;
        float k3_pos = angularVel + k2_vel * dt / 2;

        // k4 = f(t + dt, y + dt * k3)
        float k4_vel = torque * invI;
        float k4_pos = angularVel + k3_vel * dt;

        // Update angular velocity
        body.AngularVelocity += (k1_vel + 2 * k2_vel + 2 * k3_vel + k4_vel) * dt / 6.0f;
        
        // Update angle
        body.Angle += (k1_pos + 2 * k2_pos + 2 * k3_pos + k4_pos) * dt / 6.0f;

        body.AngularVelocity *= MathF.Pow(body.Damping, dt);
        body.ClearTorque();
    }

    public void IntegrateLinear(IPhysicsObject2D physicsObject2D, float dt)
    {
        // Current state
        var position = physicsObject2D.Position;
        var velocity = physicsObject2D.Velocity;
        var force = physicsObject2D.NetForce;
        var invMass = physicsObject2D.InverseMass;

        // k1 = f(t, y)
        var k1_vel = force * invMass;
        var k1_pos = velocity;

        // k2 = f(t + dt/2, y + dt/2 * k1)
        var k2_vel = force * invMass;
        var k2_pos = velocity + k1_vel * (dt / 2);

        // k3 = f(t + dt/2, y + dt/2 * k2)
        var k3_vel = force * invMass;
        var k3_pos = velocity + k2_vel * (dt / 2);

        // k4 = f(t + dt, y + dt * k3)
        var k4_vel = force * invMass;
        var k4_pos = velocity + k3_vel * dt;

        // Update velocity
        physicsObject2D.Velocity += (k1_vel + 2 * k2_vel + 2 * k3_vel + k4_vel) * dt / 6.0f;
        
        // Update position
        physicsObject2D.Position += (k1_pos + 2 * k2_pos + 2 * k3_pos + k4_pos) * dt / 6.0f;

        physicsObject2D.Velocity *= MathF.Pow(physicsObject2D.Damping, dt);
        physicsObject2D.ClearForces();
    }
}