using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Collisions.Detection.PrimitiveTests;

/// <summary>
/// Helper class containing basic collision primitive tests and closest point computations for 2D shapes.
/// </summary>
public static class PrimitiveUtilities
{
    /// <summary>
    /// Computes the closest point on a plane to the given point
    /// </summary>
    /// <param name="point">The point to find the closest point on the plane to.</param>
    /// <param name="plane">The plane to find the closest point on.</param>
    /// <returns></returns>
    public static AeroVec2 ClosestPointOnPlane(AeroVec2 point, AeroPlane plane)
    {
        // t = ((P - Q) · N) / (N · N), where P is planePoint, Q is point, N is planeNormal
        float t = AeroVec2.Dot(plane.Point - point, plane.Normal) / plane.Normal.MagnitudeSquared;
        return point + (plane.Normal * t);
    }
    
    /// <summary>
    /// Computes the signed distance from a point to a plane.
    /// </summary>
    /// <param name="point">The point to test the distance to the plane from.</param>
    /// <param name="plane">The plane from which to test the distance from the point.</param>
    /// <returns></returns>
    public static float SignedDistanceToPlane(AeroVec2 point, AeroPlane plane)
    {
        return AeroVec2.Dot(point - plane.Point, plane.Normal) / plane.Normal.Magnitude;
    }
    
    /// <summary>
    /// Compute the closest point on a line segment to a given point
    /// </summary>
    /// <returns>A tuple containing the closest point and the parametric value t where 0 ≤ t ≤ 1</returns>
    public static (AeroVec2 closestPoint, float t) ClosestPointOnLineSegment(AeroVec2 point, AeroVec2 segmentStart, AeroVec2 segmentEnd)
    {
        AeroVec2 ab = segmentEnd - segmentStart;
            
        // Project point onto line segment, computing parametric position d(t) = a + t*(b-a)
        float t = AeroVec2.Dot(point - segmentStart, ab) / ab.MagnitudeSquared;
            
        // Clamp t to segment bounds
        if (t <= 0.0f)
        {
            return (segmentStart, 0.0f);
        }
            
        if (t >= 1.0f)
        {
            return (segmentEnd, 1.0f);
        }

        // Point projects inside segment
        return (segmentStart + (ab * t), t);
    }
    
    /// <summary>
    /// Compute the squared distance between a point and a line segment
    /// </summary>
    public static float SquaredDistanceToSegment(AeroVec2 point, AeroVec2 segmentStart, AeroVec2 segmentEnd)
    {
        AeroVec2 ab = segmentEnd - segmentStart;
        AeroVec2 ac = point - segmentStart;
        AeroVec2 bc = point - segmentEnd;

        float e = AeroVec2.Dot(ac, ab);
            
        // Handle cases where point projects outside segment
        if (e <= 0.0f)
        {
            return ac.MagnitudeSquared;
        }

        float f = ab.MagnitudeSquared;
        if (e >= f)
        {
            return bc.MagnitudeSquared;
        }

        // Handle case where point projects onto segment
        return ac.MagnitudeSquared - (e * e / f);
    }
    
        /// <summary>
        /// Find the intersection point between two line segments if it exists
        /// </summary>
        /// <returns>True if segments intersect, false otherwise. If true, intersectionPoint contains the point of intersection</returns>
        public static bool SegmentSegmentIntersection(
            AeroVec2 a1, AeroVec2 a2, // First segment points
            AeroVec2 b1, AeroVec2 b2, // Second segment points 
            out AeroVec2 intersectionPoint)
        {
            intersectionPoint = AeroVec2.Zero;

            AeroVec2 a = a2 - a1;  // Direction of segment a
            AeroVec2 b = b2 - b1;  // Direction of segment b
            AeroVec2 c = b1 - a1;  // Vector between segment starts

            float denominator = AeroVec2.Cross(a, b);

            // Check if lines are parallel
            if (AeroMathExtensions.IsNearlyZero(denominator))
            {
                return false;
            }

            // Compute parametric values for intersection
            float t = AeroVec2.Cross(c, b) / denominator;
            float u = AeroVec2.Cross(c, a) / denominator;

            // Check if intersection occurs within both segments
            if (t is >= 0.0f and <= 1.0f && u is >= 0.0f and <= 1.0f)
            {
                intersectionPoint = a1 + (a * t);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Find points of intersection between a line segment and a circle
        /// </summary>
        /// <returns>The number of intersection points (0, 1, or 2)</returns>
        public static int SegmentCircleIntersection(
            AeroVec2 segmentStart, AeroVec2 segmentEnd,
            AeroVec2 circleCenter, float circleRadius,
            out AeroVec2 intersection1, out AeroVec2 intersection2)
        {
            intersection1 = intersection2 = AeroVec2.Zero;
            
            AeroVec2 d = segmentEnd - segmentStart;  // Direction vector of segment
            AeroVec2 f = segmentStart - circleCenter;  // Vector from circle center to segment start

            float a = d.MagnitudeSquared;
            float b = 2 * AeroVec2.Dot(f, d);
            float c = f.MagnitudeSquared - (circleRadius * circleRadius);

            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0) return 0;  // No intersection

            // Ray intersects circle, but need to check if intersection points lie on segment
            discriminant = MathF.Sqrt(discriminant);
            float t1 = (-b - discriminant) / (2 * a);
            float t2 = (-b + discriminant) / (2 * a);

            int numIntersections = 0;

            if (t1 >= 0 && t1 <= 1)
            {
                intersection1 = segmentStart + (d * t1);
                numIntersections++;
            }

            if (t2 >= 0 && t2 <= 1)
            {
                // If this is the first intersection point found, store in intersection1
                if (numIntersections == 0)
                    intersection1 = segmentStart + (d * t2);
                else
                    intersection2 = segmentStart + (d * t2);
                numIntersections++;
            }

            return numIntersections;
        }
        
        /// <summary>
        /// Finds the closest point on any edge of the polygon to a given point
        /// <param name="vertices">The vertices of the polygon to find the closest point on.</param>
        /// <param name="point">The point to find the closest point on the polygon to.</param>
        /// </summary>
        public static AeroVec2 FindClosestEdgePoint(
            IReadOnlyList<AeroVec2> vertices,
            AeroVec2 point)
        {
            var minDistanceSquared = float.MaxValue;
            var closestPoint = vertices[0];

            for (int i = 0; i < vertices.Count; i++)
            {
                var start = vertices[i];
                var end = vertices[(i + 1) % vertices.Count];
                var edgeClosest = PrimitiveUtilities.ClosestPointOnLineSegment(point, start, end);
            
                var distanceSquared = (point - edgeClosest.closestPoint).MagnitudeSquared;
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closestPoint = edgeClosest.closestPoint;
                }
            }

            return closestPoint;
        }
        
        /// <summary>
        /// Find the geometric center of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>A point representing the geometric center of the polygon.</returns>
        public static AeroVec2 GetPolygonCenter(IReadOnlyList<AeroVec2> vertices)
        {
            var center = AeroVec2.Zero;
            foreach (var vertex in vertices)
            {
                center += vertex;
            }
            return center / vertices.Count;
        }
}