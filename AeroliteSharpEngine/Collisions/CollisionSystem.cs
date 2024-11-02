using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using AeroliteSharpEngine.Shapes.Interfaces;

namespace AeroliteSharpEngine.Collisions
{
    /// <summary>
    /// Main collision detection system that coordinates broad and narrow phase
    /// </summary>
    public class CollisionSystem(CollisionSystemConfiguration collisionSystemConfiguration) : ICollisionSystem
    {
        #region Fields
        
        private readonly List<CollisionManifold> _collisions = [];
        private readonly HashSet<(IPhysicsObject2D, IPhysicsObject2D)> _potentialPairs = [];
        private readonly HashSet<int> _validatedConvexObjectIds = [];  // Cache for validated objects
        
        #endregion
        
        #region Properties

        public CollisionSystemConfiguration Configuration { get; } = collisionSystemConfiguration;

        public IEnumerable<CollisionManifold> Collisions => _collisions;

        public IEnumerable<(IPhysicsObject2D, IPhysicsObject2D)> PotentialPairs => _potentialPairs;
        
        #endregion
        
        #region Public Methods
        
        public void Clear()
        {
            _validatedConvexObjectIds.Clear();
            _collisions.Clear();
        }

        public List<CollisionManifold> DetectCollisions(IReadOnlyList<IPhysicsObject2D>? objects)
        {
            if(objects == null) return [];
            if (Configuration.ValidateConvexShapes)
            {
                ValidateNewShapes(objects);
            }

            _collisions.Clear();
            _potentialPairs.Clear();

            // First broad phase: Spatial partitioning
            Configuration.BroadPhase.Update(objects);
            var spatialPairs = Configuration.BroadPhase.FindPotentialCollisions().ToList();
            
            if (spatialPairs.Count == 0) return _collisions;
            foreach (var (objA, objB) in spatialPairs)
            {
                _potentialPairs.Add((objA, objB));
            }

            // Narrow phase testing
            foreach (var (objA, objB) in _potentialPairs)
            {
                var manifold = Configuration.NarrowPhase.TestCollision(objA, objB);
                if (manifold.HasCollision)
                {
                    _collisions.Add(manifold);
                }
            }

            return _collisions;
        }
        
        #endregion
        
        #region Private Methods

        private void ValidateNewShapes(IReadOnlyList<IPhysicsObject2D> objects)
        {
            if (Configuration.Type != CollisionSystemType.ConvexOnly) return;
            foreach (var obj in objects)
            {
                // Only validate objects we haven't seen before
                if (obj is not AeroBody2D body || _validatedConvexObjectIds.Contains(body.Id)) continue;
                if (body.Shape is not IConvexShape)
                {
                    if (body.Shape is AeroPolygon polygon && !polygon.IsConvex()) continue;
                    {
                        throw new InvalidOperationException(
                            $"Object {obj} uses non-convex shape" +
                            "but the collision system is configured for convex-only shapes.");
                    }
                    
                }
                _validatedConvexObjectIds.Add(body.Id);
            }
        }
        
        #endregion
    }
}