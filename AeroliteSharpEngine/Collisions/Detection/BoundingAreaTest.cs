using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection;

/// <summary>
/// Static class containing all intersection tests between different bounding areas
/// </summary>
public static class BoundingAreaTests
{
    private static readonly Dictionary<int, IBoundingArea> BoundingAreaCache = new();
    private static readonly Dictionary<AeroShape2D, float> MaxExtentCache = new();
    
    // Central test dispatcher
    public static bool Test(IBoundingArea a, IBoundingArea b)
    {
        return (a, b) switch
        {
            (AABB2D aabb1, AABB2D aabb2) => TestAABBAABB(aabb1, aabb2),
            (AABB2D aabb, BoundingCircle circle) => TestAABBCircle(aabb, circle),
            (BoundingCircle circle, AABB2D aabb) => TestAABBCircle(aabb, circle),
            (BoundingCircle circle1, BoundingCircle circle2) => TestCircleCircle(circle1, circle2),
            _ => throw new ArgumentException("Unknown bounding area combination")
        };
    }

    // Individual test implementations
    private static bool TestAABBAABB(AABB2D a, AABB2D b)
    {
        var dx = Math.Abs(a.Center.X - b.Center.X);
        var dy = Math.Abs(a.Center.Y - b.Center.Y);
        
        return dx <= (a.HalfExtents.X + b.HalfExtents.X) && 
               dy <= (a.HalfExtents.Y + b.HalfExtents.Y);
    }
    
    private static bool TestCircleCircle(BoundingCircle a, BoundingCircle b)
    {
        var distanceSquared = (a.Center - b.Center).MagnitudeSquared;
        var radiusSum = a.Radius + b.Radius;
        return distanceSquared <= radiusSum * radiusSum;
    }
    
    private static bool TestAABBCircle(AABB2D aabb, BoundingCircle circle)
    {
        // Find closest point on AABB to circle center
        var closestX = Math.Max(aabb.Center.X - aabb.HalfExtents.X, 
            Math.Min(circle.Center.X, aabb.Center.X + aabb.HalfExtents.X));
        var closestY = Math.Max(aabb.Center.Y - aabb.HalfExtents.Y, 
            Math.Min(circle.Center.Y, aabb.Center.Y + aabb.HalfExtents.Y));

        var dx = closestX - circle.Center.X;
        var dy = closestY - circle.Center.Y;
        var distanceSquared = dx * dx + dy * dy;

        return distanceSquared <= (circle.Radius * circle.Radius);
    }

    // High level test with caching and early out
    public static bool TestIntersection(IPhysicsObject a, IPhysicsObject b)
    {
        // Early out - rough distance check
        var centerDistance = (b.Position - a.Position).MagnitudeSquared;
        var maxSeparation = GetMaxExtent(a) + GetMaxExtent(b);
        
        if (centerDistance > maxSeparation * maxSeparation)
        {
            return false;
        }

        // Get/create bounding areas
        var boundA = GetOrCreateBoundingArea(a);
        var boundB = GetOrCreateBoundingArea(b);

        return boundA.Intersects(boundB);
    }

    private static IBoundingArea GetOrCreateBoundingArea(IPhysicsObject obj)
    {
        if (BoundingAreaCache.TryGetValue(obj.Id, out var cached))
        {
            if (obj is AeroBody2D body)
            {
                if (cached.Center != body.Position)
                {
                    cached.Realign(body.Angle, body.Position);
                }
            }
            else if (cached.Center != obj.Position)
            {
                cached.Realign(0, obj.Position);
            }
            return cached;
        }

        var newBound = CreateBoundingArea(obj);
        BoundingAreaCache[obj.Id] = newBound;
        return newBound;
    }

    private static IBoundingArea CreateBoundingArea(IPhysicsObject obj)
    {
        return obj switch
        {
            AeroBody2D body => body.Shape switch
            {
                AeroCircle => BoundingCircle.CreateFromShape(body.Shape, body.Position),
                _ => AABB2D.CreateFromShape(body.Shape, body.Position)
            },
            AeroParticle2D particle => new BoundingCircle(particle.Position, particle.Radius),
            _ => throw new ArgumentException($"Unknown physics object type: {obj.GetType()}")
        };
    }

    private static float GetMaxExtent(IPhysicsObject obj)
    {
        if (obj is AeroParticle2D particle)
            return particle.Radius;

        if (obj is AeroBody2D body)
        {
            if (MaxExtentCache.TryGetValue(body.Shape, out var extent))
                return extent;

            extent = CalculateMaxExtent(body.Shape);
            MaxExtentCache[body.Shape] = extent;
            return extent;
        }

        throw new ArgumentException($"Unknown physics object type: {obj.GetType()}");
    }

    private static float CalculateMaxExtent(AeroShape2D shape)
    {
        return shape switch
        {
            AeroCircle circle => circle.Radius,
            AeroPolygon polygon => CalculatePolygonMaxExtent(polygon),
            _ => throw new ArgumentException("Unknown shape type")
        };
    }

    private static float CalculatePolygonMaxExtent(AeroPolygon polygon)
    {
        var maxExtentSquared = 0f;
        foreach (var vertex in polygon.WorldVertices)
        {
            maxExtentSquared = Math.Max(maxExtentSquared, vertex.MagnitudeSquared);
        }
        return MathF.Sqrt(maxExtentSquared);
    }

    public static void ClearCache()
    {
        BoundingAreaCache.Clear();
        MaxExtentCache.Clear();
    }
}