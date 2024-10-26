// First, let's define interfaces for broad and narrow phase collision detection

using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Collision
{
    /// <summary>
    /// Interface for broad phase collision detection algorithms
    /// </summary>
    public interface IBroadPhase
    {
        /// <summary>
        /// Performs broad phase collision detection and returns potential collision pairs
        /// </summary>
        IEnumerable<(IPhysicsObject, IPhysicsObject)> FindPotentialCollisions(IReadOnlyList<IPhysicsObject> objects);

        /// <summary>
        /// Updates the internal data structures of the broad phase algorithm
        /// </summary>
        void Update(IReadOnlyList<IPhysicsObject> objects);
    }
}