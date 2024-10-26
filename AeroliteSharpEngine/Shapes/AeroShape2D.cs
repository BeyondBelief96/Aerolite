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
        Triangle,
        RegularPolygon = 2,
        Polygon = 3,
    }

    /// <summary>
    /// Abstract class that represents a generic shape.
    /// </summary>
    public abstract class AeroShape2D
    {
        // Cached properties
        protected float cachedArea;
        protected float cachedMomentOfInertia;
        protected AeroVec2 cachedCentroid;
        protected bool needsUpdate;

        protected AeroShape2D()
        {
            needsUpdate = true;
        }

        public float Area
        {
            get
            {
                if (needsUpdate) UpdateCachedProperties();
                return cachedArea;
            }
            protected set => cachedArea = value;
        }

        public AeroVec2 Centroid
        {
            get
            {
                if (needsUpdate) UpdateCachedProperties();
                return cachedCentroid;
            }
            protected set => cachedCentroid = value;
        }


        /// <summary>
        /// Used by children class members to update cached properties of the shape.
        /// </summary>
        protected abstract void UpdateCachedProperties();

        /// <summary>
        /// Returns the type of shape.
        /// </summary>
        public abstract ShapeType GetShapeType();

        /// <summary>
        /// Returns the rotational moment of inertia of the geometrical shape with
        /// respect to its center of mass.
        /// For purposes of the engine, we assume the shapes are solid with mass
        /// distributed evenly throughout.
        /// </summary>
        /// <returns></returns>
        public abstract float GetMomentOfInertia(float mass);

        /// <summary>
        /// Used to update the shape by some rotation angle or change its center of mass position.
        /// This will then update the corresponding vertices that make up the shape.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="position"></param>
        public abstract void UpdateVertices(float angle, AeroVec2 position);
    }
}
