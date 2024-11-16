using System.Runtime.InteropServices;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators;

internal class ForceRegistry
{
    private readonly Dictionary<int, (IPhysicsObject2D Object, HashSet<IForceGenerator> Generators)> _registrations = new();

    public void Add(IPhysicsObject2D obj, IForceGenerator generator)
    {
        ArgumentNullException.ThrowIfNull(generator);
        ArgumentNullException.ThrowIfNull(obj);

        ref var registration = ref CollectionsMarshal.GetValueRefOrAddDefault(
            _registrations, obj.Id, out bool exists);
        
        if (!exists)
        {
            registration = (obj, []);
        }
        
        registration.Generators.Add(generator);
    }

    public void RemoveRegistration(IPhysicsObject2D obj, IForceGenerator generator)
    {
        if (_registrations.TryGetValue(obj.Id, out var registration))
        {
            registration.Generators.Remove(generator);
            
            // Remove the object entry if no more generators
            if (registration.Generators.Count == 0)
            {
                _registrations.Remove(obj.Id);
            }
        }
    }

    public void RemoveObject(IPhysicsObject2D obj)
    {
        _registrations.Remove(obj.Id);
    }

    public void Clear()
    {
        _registrations.Clear();
    }

    public void UpdateForces(float duration)
    {
        foreach (var (_, (obj, generators)) in _registrations)
        {
            foreach (var generator in generators)
            {
                generator.UpdateForce(obj, duration);
            }
        }
    }
    
    public IReadOnlySet<IForceGenerator>? GetGeneratorsForObject(IPhysicsObject2D obj)
    {
        return _registrations.TryGetValue(obj.Id, out var registration) ? registration.Generators : null;
    }
}