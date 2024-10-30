using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

internal class ForceRegistry
{
    protected struct ForceRegistration
    {
        public IPhysicsObject2D Object2D;
        public IForceGenerator Generator;

        public ForceRegistration(IPhysicsObject2D obj, IForceGenerator gen)
        {
            Object2D = obj;
            Generator = gen;
        }
    }

    protected List<ForceRegistration> registrations;

    public ForceRegistry()
    {
        registrations = new List<ForceRegistration>();
    }

    public void Add(IPhysicsObject2D obj, IForceGenerator generator)
    {
        registrations.Add(new ForceRegistration(obj, generator));
    }

    public void Remove(IPhysicsObject2D obj, IForceGenerator generator)
    {
        registrations.RemoveAll(registration =>
            registration.Object2D == obj && registration.Generator == generator);
    }

    public void Clear()
    {
        registrations.Clear();
    }

    public void UpdateForces(float duration)
    {
        foreach (var registration in registrations)
        {
            registration.Generator.UpdateForce(registration.Object2D, duration);
        }
    }
}