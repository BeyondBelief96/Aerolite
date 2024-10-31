using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.ForceGenerators;
using AeroliteSharpEngine.Interfaces;
using AeroliteSharpEngine.Performance;

namespace AeroliteSharpEngine.Core;

public class AeroWorld2D(AeroWorldConfiguration configuration) : IAeroPhysicsWorld
{
    private readonly List<IPhysicsObject2D> _physicsObjects = [];
    private readonly List<AeroVec2> _globalForces = [];
    private readonly ForceRegistry _forceRegistry = new();
    private IIntegrator _integrator = configuration.Integrator;
    private readonly IPerformanceMonitor _performanceMonitor = new ConsolePerformanceLogger();
    private AeroWorldConfiguration _configuration = configuration;
    
    public float Gravity { get; set; } = configuration.Gravity;
    public ICollisionSystem CollisionSystem { get; private set; } = new CollisionSystem(configuration.CollisionSystemConfiguration);
    public bool PerformanceMonitoringEnabled { get; set; } = configuration.EnablePerformanceMonitoring;

    // Default constructor
    public AeroWorld2D() : this(AeroWorldConfiguration.Default)
    {
    }

    public AeroWorldConfiguration GetConfiguration() => _configuration;

    public void UpdateConfiguration(AeroWorldConfiguration newConfig)
    {
        _configuration = newConfig;
        Gravity = newConfig.Gravity;
        _integrator = newConfig.Integrator;
        PerformanceMonitoringEnabled = newConfig.EnablePerformanceMonitoring;
        CollisionSystem = new CollisionSystem(newConfig.CollisionSystemConfiguration);
    }

    public IReadOnlyList<IPhysicsObject2D> GetObjects() => _physicsObjects;
    
    public IReadOnlyList<IBody2D> GetBodies() => _physicsObjects.OfType<IBody2D>().ToList();
    
    public IReadOnlyList<AeroParticle2D> GetParticles() => _physicsObjects.OfType<AeroParticle2D>().ToList();

    public IEnumerable<CollisionManifold> GetCollisions() => CollisionSystem.Collisions;

    public void ClearWorld()
    {
        _physicsObjects.Clear();
        _globalForces.Clear();
        _forceRegistry.Clear();
        CollisionSystem.Clear();
    }

    public void AddPhysicsObject(IPhysicsObject2D physicsObject2D)
    {
        _physicsObjects.Add(physicsObject2D);
    }

    // Remove by direct object reference
    public void RemoveObject(IPhysicsObject2D physicsObject)
    {
        _physicsObjects.Remove(physicsObject);
        _forceRegistry.RemoveObject(physicsObject);
    }

    public void AddForceGenerator(IPhysicsObject2D particle, IForceGenerator generator)
    {
        _forceRegistry.Add(particle, generator);
    }

    public void AddGlobalForce(AeroVec2 force)
    {
        _globalForces.Add(force);
    }

    public void Update(float dt)
    {
        if(PerformanceMonitoringEnabled)
            _performanceMonitor.BeginStep();

        var potentialCollisionPairs = CollisionSystem.DetectCollisions(_physicsObjects);

        _forceRegistry.UpdateForces(dt);

        foreach (var physicsObject in _physicsObjects.Where(physicsObject => !physicsObject.IsStatic))
        {
            if (Gravity != 0)
            {
                var weight = new AeroVec2(0.0f, physicsObject.Mass * Gravity);
                physicsObject.ApplyForce(weight);
            }

            foreach (var force in _globalForces)
            {
                physicsObject.ApplyForce(force);
            }

            _integrator.IntegrateLinear(physicsObject, dt);
            _integrator.IntegrateAngular(physicsObject, dt);
        }

        if(PerformanceMonitoringEnabled)
            _performanceMonitor.EndStep(this);
    }
}