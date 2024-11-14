using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Core.MemoryPool;

namespace AeroliteSharpEngine.Collisions;

public class CollisionPoolService
{
    private static CollisionPoolService? _instance;
    private static readonly object _lock = new();

    private readonly CollisionManifoldPool _manifoldPool;
    private readonly ContactPointPool _contactPool;

    public static CollisionPoolService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new CollisionPoolService();
                }
            }
            return _instance;
        }
    }

    // Private constructor for singleton
    private CollisionPoolService(int initialCapacity = 100)
    {
        _manifoldPool = new CollisionManifoldPool(initialCapacity);
        _contactPool = new ContactPointPool(initialCapacity);
    }

    public CollisionManifold GetManifold() => _manifoldPool.Get();
    public void ReturnManifold(CollisionManifold manifold) => _manifoldPool.Return(manifold);
    public ContactPoint GetContactPoint() => _contactPool.Get();
    public void ReturnContactPoint(ContactPoint contact) => _contactPool.Return(contact);

    public void Clear()
    {
        _manifoldPool.Clear();
        _contactPool.Clear();
    }
}