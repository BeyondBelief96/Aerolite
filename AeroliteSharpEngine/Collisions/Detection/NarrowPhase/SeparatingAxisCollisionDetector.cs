using System.Runtime.CompilerServices;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.PrimitiveTests;

namespace AeroliteSharpEngine.Collisions.Detection.NarrowPhase;

/// <summary>
/// Implementation of collision detection using the Separating Axis Theorem (SAT)
/// for convex shapes.
/// </summary>
public static class SeparatingAxisCollisionDetector
{
    /// <summary>
    /// Tests for collisions and computes contact information between two polygons.
    /// </summary>
    /// <param name="verticesA"></param>
    /// <param name="verticesB"></param>
    /// <returns></returns>
    public static (bool hasCollision, AeroVec2 normal, float depth, ContactPoint contact) TestPolygonPolygon(
        IReadOnlyList<AeroVec2> verticesA,
        IReadOnlyList<AeroVec2> verticesB)
    {
        // Test axes from both polygons
        var resultA = CheckSeparatingAxes(verticesA, verticesB);
        if (!resultA.hasCollision)
            return (false, AeroVec2.Zero, 0, default);

        var resultB = CheckSeparatingAxes(verticesB, verticesA);
        if (!resultB.hasCollision)
            return (false, AeroVec2.Zero, 0, default);

        // Get the axis with minimum penetration
        var (normal, depth) = GetCollisionNormal(
            resultA.penetration, resultA.normal,
            resultB.penetration, resultB.normal,
            verticesA, verticesB);

        // Find contact point using the collision normal
        var contact = FindContactPoint(verticesA, verticesB, normal, depth);

        return (true, normal, depth, contact);
    }

    /// <summary>
    /// Checks for separation between two polygons against all axes of the polygon represented by <see cref="vertices"/>
    /// </summary>
    /// <param name="vertices">The polygon whose axes were using to test.</param>
    /// <param name="otherVertices">The other polygon in test.</param>
    /// <returns></returns>
    private static (bool hasCollision, float penetration, AeroVec2 normal) CheckSeparatingAxes(
        IReadOnlyList<AeroVec2> vertices,
        IReadOnlyList<AeroVec2> otherVertices)
    {
        var minPenetration = float.MaxValue;
        var minNormal = AeroVec2.Zero;

        // Check each edge's normal as a potential separating axis
        for (int i = 0; i < vertices.Count; i++)
        {
            var normal = GetEdgeNormal(vertices, i);
            var overlap = GetOverlap(vertices, otherVertices, normal);

            if (!overlap.hasOverlap)
                return (false, 0, AeroVec2.Zero);

            if (overlap.amount < minPenetration)
            {
                minPenetration = overlap.amount;
                minNormal = normal;
            }
        }

        return (true, minPenetration, minNormal);
    }

    /// <summary>
    /// Gets the overlap between two polygons along a given axis
    /// </summary>
    private static (bool hasOverlap, float amount) GetOverlap(
        IReadOnlyList<AeroVec2> verticesA,
        IReadOnlyList<AeroVec2> verticesB,
        AeroVec2 axis)
    {
        var projectionA = ProjectVertices(verticesA, axis);
        var projectionB = ProjectVertices(verticesB, axis);

        // Check if projections overlap
        if (projectionA.max < projectionB.min || projectionB.max < projectionA.min)
            return (false, 0);

        // Calculate overlap amount
        var overlap = Math.Min(
            projectionA.max - projectionB.min,
            projectionB.max - projectionA.min);

        return (true, overlap);
    }

    /// <summary>
    /// Projects vertices onto an axis and returns min/max values
    /// </summary>
    private static (float min, float max) ProjectVertices(
        IReadOnlyList<AeroVec2> vertices,
        AeroVec2 axis)
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (var vertex in vertices)
        {
            var projection = AeroVec2.Dot(vertex, axis);
            min = Math.Min(min, projection);
            max = Math.Max(max, projection);
        }

        return (min, max);
    }

    /// <summary>
    /// Determines the final collision normal from the two polygon's penetration results
    /// </summary>
    private static (AeroVec2 normal, float depth) GetCollisionNormal(
        float depthA, AeroVec2 normalA,
        float depthB, AeroVec2 normalB,
        IReadOnlyList<AeroVec2> verticesA,
        IReadOnlyList<AeroVec2> verticesB)
    {
        // Use the axis with the least penetration
        var (normal, depth) = depthA < depthB 
            ? (normalA, depthA) 
            : (normalB, depthB);

        // Ensure normal points from A to B
        var centerA = PrimitiveUtilities.GetPolygonCenter(verticesA);
        var centerB = PrimitiveUtilities.GetPolygonCenter(verticesB);
        
        if (AeroVec2.Dot(centerB - centerA, normal) < 0)
            normal = -normal;

        return (normal, depth);
    }

    /// <summary>
    /// Finds the contact point between two colliding polygons
    /// </summary>
    private static ContactPoint FindContactPoint(
        IReadOnlyList<AeroVec2> verticesA,
        IReadOnlyList<AeroVec2> verticesB,
        AeroVec2 normal,
        float depth)
    {
        // Find the deepest point of A into B
        var deepestPoint = FindDeepestPoint(verticesA, -normal);
        
        // Find the closest edge of B to this point
        var closestEdgePoint = PrimitiveUtilities.FindClosestEdgePoint(verticesB, deepestPoint);

        return new ContactPoint
        {
            PointOnA = deepestPoint,
            PointOnB = closestEdgePoint,
            Depth = depth
        };
    }

    /// <summary>
    /// Finds the deepest point of a polygon in a given direction
    /// </summary>
    private static AeroVec2 FindDeepestPoint(
        IReadOnlyList<AeroVec2> vertices,
        AeroVec2 direction)
    {
        var maxProjection = float.MinValue;
        var deepestPoint = vertices[0];

        foreach (var vertex in vertices)
        {
            var projection = AeroVec2.Dot(vertex, direction);
            if (projection > maxProjection)
            {
                maxProjection = projection;
                deepestPoint = vertex;
            }
        }

        return deepestPoint;
    }
    
    private static AeroVec2 GetEdgeNormal(IReadOnlyList<AeroVec2> vertices, int index)
    {
        var edge = vertices[(index + 1) % vertices.Count] - vertices[index];
        return new AeroVec2(-edge.Y, edge.X).UnitVector();
    }
}