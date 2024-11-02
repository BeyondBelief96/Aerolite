using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives 
{
    /// <summary>
    /// Represents the complete collision information between two objects
    /// </summary>
    public struct CollisionManifold 
    {
        /// <summary>
        /// Whether a collision was detected
        /// </summary>
        public bool HasCollision;

        /// <summary>
        /// The collision normal pointing from object B to object A
        /// </summary>
        public AeroVec2 Normal;

        /// <summary>
        /// The first object involved in the collision
        /// </summary>
        public IPhysicsObject2D ObjectA;

        /// <summary>
        /// The second object involved in the collision
        /// </summary>
        public IPhysicsObject2D ObjectB;

        /// <summary>
        /// The contact points generated for this collision.
        /// For circle-circle collisions this will typically be 1 point.
        /// For polygon collisions this can be 1-2 points.
        /// </summary>
        public List<ContactPoint> ContactPoints;

        /// <summary>
        /// The maximum penetration depth across all contact points
        /// </summary>
        public float MaxDepth => ContactPoints.Count > 0 ? 
            ContactPoints.Max(cp => cp.Depth) : 0;
    }
}