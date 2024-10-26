using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Shapes.Interfaces;
using AeroliteSharpEngine.Shapes;

/// <summary>
/// A regular polygon is a polygon that is direct equiangular (all angles are equal in measure) and equilateral (all sides have the same length). Regular polygons may be either convex, star or skew.
/// </summary>
public class AeroRegularPolygon : AeroPolygon, IConvexShape
{
    /// <summary>
    /// Number of sides the polygon has.
    /// </summary>
    public int NumberOfSides { get; private set; }

    /// <summary>
    /// Side length between each vertex of the polygon. Each side has equal length.
    /// </summary>
    public float SideLength { get; private set; }

    /// <summary>
    /// Distance from the center of the polygon to any vertex.
    /// </summary>
    public float Radius { get; private set; }

    public AeroRegularPolygon(int numberOfSides, float radius)
        : base(GenerateRegularPolygonVertices(numberOfSides, radius))
    {
        if (numberOfSides < 3)
        {
            throw new ArgumentException("Regular polygon must have at least 3 sides");
        }

        // Set properties after base constructor
        NumberOfSides = numberOfSides;
        Radius = radius;
        SideLength = 2 * radius * MathF.Sin(MathF.PI / numberOfSides);

        // Force recalculation of cached properties now that we have our properties set
        needsUpdate = true;
        UpdateCachedProperties();
    }

    private static IEnumerable<AeroVec2> GenerateRegularPolygonVertices(int sides, float radius)
    {
        var vertices = new List<AeroVec2>(sides);
        float angleStep = 2 * MathF.PI / sides;
        float startAngle = MathF.PI / 2; // Start at top

        for (int i = 0; i < sides; i++)
        {
            float angle = startAngle + i * angleStep;
            var vertex = new AeroVec2(
                radius * MathF.Cos(angle),
                radius * MathF.Sin(angle)
            );
            vertices.Add(vertex);
        }
        return vertices;
    }

    protected override void UpdateArea()
    {
        // Calculate area using properties not cached values
        cachedArea = 0.5f * NumberOfSides * Radius * Radius *
                    MathF.Sin(2 * MathF.PI / NumberOfSides);
    }

    protected override void UpdateMomentOfInertia()
    {
        // Calculate moment using properties not cached values
        cachedMomentOfInertia = (1.0f / 6.0f) * NumberOfSides * Radius * Radius *
                               (1 + MathF.Cos(MathF.PI / NumberOfSides));
    }

    // No need to override UpdateCentroid as it's already at origin

    public override ShapeType GetShapeType()
    {
        return ShapeType.RegularPolygon;
    }

    public override float GetMomentOfInertia(float mass)
    {
        if (needsUpdate)
        {
            UpdateCachedProperties();
        }
        return mass * cachedMomentOfInertia;
    }
}