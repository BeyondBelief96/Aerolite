using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution.Resolvers.Impulse;

/// <summary>
/// Impulse-based collision resolver implementation
/// For reference on implementation details: http://www.chrishecker.com/images/e/e7/Gdmphys3.pdf
/// </summary>
public class ImpulseMethodCollisionResolver : CollisionResolverBase
{
    protected override void ResolveBodyBody(CollisionManifold manifold, IBody2D bodyA, IBody2D bodyB)
    {
        var e = Math.Min(bodyA.Restitution, bodyB.Restitution);
        var f = Math.Min(bodyA.Friction, bodyB.Friction);
        
        var ra = manifold.Contact.EndPoint - bodyA.Position;
        var rb = manifold.Contact.StartPoint - bodyB.Position;
        
        var va = bodyA.Velocity + new AeroVec2(-bodyA.AngularVelocity * ra.Y, bodyA.AngularVelocity * ra.X);
        var vb = bodyB.Velocity + new AeroVec2(-bodyB.AngularVelocity * rb.Y, bodyB.AngularVelocity * rb.X);
        var vRel = va - vb;
        
        // Calculate and apply normal impulse
        var normalImpulse = CalculateNormalImpulse(manifold.Normal, vRel, e, bodyA, bodyB, ra, rb);
        
        // Calculate and apply tangential (friction) impulse
        var tangentialImpulse = CalculateTangentialImpulse(manifold.Normal, vRel, f, bodyA, bodyB, ra, rb);
        
        var totalImpulse = normalImpulse + tangentialImpulse;
        bodyA.ApplyImpulseAtPoint(totalImpulse, ra);
        bodyB.ApplyImpulseAtPoint(-totalImpulse, rb);
    }

    protected override void ResolveBodyParticle(CollisionManifold manifold, IBody2D body, AeroParticle2D particle)
    {
        var e = Math.Min(body.Restitution, particle.Restitution);
        var f = Math.Min(body.Friction, particle.Friction);
        
        var r = manifold.Contact.StartPoint - body.Position;
        
        var vBody = body.Velocity + new AeroVec2(-body.AngularVelocity * r.Y, body.AngularVelocity * r.X);
        var vRel = particle.Velocity - vBody;
        
        var normal = manifold.Normal;
        var vRelNormal = vRel.Dot(normal);
        
        // Normal impulse
        var denominatorN = particle.InverseMass + body.InverseMass + 
            (r.Cross(normal) * r.Cross(normal)) * body.InverseInertia;
        var jn = normal * (-(1 + e) * vRelNormal / denominatorN);
        
        // Tangential impulse
        var tangent = normal.Normal();
        var vRelTangent = vRel.Dot(tangent);
        var denominatorT = particle.InverseMass + body.InverseMass +
            (r.Cross(tangent) * r.Cross(tangent)) * body.InverseInertia;
        var jt = tangent * (f * -(1 + e) * vRelTangent / denominatorT);
        
        var j = jn + jt;
        particle.ApplyImpulse(j);
        body.ApplyImpulseAtPoint(-j, r);
    }

    protected override void ResolveParticleParticle(CollisionManifold manifold, AeroParticle2D particleA, AeroParticle2D particleB)
    {
        var e = Math.Min(particleA.Restitution, particleB.Restitution);
        var vRel = particleA.Velocity - particleB.Velocity;
        
        var vRelNormal = vRel.Dot(manifold.Normal);
        var impulseMagnitude = (-(1 + e) * vRelNormal) / (particleA.InverseMass + particleB.InverseMass);
        var impulse = manifold.Normal * impulseMagnitude;
        
        particleA.ApplyImpulse(impulse);
        particleB.ApplyImpulse(-impulse);
    }

    private static AeroVec2 CalculateNormalImpulse(
        AeroVec2 normal, AeroVec2 vRel, float e,
        IBody2D bodyA, IBody2D bodyB,
        AeroVec2 ra, AeroVec2 rb)
    {
        var vRelNormal = vRel.Dot(normal);
        var raCrossN = ra.Cross(normal);
        var rbCrossN = rb.Cross(normal);
        
        var denominator = bodyA.InverseMass + bodyB.InverseMass +
            (raCrossN * raCrossN) * bodyA.InverseInertia +
            (rbCrossN * rbCrossN) * bodyB.InverseInertia;
            
        var magnitude = (-(1 + e) * vRelNormal) / denominator;
        return normal * magnitude;
    }

    private static AeroVec2 CalculateTangentialImpulse(
        AeroVec2 normal, AeroVec2 vRel, float f,
        IBody2D bodyA, IBody2D bodyB,
        AeroVec2 ra, AeroVec2 rb)
    {
        var tangent = normal.Normal();
        var vRelTangent = vRel.Dot(tangent);
        var raCrossT = ra.Cross(tangent);
        var rbCrossT = rb.Cross(tangent);
        
        var denominator = bodyA.InverseMass + bodyB.InverseMass +
            (raCrossT * raCrossT) * bodyA.InverseInertia +
            (rbCrossT * rbCrossT) * bodyB.InverseInertia;
            
        var magnitude = f * -(1 + f) * vRelTangent / denominator;
        return tangent * magnitude;
    }
}