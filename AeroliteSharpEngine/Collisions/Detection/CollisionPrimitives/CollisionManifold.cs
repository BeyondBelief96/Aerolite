using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives 
{
    /// <summary>
    /// Represents contact information between two colliding bodies at a specific point
    /// </summary>
    public struct ContactPoint
    {
        /// <summary>
        /// The point of contact inside objectA, that lies on objectB.
        /// </summary>
        public AeroVec2 StartPoint;

        /// <summary>
        /// The point of contact inside objectB that lies on objectA.
        /// </summary>
        public AeroVec2 EndPoint;

        /// <summary>
        /// The penetration depth between the start and end contact points.
        /// </summary>
        public float Depth;

        public void Reset()
        {
            StartPoint = AeroVec2.Zero;
            EndPoint = AeroVec2.Zero;
            Depth = 0;
        }
    }

    /// <summary>
    /// Represents the complete collision information between two objects. Only valid if HasCollision is true.
    /// </summary>
    public struct CollisionManifold
    {
        /// <summary>
        /// Whether a collision has been detected between the two objects
        /// </summary>
        public bool HasCollision;

        /// <summary>
        /// The first object in the collision
        /// </summary>
        public IPhysicsObject2D? ObjectA;

        /// <summary>
        /// The second object in the collision
        /// </summary>
        public IPhysicsObject2D? ObjectB;

        /// <summary>
        /// The collision normal vector, pointing from the point inside objectA towards objectB.
        /// </summary>
        public AeroVec2 Normal;

        /// <summary>
        /// The points of contact between the two bodies
        /// </summary>
        public ContactPoint Contact;

        public void Reset()
        {
            HasCollision = false;
            ObjectA = null;
            ObjectB = null;
            Normal = AeroVec2.Zero;
            Contact.Reset();
        }
    }

}