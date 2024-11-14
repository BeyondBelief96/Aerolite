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
    /// Main collision detection system that coordinates broad and narrow phase collision routines.
    /// </summary>
    public class CollisionSystem : ICollisionSystem
{
    private readonly List<CollisionManifold> _collisions = [];
    private readonly HashSet<(IPhysicsObject2D, IPhysicsObject2D)> _potentialPairs = [];
    private readonly HashSet<int> _validatedConvexObjectIds = [];

    public CollisionSystem(CollisionSystemConfiguration collisionSystemConfiguration)
    {
        Configuration = collisionSystemConfiguration;
    }

    public CollisionSystemConfiguration Configuration { get; }
    public IEnumerable<CollisionManifold> Collisions => _collisions;
    public IEnumerable<(IPhysicsObject2D, IPhysicsObject2D)> PotentialPairs => _potentialPairs;

    public void Clear()
    {
        // Return all manifolds to the pool
        foreach (var manifold in _collisions)
        {
            CollisionPoolService.Instance.ReturnManifold(manifold);
        }
        
        _validatedConvexObjectIds.Clear();
        _collisions.Clear();
        _potentialPairs.Clear();
    }

    public void HandleCollisions(IReadOnlyList<IPhysicsObject2D>? objects)
    {
        if (objects == null || objects.Count == 0) return;
        if (Configuration.ValidateConvexShapes)
        {
            ValidateNewShapes(objects);
        }

        // Return existing manifolds to the pool
        foreach (var manifold in _collisions)
        {
            CollisionPoolService.Instance.ReturnManifold(manifold);
        }
        
        _collisions.Clear();
        _potentialPairs.Clear();
        
        // First broad phase: Spatial partitioning
        Configuration.BroadPhase.Update(objects);
        var potentialPairs = Configuration.BroadPhase.FindPotentialCollisions().ToList();

        if (potentialPairs.Count == 0) return;
        
        foreach (var (objA, objB) in potentialPairs)
        {
            _potentialPairs.Add((objA, objB));
            
            var manifold = Configuration.NarrowPhase.TestCollision(objA, objB);
            if (manifold.HasCollision)
            {
                _collisions.Add(manifold);
            }
            else
            {
                CollisionPoolService.Instance.ReturnManifold(manifold);
            }
        }
        
        // Resolve collisions
        if(_collisions.Count > 0)
        {
            Configuration.CollisionResolver.ResolveCollisions(_collisions);
        }
    }

    private void ValidateNewShapes(IReadOnlyList<IPhysicsObject2D> objects)
    {
        if (Configuration.Type != CollisionSystemType.ConvexOnly) return;
        foreach (var obj in objects)
        {
            // Only validate objects we haven't seen before
            if (obj is not AeroBody2D body || _validatedConvexObjectIds.Contains(body.Id)) continue;
            if (body.Shape is not IConvexShape)
            {
                if (body.Shape is AeroPolygon polygon && !polygon.IsConvex) 
                {
                    throw new InvalidOperationException(
                        $"Object {obj} uses non-convex shape " +
                        "but the collision system is configured for convex-only shapes.");
                }
            }
            _validatedConvexObjectIds.Add(body.Id);
        }
    }
}
}