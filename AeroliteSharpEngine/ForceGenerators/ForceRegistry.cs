using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.ForceGenerators;

internal class ForceRegistry
{
    protected struct ForceRegistration(IPhysicsObject2D obj, IForceGenerator gen)
    {
        public readonly IPhysicsObject2D Object2D = obj;
        public readonly IForceGenerator Generator = gen;
    }

    protected readonly List<ForceRegistration> Registrations = [];

    public void Add(IPhysicsObject2D obj, IForceGenerator generator)
    {
        Registrations.Add(new ForceRegistration(obj, generator));
    }

    public void RemoveRegistration(IPhysicsObject2D obj, IForceGenerator generator)
    {
        Registrations.RemoveAll(registration =>
            Equals(registration.Object2D, obj) && registration.Generator == generator);
    }

    public void RemoveObject(IPhysicsObject2D obj)
    {
        Registrations.RemoveAll(registration => Equals(registration.Object2D, obj));
    }

    public void Clear()
    {
        Registrations.Clear();
    }

    public void UpdateForces(float duration)
    {
        foreach (var registration in Registrations)
        {
            registration.Generator.UpdateForce(registration.Object2D, duration);
        }
    }
}