using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Shapes;
using AeroliteSharpEngine.Shapes.Interfaces;

public class AeroCircle : AeroShape2D, IConvexShape
{
    public float Radius { get; private set; }

    public AeroCircle(float radius)
    {
        Radius = radius;
        UpdateCachedProperties();
    }

    protected override void UpdateCachedProperties()
    {
        // Area of a circle
        CachedArea = MathF.PI * Radius * Radius;

        // Centroid is at origin
        CachedCentroid = new AeroVec2(0, 0);

        // Moment of inertia for a circle about its center
        // I = r² / 2  (without mass)
        CachedMomentOfInertia = (Radius * Radius) / 2.0f;

        NeedsUpdate = false;
    }

    public override ShapeType GetShapeType()
    {
        return ShapeType.Circle;
    }

    public override float GetMomentOfInertia(float mass)
    {
        if (NeedsUpdate) UpdateCachedProperties();
        return mass * CachedMomentOfInertia;
    }

    public override void UpdateVertices(float angle, AeroVec2 position)
    {
        // Circles don't need vertex updates since they're defined by radius
        // But we might want to update the center position if needed
        CachedCentroid = position;
    }

    public bool ContainsPoint(AeroVec2 point)
    {
        return (point - CachedCentroid).MagnitudeSquared <= Radius * Radius;
    }

    public AeroVec2 GetPointOnCircle(float angle)
    {
        return new AeroVec2(
            CachedCentroid.X + Radius * MathF.Cos(angle),
            CachedCentroid.Y + Radius * MathF.Sin(angle)
        );
    }
}