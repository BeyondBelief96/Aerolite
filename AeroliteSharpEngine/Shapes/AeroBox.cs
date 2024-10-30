using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Shapes.Interfaces;

namespace AeroliteSharpEngine.Shapes;

public class AeroBox : AeroPolygon, IConvexShape
{
    public float Width { get; private set; }
    public float Height { get; private set; }

    public AeroBox(float width, float height)
        : base(CreateBoxVertices(width, height))
    {
        Width = width;
        Height = height;
        UpdateCachedProperties();
    }

    private static IEnumerable<AeroVec2> CreateBoxVertices(float width, float height)
    {
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        return new List<AeroVec2>
        {
            new AeroVec2(-halfWidth, -halfHeight),  // Bottom left
            new AeroVec2(halfWidth, -halfHeight),   // Bottom right
            new AeroVec2(halfWidth, halfHeight),    // Top right
            new AeroVec2(-halfWidth, halfHeight)    // Top left
        };
    }

    protected sealed override void UpdateCachedProperties()
    {
        // Area of a rectangle
        CachedArea = Width * Height;

        // Centroid is at origin for a centered rectangle
        CachedCentroid = new AeroVec2(0, 0);

        // Moment of inertia for a rectangle about its center
        // I = (width² + height²) / 12  (without mass)
        CachedMomentOfInertia = (Width * Width + Height * Height) / 12.0f;

        NeedsUpdate = false;
    }

    public override ShapeType GetShapeType()
    {
        return ShapeType.Box;
    }
}