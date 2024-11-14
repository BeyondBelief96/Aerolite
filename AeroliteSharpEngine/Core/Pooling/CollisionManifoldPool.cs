using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;

namespace AeroliteSharpEngine.Core.Pooling;

public class CollisionManifoldPool
{
    private readonly ObjectPool<CollisionManifold> _pool;

    public CollisionManifoldPool(int initialCapacity = 100)
    {
        _pool = new ObjectPool<CollisionManifold>(
            initialCapacity, 
            resetAction: manifold => manifold.Reset()
        );
    }

    public CollisionManifold Get() => _pool.Get();
    public void Return(CollisionManifold manifold) => _pool.Return(manifold);
    public void Clear() => _pool.Clear();
}
