using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Shapes;

/// <summary>
/// Base polygon class that can represent any polygon, convex or concave.
/// </summary>
public class AeroPolygon : AeroShape2D
{
    #region Fields
    private readonly struct CacheKey(float angle, AeroVec2 position) : IEquatable<CacheKey>
    {
        private readonly float angle = angle;
        private readonly AeroVec2 position = position;

        public bool Equals(CacheKey other) =>
            AeroMathExtensions.IsNearlyZero(angle - other.angle) && position.Equals(other.position);

        public override bool Equals(object? obj) => 
            obj is CacheKey key && Equals(key);

        public override int GetHashCode() => 
            HashCode.Combine(angle, position);
    }

    private bool? _isConvex;
    private readonly List<AeroVec2> _localVertices;
    private readonly List<AeroVec2> _worldVertices;
    private CacheKey _lastTransform;

    // Pre-allocated arrays for better performance
    private readonly float[] _crossProducts;
    private readonly AeroVec2[] _triangulationBuffer;
    
    #endregion

    #region Properties 
    protected IReadOnlyList<AeroVec2> LocalVertices => _localVertices;
    public IReadOnlyList<AeroVec2> WorldVertices => _worldVertices;
    
    /// <summary>
    /// Gets whether the polygon is convex (cached after first calculation)
    /// </summary>
    public bool IsConvex => _isConvex ??= CalculateIsConvex();
    
    #endregion
    
    #region Constructors
    protected AeroPolygon(IEnumerable<AeroVec2> vertices)
    {
        ArgumentNullException.ThrowIfNull(vertices);
        
        var verticesArray = vertices as AeroVec2[] ?? vertices.ToArray();
        ValidateVertices(verticesArray);
        
        _localVertices = [..verticesArray];
        _worldVertices = new List<AeroVec2>(verticesArray.Length);
        _worldVertices.AddRange(verticesArray);
        
        // Pre-allocate buffers
        _crossProducts = new float[verticesArray.Length];
        _triangulationBuffer = new AeroVec2[3];
        
        EnsureClockwiseWinding();
        InitializeBaseProperties();
    }
    #endregion
    
    #region Public Methods
    
    public override ShapeType GetShapeType() => ShapeType.Polygon;

    public override float GetMomentOfInertia(float mass)
    {
        if (NeedsUpdate)
            UpdateCachedProperties();
        return mass * CachedMomentOfInertia;
    }
    
    public override void UpdateVertices(float angle, AeroVec2 position)
    {
        var newTransform = new CacheKey(angle, position);
        
        if (newTransform.Equals(_lastTransform))
            return;

        int vertexCount = _localVertices.Count;
        _worldVertices.Clear();
        
        for (int i = 0; i < vertexCount; i++)
        {
            _worldVertices.Add(_localVertices[i].Rotate(angle) + position);
        }

        _lastTransform = newTransform;
    }

    /// <summary>
    /// Calculates the minimum separation distance between two polygons along this polygon's edge normals.
    /// Returns a positive value if the polygons are separated by a gap, or a negative value if they overlap.
    /// </summary>
    /// <param name="otherPolygon">The polygon to check separation against</param>
    /// <param name="axisOfSeparation">The axis along which the minimum separation occurs</param>
    /// <param name="minVertex">The vertex that produces the minimum separation</param>
    /// <returns>The minimum separation distance between the polygons</returns>
    public float FindMinimumSeparation(AeroPolygon otherPolygon, out AeroVec2 axisOfSeparation, out AeroVec2 minVertex)
    {
        float separation = float.MinValue;
        axisOfSeparation = AeroVec2.Zero;
        minVertex = AeroVec2.Zero;
    
        int vertexCount = _worldVertices.Count;
        var otherVertices = otherPolygon.WorldVertices;
        int otherVertexCount = otherVertices.Count;
    
        for (int i = 0; i < vertexCount; i++)
        {
            var va = _worldVertices[i];
            var edgeNormal = GetEdgeNormal(i);
            float minSeparation = float.MaxValue;
            AeroVec2 minPoint = AeroVec2.Zero;
        
            // Find minimum projection along current edge normal
            for (int j = 0; j < otherVertexCount; j++)
            {
                var vb = otherVertices[j];
                var projection = AeroVec2.Dot(vb - va, edgeNormal);
                if (projection < minSeparation)
                {
                    minSeparation = projection;
                    minPoint = vb;
                }
            }

            // Update overall minimum separation if this edge provides larger separation
            if (minSeparation > separation)
            {
                separation = minSeparation;
                axisOfSeparation = edgeNormal;
                minVertex = minPoint;
            }
        }

        return separation;
    }
    
    /// <summary>
    /// Returns the normal vector of the edge starting from the vertex specified from index to index + 1.
    /// </summary>
    /// <param name="index">The starting vertex for the edge.</param>
    /// <returns>The normal vector of the specified edge.</returns>
    public AeroVec2 GetEdgeNormal(int index)
    {
        var current = _worldVertices[index];
        var next = _worldVertices[(index + 1) % _worldVertices.Count];
        var edge = next - current;
        return edge.Normal();
    }
    
    #endregion
    
    #region Protected Methods
    
    protected override void UpdateCachedProperties()
    {
        // Allow derived classes to override specific update methods
        UpdateArea();
        UpdateCentroid();
        UpdateMomentOfInertia();
        NeedsUpdate = false;
    }

    /// <summary>
    /// Updates the area of the polygon. Can be overridden by derived classes.
    /// </summary>
    protected virtual void UpdateArea()
    {
        CachedArea = CalculateArea();
    }
    
    /// <summary>
    /// Updates the moment of inertia of the polygon. Can be overridden by derived classes.
    /// </summary>
    protected virtual void UpdateMomentOfInertia()
    {
        CachedMomentOfInertia = CalculateMomentOfInertia();
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Initializes base properties without virtual method calls
    /// </summary>
    private void InitializeBaseProperties()
    {
        // Only initialize if not overridden
        if (GetType() == typeof(AeroPolygon))
        {
            CachedArea = CalculateArea();
            CachedCentroid = CalculateCentroid();
            CachedMomentOfInertia = CalculateMomentOfInertia();
        }
        NeedsUpdate = false;
    }

    /// <summary>
    /// Updates the centroid of the polygon. Can be overridden by derived classes.
    /// </summary>
    private void UpdateCentroid()
    {
        CachedCentroid = CalculateCentroid();
    }
    
    private float CalculateArea()
    {
        float area = 0;
        int vertexCount = _localVertices.Count;
        
        for (int i = 0, j = 1; i < vertexCount; i++, j = (j + 1) % vertexCount)
        {
            area += _localVertices[i].Cross(_localVertices[j]);
        }
        
        return Math.Abs(area) * 0.5f;
    }

    private AeroVec2 CalculateCentroid()
    {
        var centroid = new AeroVec2();
        float signedArea = 0;
        int vertexCount = _localVertices.Count;

        for (int i = 0, j = 1; i < vertexCount; i++, j = (j + 1) % vertexCount)
        {
            float cross = _localVertices[i].Cross(_localVertices[j]);
            signedArea += cross;
            centroid += (_localVertices[i] + _localVertices[j]) * cross;
        }

        signedArea *= 0.5f;
        return AeroMathExtensions.IsNearlyZero(signedArea) ? 
            centroid : centroid / (6 * signedArea);
    }

    private float CalculateMomentOfInertia()
    {
        float totalMoment = 0;
        int vertexCount = _localVertices.Count;

        _triangulationBuffer[0] = _localVertices[0];
        
        for (int i = 1; i < vertexCount - 1; i++)
        {
            _triangulationBuffer[1] = _localVertices[i];
            _triangulationBuffer[2] = _localVertices[i + 1];
            
            totalMoment += CalculateTriangleMomentOfInertia(_triangulationBuffer);
        }

        return totalMoment;
    }
    
    private bool CalculateIsConvex()
    {
        int vertexCount = _localVertices.Count;
        bool? sign = null;

        // Pre-calculate cross products
        for (int i = 0; i < vertexCount; i++)
        {
            var current = _localVertices[i];
            var next = _localVertices[(i + 1) % vertexCount];
            var nextNext = _localVertices[(i + 2) % vertexCount];

            var edge1 = next - current;
            var edge2 = nextNext - next;
            _crossProducts[i] = edge1.Cross(edge2);
        }

        // Check cross products for sign changes
        for (int i = 0; i < vertexCount; i++)
        {
            float cross = _crossProducts[i];
            
            if (AeroMathExtensions.IsNearlyZero(cross))
                continue;

            if (!sign.HasValue)
            {
                sign = cross > 0;
            }
            else if ((cross > 0) != sign.Value)
            {
                return false;
            }
        }

        return true;
    }

    private void ValidateVertices(AeroVec2[] vertices)
    {
        if (vertices.Length < 3)
        {
            throw new ArgumentException("Polygon must have at least 3 vertices");
        }

        // Check for duplicate consecutive vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            var next = vertices[(i + 1) % vertices.Length];
            if (AeroMathExtensions.IsNearlyZero((vertices[i] - next).MagnitudeSquared))
            {
                throw new ArgumentException($"Consecutive duplicate vertex detected at index {i}");
            }
        }
    }

    private void EnsureClockwiseWinding()
    {
        float signedArea = 0;
        int vertexCount = _localVertices.Count;
        
        for (int i = 0; i < vertexCount; i++)
        {
            var current = _localVertices[i];
            var next = _localVertices[(i + 1) % vertexCount];
            signedArea += current.Cross(next);
        }

        if (signedArea > 0)
        {
            _localVertices.Reverse();
            _worldVertices.Clear();
            _worldVertices.AddRange(_localVertices);
        }
    }

    private float CalculateTriangleMomentOfInertia(AeroVec2[] triangle)
    {
        float a = (triangle[1] - triangle[0]).Magnitude;
        float b = (triangle[2] - triangle[1]).Magnitude;
        float c = (triangle[0] - triangle[2]).Magnitude;
        
        float area = Math.Abs((triangle[1] - triangle[0])
            .Cross(triangle[2] - triangle[0])) * 0.5f;

        return (a * a + b * b + c * c) * area / 36.0f;
    }
    
    #endregion
}