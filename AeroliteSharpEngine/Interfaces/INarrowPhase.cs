// First, let's define interfaces for broad and narrow phase collision detection

using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collision
{
    /// <summary>
    /// Interface for narrow phase collision detection algorithms
    /// </summary>
    public interface INarrowPhase
    {
        /// <summary>
        /// Tests for collision between two physics objects and returns detailed collision information
        /// </summary>
        CollisionManifold TestCollision(IPhysicsObject objectA, IPhysicsObject objectB);
    }
}