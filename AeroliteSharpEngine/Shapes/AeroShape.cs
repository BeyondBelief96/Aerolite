using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Shapes
{
    /// <summary>
    /// Defines the possible shape types available to bodies in the physics engine.
    /// </summary>
    public enum ShapeType
    {
        Circle = 0,
        Box = 1,
        Polygon = 2,
    }

    /// <summary>
    /// Abstract class that represents a generic shape.
    /// </summary>
    public abstract class AeroShape
    {
        public abstract ShapeType GetShapeType();
        public abstract float GetMomentOfInertia();
        public abstract void UpdateVertices(float angle, AeroVec2 position);
    }

    public class AeroCircle : AeroShape
    {
        public float Radius { get; set; }

        public override float GetMomentOfInertia()
        {
            return 0.5f * (Radius * Radius);
        }

        public override ShapeType GetShapeType()
        {
            return ShapeType.Circle;
        }

        public override void UpdateVertices(float angle, AeroVec2 position)
        {
            return; // Circles don't have any vertices
        }
    }
}
