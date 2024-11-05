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
        for (int i = 0; i < verticesA.Count; i++)
        {
            var v1 = verticesA[i];
            var v2 = verticesA[(i + 1) % verticesA.Count];
            var edge = v2 - v1;
            var axis = new AeroVec2(-edge.Y, edge.X).UnitVector();

            var (minA, maxA) = ProjectVertices(verticesA, axis);
            var (minB, maxB) = ProjectVertices(verticesB, axis);

            if (minA >= maxB || minB >= maxA)
            {
                return (false, default, default, default);
            }
        }

        for (int i = 0; i < verticesB.Count; i++)
        {
            var v1 = verticesB[i];
            var v2 = verticesB[(i + 1) % verticesA.Count];
            var edge = v2 - v1;
            var axis = new AeroVec2(-edge.Y, edge.X);

            var (minA, maxA) = ProjectVertices(verticesA, axis);
            var (minB, maxB) = ProjectVertices(verticesB, axis);

            if (minA >= maxB || minB >= maxA)
            {
                return (false, default, default, default);
            }
        }
        
        return (true, default, default, default);
        
        // // Test axes from both polygons
        // var resultA = CheckSeparatingAxes(verticesA, verticesB);
        // if (!resultA.hasCollision)
        //     return (false, AeroVec2.Zero, 0, default);
        //
        // var resultB = CheckSeparatingAxes(verticesB, verticesA);
        // if (!resultB.hasCollision)
        //     return (false, AeroVec2.Zero, 0, default);
        //
        // // Get the axis with minimum penetration
        // var (normal, depth) = GetCollisionNormal(
        //     resultA.penetration, resultA.normal,
        //     resultB.penetration, resultB.normal,
        //     verticesA, verticesB);
        //
        // return (true, normal, depth, new ContactPoint());
    }

    /// <summary>
    /// Checks for separation between two polygons against all axes of the polygon represented by <see cref="axisVertices"/>
    /// </summary>
    /// <param name="axisVertices">The polygon whose axes were using to test.</param>
    /// <param name="otherVertices">The other polygon in test.</param>
    /// <returns></returns>
    private static (bool hasCollision, float penetration, AeroVec2 normal) CheckSeparatingAxes(
        IReadOnlyList<AeroVec2> axisVertices,
        IReadOnlyList<AeroVec2> otherVertices)
    {
        // These two will be the axis and the minimum overlap/distance needed to separate the two bodies.
        var minPenetration = float.MaxValue;
        var minNormal = AeroVec2.Zero;

        // Check each edge's normal as a potential separating axis
        for (int i = 0; i < axisVertices.Count; i++)
        {
            var normal = GetEdgeNormal(axisVertices, i);
            var overlap = GetOverlap(axisVertices, otherVertices, normal);
            
            // If we find an axis with no overlap, we can exit early as we have found a separating axis.
            if (!overlap.hasOverlap)
                return (false, 0, AeroVec2.Zero);
            
            // We want the axis that we can move the minimum amount of distance to separate the
            // two objects, and the axis that corresponds to. 
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
    /// <param name="vertices">The vertices to project.</param>
    /// <param name="axis">The axis to project on.</param>
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
        
        return (normal, depth);
    }
    
    private static AeroVec2 GetEdgeNormal(IReadOnlyList<AeroVec2> vertices, int index)
    {
        var current = vertices[index];
        var next = vertices[(index + 1) % vertices.Count];
        var edge = next - current;
        
        // Ensure consistent winding order for normals
        var normal = new AeroVec2(-edge.Y, edge.X);
        
        // Verify the normal points outward
        var center = PrimitiveUtilities.GetPolygonCenter(vertices);
        var edgeCenter = (current + next) * 0.5f;
        var toCenter = center - edgeCenter;
        
        // If normal points inward, flip it
        if (AeroVec2.Dot(normal, toCenter) < 0)
        {
            normal = -normal;
        }
        
        return normal.UnitVector();
    }
}