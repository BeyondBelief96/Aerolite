using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.Interfaces 
{
    /// <summary>
    /// Abstraction for a spatial partitioning algorithm to test for early outs during
    /// the collision detection lifecycle.
    /// </summary>
    public interface IBroadPhase
    {
        /// <summary>
        /// Used to update the spatial data structure with the new set of physics objects.
        /// </summary>
        /// <param name="objects"></param>
        void Update(IEnumerable<IPhysicsObject2D> objects);
        
        /// <summary>
        /// Returns an enumerable set of pairs of physics objects that are potentially colliding
        /// inside the spatial data structure.
        /// </summary>
        /// <returns></returns>
        IEnumerable<(IPhysicsObject2D, IPhysicsObject2D)> FindPotentialCollisions();
    }
}