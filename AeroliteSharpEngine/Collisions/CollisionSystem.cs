using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collision;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes.Interfaces;

namespace AeroliteSharpEngine.Collisions
{
    public enum CollisionSystemType
    {
        ConvexOnly, // Optimized for convex shapes only
        General, // Handles both convex and concave shapes. Less optimized due to having to handle the concave case.
    }

    /// <summary>
    /// Represents the result of a collision test between two objects
    /// </summary>
    public struct CollisionManifold
    {
        public bool HasCollision;
        public AeroVec2 Normal;
        public float Depth;
        public AeroVec2 Point;
        public IPhysicsObject ObjectA;
        public IPhysicsObject ObjectB;
    }

    /// <summary>
    /// Main collision detection system that coordinates broad and narrow phase
    /// </summary>
    public class CollisionSystem
    {
        private readonly CollisionSystemType _collisionSystemType;
        private IBroadPhase _spatialBroadPhase;
        private INarrowPhase _narrowPhase;
        private readonly List<CollisionManifold> _collisions;
        private readonly HashSet<(IPhysicsObject, IPhysicsObject)> _potentialPairs;
        private readonly HashSet<int> _validatedConvexObjectIds;  // Cache for validated objects

        public CollisionSystem(CollisionSystemType type, IBroadPhase spatialBroadPhase, INarrowPhase narrowPhase)
        {
            _collisionSystemType = type;
            _spatialBroadPhase = spatialBroadPhase;
            _narrowPhase = narrowPhase;
            _collisions = new List<CollisionManifold>();
            _potentialPairs = new HashSet<(IPhysicsObject, IPhysicsObject)>();
            _validatedConvexObjectIds = new HashSet<int>();

            if (type == CollisionSystemType.ConvexOnly)
            {
                ValidateConvexShapes = true;
            }
        }

        /// <summary>
        /// Marks the collision system to validate for convex shapes. Defaults to true for <see cref="CollisionSystemType.ConvexOnly"/>
        /// however can be manually toggled off if performance is needed.
        /// </summary>
        public bool ValidateConvexShapes { get; set; }


        /// <summary>
        /// Used to clear convex object validation cache.
        /// </summary>
        public void ClearValidationCache()
        {
            _validatedConvexObjectIds.Clear();
        }

        public void SetBroadPhase(IBroadPhase broadPhaseAlgorithm)
        {
            _spatialBroadPhase = broadPhaseAlgorithm;
        }

        public void SetNarrowPhase(INarrowPhase narrowPhaseAlgorithm)
        {
            _narrowPhase = narrowPhaseAlgorithm;
        }

        public List<CollisionManifold> DetectCollisions(IReadOnlyList<IPhysicsObject>? objects)
        {
            if(objects == null) return [];
            if (ValidateConvexShapes)
            {
                ValidateNewShapes(objects);
            }

            _collisions.Clear();
            _potentialPairs.Clear();

            // First broad phase: Spatial partitioning
            _spatialBroadPhase.Update(objects);
            var spatialPairs = _spatialBroadPhase.FindPotentialCollisions(objects).ToList();

            if(spatialPairs.Count != 0) 
            {
                foreach (var (objA, objB) in spatialPairs)
                {
                    // Only add pairs that pass AABB test
                    if (BoundingAreaTest.TestIntersection(objA, objB))
                    {
                        _potentialPairs.Add((objA, objB));
                    }
                }
            }

            // Narrow phase testing
            foreach (var (objA, objB) in _potentialPairs)
            {
                var manifold = _narrowPhase.TestCollision(objA, objB);
                if (manifold.HasCollision)
                {
                    _collisions.Add(manifold);
                }
            }

            return _collisions;
        }

        private void ValidateNewShapes(IReadOnlyList<IPhysicsObject> objects)
        {
            if (_collisionSystemType != CollisionSystemType.ConvexOnly) return;
            foreach (var obj in objects)
            {
                // Only validate objects we haven't seen before
                if (obj is not AeroBody2D body || _validatedConvexObjectIds.Contains(body.Id)) continue;
                if (body.Shape is not IConvexShape)
                {
                    throw new InvalidOperationException(
                        $"Object {obj} uses non-convex shape {body.Shape.GetType().Name} " +
                        "but the collision system is configured for convex-only shapes.");
                }
                _validatedConvexObjectIds.Add(body.Id);
            }
        }
    }
}