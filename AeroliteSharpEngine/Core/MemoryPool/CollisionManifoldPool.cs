using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;

namespace AeroliteSharpEngine.Core.MemoryPool;

public class CollisionManifoldPool
{
    private readonly ObjectPool<CollisionManifold> _pool;
    private readonly ContactPointPool _contactPool;

    public CollisionManifoldPool(int initialCapacity = 100)
    {
        _pool = new ObjectPool<CollisionManifold>(initialCapacity, resetAction: manifold => manifold.Reset());
        _contactPool = new ContactPointPool(initialCapacity);
    }

    public CollisionManifold Get()
    {
        var manifold = _pool.Get();
        manifold.Contact = _contactPool.Get();
        return manifold;
    }

    public void Return(CollisionManifold manifold)
    {
        _contactPool.Return(manifold.Contact);
        _pool.Return(manifold);
    }

    public void Clear()
    {
        _pool.Clear();
        _contactPool.Clear();
    }
}