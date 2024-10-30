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
        Triangle = 2,
        RegularPolygon = 3,
        Polygon = 4,
    }

    /// <summary>
    /// Abstract class that represents a generic shape.
    /// </summary>
    public abstract class AeroShape2D
    {
        // Cached properties
        protected float CachedArea;
        protected float CachedMomentOfInertia;
        protected AeroVec2 CachedCentroid;
        protected bool NeedsUpdate = true;
        
        public float Area
        {
            get
            {
                if (NeedsUpdate) UpdateCachedProperties();
                return CachedArea;
            }
            protected set => CachedArea = value;
        }

        public AeroVec2 Centroid
        {
            get
            {
                if (NeedsUpdate) UpdateCachedProperties();
                return CachedCentroid;
            }
            protected set => CachedCentroid = value;
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
