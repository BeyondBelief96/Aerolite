using AeroliteMonoGraphics.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class Renderer : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly BasicEffect _basicEffect;
    private VertexPositionColor[] _vertices;
    private int[] _indices;
    private int _shapeCount;
    private int _vertexCount;
    private int _indexCount;
    private bool _isDrawing;

    public Renderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _basicEffect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            View = Matrix.Identity,
            World = Matrix.Identity,
            Projection = Matrix.CreateOrthographicOffCenter(
                0, graphicsDevice.Viewport.Width,
                0, graphicsDevice.Viewport.Height, // Flip Y coordinates
                0, 1)
        };

        _vertices = new VertexPositionColor[1024];
        _indices = new int[_vertices.Length * 3];
    }

    public void Begin(Camera camera = null)
    {
        if (_isDrawing)
            throw new InvalidOperationException("End must be called before Begin can be called again");

        _basicEffect.World = camera?.GetTransformation() ?? Matrix.Identity;
        _isDrawing = true;
    }

    public void End()
    {
        if (!_isDrawing)
            throw new InvalidOperationException("Begin must be called before End");

        Flush();
        _isDrawing = false;
    }

    private void Flush()
    {
        if (_shapeCount == 0) return;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                _vertices,
                0,
                _vertexCount,
                _indices,
                0,
                _indexCount / 3);
        }

        _shapeCount = 0;
        _vertexCount = 0;
        _indexCount = 0;
    }

    public void DrawRectangle(Vector2 position, float width, float height, Color color, float rotation = 0f)
    {
        EnsureSpace(4, 6);

        float left = position.X - width / 2;
        float right = left + width;
        float top = position.Y - height / 2;
        float bottom = top + height;

        var cos = (float)Math.Cos(rotation);
        var sin = (float)Math.Sin(rotation);

        Vector2 center = position;
        Vector2 a = Transform(new Vector2(left - center.X, top - center.Y), cos, sin) + center;
        Vector2 b = Transform(new Vector2(right - center.X, top - center.Y), cos, sin) + center;
        Vector2 c = Transform(new Vector2(right - center.X, bottom - center.Y), cos, sin) + center;
        Vector2 d = Transform(new Vector2(left - center.X, bottom - center.Y), cos, sin) + center;

        AddVertex(new Vector3(a, 0), color);
        AddVertex(new Vector3(b, 0), color);
        AddVertex(new Vector3(c, 0), color);
        AddVertex(new Vector3(d, 0), color);

        _indices[_indexCount++] = _vertexCount - 4;
        _indices[_indexCount++] = _vertexCount - 3;
        _indices[_indexCount++] = _vertexCount - 2;
        _indices[_indexCount++] = _vertexCount - 4;
        _indices[_indexCount++] = _vertexCount - 2;
        _indices[_indexCount++] = _vertexCount - 1;

        _shapeCount++;
    }

    public void DrawCircle(Vector2 center, float radius, int segments, Color color)
    {
        const int minSegments = 3;
        const int maxSegments = 256;
        segments = Math.Clamp(segments, minSegments, maxSegments);

        EnsureSpace(segments + 1, (segments) * 3); // Add 1 for center vertex

        // Center vertex
        AddVertex(new Vector3(center, 0), color);

        // Outer vertices
        for (int i = 0; i < segments; i++)
        {
            float angle = i * MathHelper.TwoPi / segments;
            Vector2 point = center + radius * new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle));
            AddVertex(new Vector3(point, 0), color);
        }

        // Indices for triangle fan
        for (int i = 0; i < segments; i++)
        {
            AddIndex(0, i + 1, ((i + 1) % segments) + 1);
        }

        _shapeCount++;
    }

    public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        EnsureSpace(3, 3);

        AddVertex(new Vector3(a, 0), color);
        AddVertex(new Vector3(b, 0), color);
        AddVertex(new Vector3(c, 0), color);

        AddIndex(0, 1, 2);

        _shapeCount++;
    }


    public void DrawTriangle(float ax, float ay, float bx, float by, float cx, float cy, Color color)
    {
        DrawTriangle(new Vector2(ax, ay), new Vector2(bx, by), new Vector2(cx, cy), color);
    }

    private Vector3 Transform(float x, float y, float cos, float sin, Vector2 position)
    {
        return new Vector3(
            x * cos - y * sin + position.X,
            x * sin + y * cos + position.Y,
            0);
    }

    private void EnsureSpace(int vertexCount, int indexCount)
    {
        if (!_isDrawing)
            throw new InvalidOperationException("Begin must be called before drawing");

        if (_vertexCount + vertexCount > _vertices.Length ||
            _indexCount + indexCount > _indices.Length)
        {
            Flush();
        }
    }

    private void AddVertex(Vector3 position, Color color)
    {
        _vertices[_vertexCount++] = new VertexPositionColor(position, color);
    }

    private void AddIndex(int a, int b, int c)
    {
        _indices[_indexCount++] = _vertexCount + a - 3;
        _indices[_indexCount++] = _vertexCount + b - 3;
        _indices[_indexCount++] = _vertexCount + c - 3;
    }

    private Vector2 Transform(Vector2 point, float cos, float sin)
    {
        return new Vector2(
            point.X * cos - point.Y * sin,
            point.X * sin + point.Y * cos);
    }

    public void Dispose()
    {
        _basicEffect?.Dispose();
    }
}