using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Resolution.Resolvers;

public class ImpulseMethodCollisionResolver : ICollisionResolver
{
    private readonly ProjectionCollisionResolver  _projectionResolver = new();
    public void ResolveCollisions(IReadOnlyList<CollisionManifold> collisionManifolds)
    {
        _projectionResolver.ResolveCollisions(collisionManifolds);
        foreach (var collisionManifold in collisionManifolds)
        {
            ApplyLinearAndAngularImpulse(collisionManifold);
        }
    }
    
    private static void ApplyLinearImpulse(CollisionManifold manifold)
    {
        var objectA = manifold.ObjectA;
        var objectB = manifold.ObjectB;
        
        var e = Math.Min(manifold.ObjectA.Restitution, manifold.ObjectB.Restitution);
        var vRel = objectA.Velocity - objectB.Velocity;
        
        var vRelNormal = vRel.Dot(manifold.Normal);
        var impulseDirectionN = manifold.Normal;
        var impulseMagnitudeN = (-(1 + e) * vRelNormal) / (objectA.InverseMass + objectB.InverseMass);
        var jn = impulseDirectionN * impulseMagnitudeN;
        
        objectA.ApplyImpulse(jn);
        objectB.ApplyImpulse(-jn);
    }
    
    private static void ApplyLinearAndAngularImpulse(CollisionManifold manifold)
    {
        if (manifold.ObjectA is not IBody2D objectA || manifold.ObjectB is not IBody2D objectB) return;
        
        var e = Math.Min(manifold.ObjectA.Restitution, manifold.ObjectB.Restitution);
        var f = Math.Min(manifold.ObjectA.Friction, manifold.ObjectB.Friction);
        
        var ra = manifold.Contact.EndPoint - objectA.Position;
        var rb = manifold.Contact.StartPoint - objectB.Position;
        
        // Linear + Angular velocities at the contact points.
        var va = objectA.Velocity + new AeroVec2(-objectA.AngularVelocity * ra.Y, objectA.AngularVelocity * ra.X);
        var vb = objectB.Velocity + new AeroVec2(-objectB.AngularVelocity * rb.Y, objectB.AngularVelocity * rb.X);
        var vRel = va - vb;
        
        var vRelNormal = vRel.Dot(manifold.Normal); 
        
        var impulseDirectionN = manifold.Normal;
        
        // Reference article: http://www.chrishecker.com/images/e/e7/Gdmphys3.pdf
        var impulseMagnitudeN = 
            (-(1 + e) * vRelNormal) / (objectA.InverseMass + objectB.InverseMass + 
                                       ((ra.Cross(impulseDirectionN) * ra.Cross(impulseDirectionN)) * objectA.InverseInertia + 
                                        (rb.Cross(impulseDirectionN) * rb.Cross(impulseDirectionN)) * objectB.InverseInertia));
        var jn = impulseDirectionN * impulseMagnitudeN;
        
                
        // Calculate impulse along tangent.
        var tangent = manifold.Normal.Normal();
        var vRelTangent = vRel.Dot(tangent);
        var impulseDirectionT = tangent;
        var impulseMagnitudeT = f * -(1 + e) * vRelTangent / (objectA.InverseMass + objectB.InverseMass + 
                                                              ((ra.Cross(impulseDirectionT) * ra.Cross(impulseDirectionT)) * objectA.InverseInertia + 
                                                               (rb.Cross(impulseDirectionT) * rb.Cross(impulseDirectionT)) * objectB.InverseInertia));
        
        var jt = impulseDirectionT * impulseMagnitudeT;

        var j = jn + jt;
        objectA.ApplyImpulseAtPoint(j, ra);
        objectB.ApplyImpulseAtPoint(-j, rb);
    }

}