namespace AeroliteSharpEngine.Core.Pooling;

public class ObjectPool<T> where T : class, new()
{
    private readonly Stack<T> _pool;
    private readonly Action<T> _resetAction;
    private readonly int _maxSize;

    public ObjectPool(int initialCapacity = 100, int maxSize = 1000, Action<T> resetAction = null)
    {
        _pool = new Stack<T>(initialCapacity);
        _maxSize = maxSize;
        _resetAction = resetAction;

        // Pre-populate pool
        for (int i = 0; i < initialCapacity; i++)
        {
            _pool.Push(new T());
        }
    }

    public T Get()
    {
        return _pool.Count > 0 ? _pool.Pop() : new T();
    }

    public void Return(T? item)
    {
        if (item == null || _pool.Count >= _maxSize) return;
        
        _resetAction?.Invoke(item);
        _pool.Push(item);
    }

    public void Clear()
    {
        _pool.Clear();
    }
}