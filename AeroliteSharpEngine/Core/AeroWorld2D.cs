using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collision;
using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Collisions.BroadPhase;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Integrators;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Core;

public class AeroWorld2D : IAeroPhysicsWorld
{

    private int _bodyCount = 0;
    private int _particleCount = 0;

    private readonly List<IPhysicsObject> _physicsObjects;
    private readonly List<AeroVec2> _globalForces;
    private readonly ForceRegistry _forceRegistry;
    private IIntegrator _integrator;
    private readonly IPerformanceMonitor _performanceMonitor;

    private readonly CollisionSystem _collisionSystem;
    private List<CollisionManifold> _currentFrameCollisions;

    public AeroWorld2D(float gravity = 9.8f, CollisionSystemType collisionSystemType = CollisionSystemType.ConvexOnly)
    {
        Gravity = gravity;
        _physicsObjects = new List<IPhysicsObject>();
        _integrator = new EulerIntegrator();
        _globalForces = new List<AeroVec2>();
        _forceRegistry = new ForceRegistry();
        _performanceMonitor = new ConsolePerformanceLogger();

        var broadPhase = new SpatialHashBroadPhase();
        var narrowPhase = CollisionDetectorFactory.CreateNarrowPhase(collisionSystemType);
        _collisionSystem = new CollisionSystem(collisionSystemType, broadPhase, narrowPhase);
        _currentFrameCollisions = new List<CollisionManifold>();

        PerformanceMonitoringEnabled = true; ;
    }

    public float Gravity { get; set; }

    public IReadOnlyList<IPhysicsObject> GetObjects() => _physicsObjects;
    
    public IReadOnlyList<IBody2D> GetBodies() => _physicsObjects.OfType<IBody2D>().ToList();

    public IReadOnlyList<CollisionManifold> GetCollisions() => _currentFrameCollisions;

    public bool PerformanceMonitoringEnabled { get; set; }

    public void ClearWorld()
    {
        _physicsObjects.Clear();
        _globalForces.Clear();
        _forceRegistry.Clear();
        _currentFrameCollisions.Clear();
        _collisionSystem.ClearValidationCache();
    }

    public void SetIntegrator(IIntegrator integrator)
    {
        _integrator = integrator;
    }

    public void AddPhysicsObject(IPhysicsObject physicsObject)
    {
        _physicsObjects.Add(physicsObject);

        if (!PerformanceMonitoringEnabled) return;
        switch (physicsObject)
        {
            case AeroBody2D:
                _bodyCount++;
                break;
            case AeroParticle2D:
                _particleCount++;
                break;
        }
    }

    public void AddForceGenerator(IPhysicsObject particle, IForceGenerator generator)
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

        _currentFrameCollisions = _collisionSystem.DetectCollisions(_physicsObjects);

        // Updates any forces registered to specific physics objects
        _forceRegistry.UpdateForces(dt);

        // Update all physics objects
        foreach (var physicsObject in _physicsObjects.Where(physicsObject => !physicsObject.IsStatic))
        {
            // Apply world gravity
            if (Gravity != 0)
            {
                var weight = new AeroVec2(0.0f, physicsObject.Mass * Gravity);
                physicsObject.ApplyForce(weight);
            }

            // Apply any global forces
            foreach (var force in _globalForces)
            {
                physicsObject.ApplyForce(force);
            }

            // Integrate the object
            _integrator.IntegrateLinear(physicsObject, dt);
            _integrator.IntegrateAngular(physicsObject, dt);
        }

        if(PerformanceMonitoringEnabled)
            _performanceMonitor.EndStep(_bodyCount, _particleCount);
    }
}