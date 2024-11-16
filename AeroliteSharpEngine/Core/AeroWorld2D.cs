using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.ForceGenerators;
using AeroliteSharpEngine.Interfaces;
using AeroliteSharpEngine.Performance;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core;

public class AeroWorld2D : IAeroPhysicsWorld
{
    
    #region Private Fields
    private readonly List<IPhysicsObject2D> _dynamicObjects = [];
    private readonly List<IPhysicsObject2D> _staticObjects = [];
    private readonly List<AeroVec2> _globalForces = [];
    private readonly ForceRegistry _forceRegistry = new();
    private readonly IPerformanceMonitor _performanceMonitor;
    private AeroWorldConfiguration _configuration;
    #endregion

    #region Properties
    public float Gravity 
    { 
        get => _configuration.Gravity;
        set => _configuration = _configuration.WithGravity(value);
    }
    
    public bool PerformanceMonitoringEnabled
    {
        get => _configuration.EnablePerformanceMonitoring;
        set => _configuration = _configuration.WithPerformanceMonitoring(value);
    }
    
    public ICollisionSystem CollisionSystem { get; private set; }
    public SimulationBounds Bounds => _configuration.SimulationBounds;
    #endregion

    #region Events
    public event Action<SimulationBounds>? OnBoundsChanged;
    public event Action<IPhysicsObject2D>? OnObjectRemoved;
    public event Action<IPhysicsObject2D>? OnObjectAdded;
    #endregion

    #region Constructor
    public AeroWorld2D(AeroWorldConfiguration configuration) 
    {
        _configuration = configuration;
        _performanceMonitor = new ConsolePerformanceLogger();
        CollisionSystem = new CollisionSystem(configuration.CollisionSystemConfiguration);
    }

    public AeroWorld2D() : this(AeroWorldConfiguration.Default)
    {
    }
    #endregion

    #region Configuration Management
    public void UpdateConfiguration(AeroWorldConfiguration newConfig)
    {
        var oldBounds = _configuration.SimulationBounds;
        _configuration = newConfig;
        
        if (!oldBounds.Equals(newConfig.SimulationBounds))
        {
            UpdateBroadPhase();
            OnBoundsChanged?.Invoke(newConfig.SimulationBounds);
        }

        CollisionSystem = new CollisionSystem(newConfig.CollisionSystemConfiguration);
    }

    public AeroWorldConfiguration GetConfiguration() => _configuration;
    #endregion

    #region Object Management
    public void AddPhysicsObject(IPhysicsObject2D obj)
    {
        if (obj.IsStatic)
            _staticObjects.Add(obj);
        else
            _dynamicObjects.Add(obj);
        
        OnObjectAdded?.Invoke(obj);
    }

    public void RemovePhysicsObject(IPhysicsObject2D obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        
        // Remove from appropriate list
        if (obj.IsStatic)
            _staticObjects.Remove(obj);
        else
            _dynamicObjects.Remove(obj);

        // ForceRegistry will handle cleanup of all associated force generators
        _forceRegistry.RemoveObject(obj);
        
        OnObjectRemoved?.Invoke(obj);
    }

    public void ClearWorld()
    {
        _dynamicObjects.Clear();
        _staticObjects.Clear();
        _globalForces.Clear();
        _forceRegistry.Clear();
        CollisionSystem.Clear();
    }

    public void ClearDynamicObjects()
    {
        // Remove force registrations for all dynamic objects
        foreach (var obj in _dynamicObjects)
        {
            _forceRegistry.RemoveObject(obj);
        }
        
        _dynamicObjects.Clear();
        _globalForces.Clear();
        CollisionSystem.Clear();
    }
    #endregion

    #region Force Management
    
    public void AddForceGenerator(IPhysicsObject2D? obj, IForceGenerator? generator)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(generator);
        
        // Only add force generators to objects that exist in our world
        if (_dynamicObjects.Contains(obj) || _staticObjects.Contains(obj))
        {
            _forceRegistry.Add(obj, generator);
        }
        else
        {
            throw new InvalidOperationException("Cannot add force generator to object not in world.");
        }
    }

    public void RemoveForceGenerator(IPhysicsObject2D? obj, IForceGenerator? generator)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(generator);
        
        _forceRegistry.RemoveRegistration(obj, generator);
    }

    public void AddGlobalForce(AeroVec2 force) 
        => _globalForces.Add(force);

    public void ClearGlobalForces() 
        => _globalForces.Clear();
    #endregion

    #region Bounds Management
    public void ResizeSimulation(float width, float height)
        => UpdateBounds(new SimulationBounds(width, height, Bounds.RemovalThreshold));

    public void SetRemovalThreshold(float threshold)
        => UpdateBounds(new SimulationBounds(Bounds.Width, Bounds.Height, threshold));

    private void UpdateBounds(SimulationBounds newBounds)
    {
        _configuration = _configuration.WithBounds(
            newBounds.Width, 
            newBounds.Height, 
            newBounds.RemovalThreshold
        );
        
        UpdateBroadPhase();
        OnBoundsChanged?.Invoke(newBounds);
    }

    private void UpdateBroadPhase()
    {
        if (_configuration.CollisionSystemConfiguration.BroadPhase is DynamicQuadTree quadTree)
        {
            var newQuadTree = new DynamicQuadTree(
                Bounds.Width,
                Bounds.Height,
                new AeroVec2(Bounds.Left, Bounds.Top),
                quadTree.BoundingAreaType,
                quadTree.MaxObjectsPerNode,
                quadTree.MaxDepth
            );

            _configuration = _configuration.WithCollisionSystemConfiguration(
                _configuration.CollisionSystemConfiguration.WithBroadPhase(newQuadTree)
            );
            
            CollisionSystem = new CollisionSystem(_configuration.CollisionSystemConfiguration);
        }
    }
    #endregion

    #region Query Methods
    public IReadOnlyList<IPhysicsObject2D> GetObjects() 
        => _dynamicObjects.Concat(_staticObjects).ToList();
    
    public IReadOnlyList<IPhysicsObject2D> GetDynamicObjects() 
        => _dynamicObjects.Where(x => !x.IsStatic).ToList();
    
    public IReadOnlyList<IPhysicsObject2D> GetStaticObjects() 
        => _staticObjects.Where(x => !x.IsStatic).ToList();
    
    public IReadOnlyList<IBody2D> GetDynamicBodies() 
        => _dynamicObjects.OfType<IBody2D>().ToList();
    
    public IReadOnlyList<AeroParticle2D> GetDynamicParticles() 
        => _dynamicObjects.OfType<AeroParticle2D>().ToList();

    public IEnumerable<CollisionManifold> GetCollisions() 
        => CollisionSystem.Collisions;
    #endregion

    #region Simulation Update
    public void Update(float dt)
    {
        if(PerformanceMonitoringEnabled)
            _performanceMonitor.BeginStep();

        UpdateSimulationStep(dt);
 
        if(PerformanceMonitoringEnabled)
            _performanceMonitor.EndStep(this);
    }

    private void UpdateSimulationStep(float dt)
    {
        _forceRegistry.UpdateForces(dt);
        RemoveOutOfBoundsObjects();
        
        UpdateDynamicObjects(dt);
        UpdateStaticObjects();
        
        CollisionSystem.HandleCollisions(GetObjects());
    }

    private void UpdateDynamicObjects(float dt)
    {
        foreach (var obj in _dynamicObjects)
        {
            ApplyForces(obj);
            IntegrateObject(obj, dt);
            obj.UpdateGeometry();
        }
    }

    private void UpdateStaticObjects()
    {
        foreach (var obj in _staticObjects)
        {
            obj.UpdateGeometry();
        }
    }

    private void ApplyForces(IPhysicsObject2D obj)
    {
        if (Gravity != 0)
        {
            var weight = new AeroVec2(0.0f, obj.Mass * Gravity);
            obj.ApplyForce(weight);
        }

        foreach (var force in _globalForces)
        {
            obj.ApplyForce(force);
        }
    }

    private void IntegrateObject(IPhysicsObject2D obj, float dt)
    {
        _configuration.Integrator.IntegrateLinear(obj, dt);
        _configuration.Integrator.IntegrateAngular(obj, dt);
    }

    private void RemoveOutOfBoundsObjects()
    {
        var objectsToRemove = _dynamicObjects
            .Where(IsOutOfBounds)
            .ToList();

        foreach (var obj in objectsToRemove)
        {
            RemovePhysicsObject(obj);
        }
    }

    private bool IsOutOfBounds(IPhysicsObject2D obj)
        => Bounds.IsOutsideRemovalThreshold(obj.Position);
    #endregion
}