﻿using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Shapes;

/// <summary>
/// Base polygon class that can represent any polygon, convex or concave
/// </summary>
public class AeroPolygon : AeroShape2D
{
    private readonly List<AeroVec2> _localVertices;  // Vertices in local space
    private readonly List<AeroVec2> _worldVertices;  // Transformed vertices in world space
    private bool verticesDirty;            // Flag to indicate if vertices need updating
    private float lastAngle;
    private AeroVec2 lastPosition;

    protected IReadOnlyList<AeroVec2> LocalVertices => _localVertices;
    public IReadOnlyList<AeroVec2> WorldVertices => _worldVertices;

    protected AeroPolygon(IEnumerable<AeroVec2> vertices)
    {
        var verticesVec2 = vertices as AeroVec2[] ?? vertices.ToArray();
        _localVertices = [..verticesVec2];
        _worldVertices = [..verticesVec2];

        if (_localVertices.Count < 3)
        {
            throw new ArgumentException("Polygon must have at least 3 vertices");
        }

        verticesDirty = true;
        UpdateCachedProperties();
    }

    protected override void UpdateCachedProperties()
    {
        UpdateArea();
        UpdateCentroid();
        UpdateMomentOfInertia();
        NeedsUpdate = false;
    }

    protected virtual void UpdateArea()
    {
        // Calculate area using the shoelace formula
        float area = 0;
        for (int i = 0; i < _localVertices.Count; i++)
        {
            int j = (i + 1) % _localVertices.Count;
            area += _localVertices[i].Cross(_localVertices[j]);
        }
        CachedArea = Math.Abs(area) / 2;
    }

    protected virtual void UpdateCentroid()
    {
        // Calculate centroid using weighted average of vertices
        AeroVec2 centroid = new AeroVec2();
        float signedArea = 0;

        for (int i = 0; i < _localVertices.Count; i++)
        {
            int j = (i + 1) % _localVertices.Count;
            float cross = _localVertices[i].Cross(_localVertices[j]);
            signedArea += cross;
            centroid += (_localVertices[i] + _localVertices[j]) * cross;
        }

        signedArea /= 2;
        if (Math.Abs(signedArea) > float.Epsilon)
        {
            centroid /= 6 * signedArea;
        }

        CachedCentroid = centroid;
    }

    protected virtual void UpdateMomentOfInertia()
    {
        // Generic moment of inertia calculation that works for any polygon
        float totalMoment = 0;

        // Triangulate from first vertex and sum contributions
        for (int i = 1; i < _localVertices.Count - 1; i++)
        {
            var triangle = new[] {
                _localVertices[0],
                _localVertices[i],
                _localVertices[i + 1]
            };

            totalMoment += CalculateTriangleMomentOfInertia(triangle);
        }

        CachedMomentOfInertia = totalMoment;
    }

    private float CalculateTriangleMomentOfInertia(AeroVec2[] triangleVertices)
    {
        // Calculate moment of inertia for a triangle about its centroid
        float a = (triangleVertices[1] - triangleVertices[0]).Magnitude;
        float b = (triangleVertices[2] - triangleVertices[1]).Magnitude;
        float c = (triangleVertices[0] - triangleVertices[2]).Magnitude;
        float area = Math.Abs((triangleVertices[1] - triangleVertices[0])
            .Cross(triangleVertices[2] - triangleVertices[0])) / 2.0f;

        return (a * a + b * b + c * c) * area / 36.0f;
    }

    public override void UpdateVertices(float angle, AeroVec2 position)
    {
        if (!verticesDirty && Math.Abs(lastAngle - angle) < float.Epsilon && lastPosition == position)
            return;

        for (var i = 0; i < _localVertices.Count; i++)
        {
            _worldVertices[i] = _localVertices[i].Rotate(angle) + position;
        }

        verticesDirty = false;
        lastAngle = angle;
        lastPosition = position;
    }

    public override ShapeType GetShapeType()
    {
        return ShapeType.Polygon;
    }

    public override float GetMomentOfInertia(float mass)
    {
        if (NeedsUpdate)
            UpdateCachedProperties();
        return mass * CachedMomentOfInertia;
    }
}