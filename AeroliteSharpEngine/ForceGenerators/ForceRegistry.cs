using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Interfaces;

internal class ForceRegistry
{
    protected struct ForceRegistration
    {
        public IPhysicsObject Object;
        public IForceGenerator Generator;

        public ForceRegistration(IPhysicsObject obj, IForceGenerator gen)
        {
            Object = obj;
            Generator = gen;
        }
    }

    protected List<ForceRegistration> registrations;

    public ForceRegistry()
    {
        registrations = new List<ForceRegistration>();
    }

    public void Add(IPhysicsObject obj, IForceGenerator generator)
    {
        registrations.Add(new ForceRegistration(obj, generator));
    }

    public void Remove(IPhysicsObject obj, IForceGenerator generator)
    {
        registrations.RemoveAll(registration =>
            registration.Object == obj && registration.Generator == generator);
    }

    public void Clear()
    {
        registrations.Clear();
    }

    public void UpdateForces(float duration)
    {
        foreach (var registration in registrations)
        {
            registration.Generator.UpdateForce(registration.Object, duration);
        }
    }
}