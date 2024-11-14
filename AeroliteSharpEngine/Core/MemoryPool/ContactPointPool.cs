using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;

namespace AeroliteSharpEngine.Core.MemoryPool;

public class ContactPointPool
{
    private readonly ObjectPool<ContactPoint> _pool;

    public ContactPointPool(int initialCapacity = 100)
    {
        _pool = new ObjectPool<ContactPoint>(
            initialCapacity,
            resetAction: contact => contact.Reset()
        );
    }

    public ContactPoint Get() => _pool.Get();
    public void Return(ContactPoint contact) => _pool.Return(contact);
    public void Clear() => _pool.Clear();
}