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

public class AeroWorld2D(AeroWorldConfiguration configuration) : IAeroPhysicsWorld
{
    #region Fields
    
    private readonly List<IPhysicsObject2D> _dynamicObjects = [];
    private readonly List<IPhysicsObject2D> _staticObjects = [];
    private readonly List<AeroVec2> _globalForces = [];
    private readonly ForceRegistry _forceRegistry = new();
    private readonly IPerformanceMonitor _performanceMonitor = new ConsolePerformanceLogger();
    private AeroWorldConfiguration _configuration = configuration;
    
    private SimulationBounds Bounds => _configuration.SimulationBounds;
    
    #endregion
    
    #region Properties
    
    public float Gravity { get; set; } = configuration.Gravity;
    public ICollisionSystem CollisionSystem { get; private set; } = new CollisionSystem(configuration.CollisionSystemConfiguration);
    public bool PerformanceMonitoringEnabled { get; set; } = configuration.EnablePerformanceMonitoring;
    
    #endregion
    
    #region Constructor
    
    public AeroWorld2D() : this(AeroWorldConfiguration.Default)
    {
    }
    
    #endregion
    
    #region Public Methods
    
    #region Bounds Management Methods

    /// <summary>
    /// Updates the simulation bounds with new dimensions
    /// </summary>
    public void ResizeSimulation(float width, float height)
    {
        UpdateBounds(new SimulationBounds(width, height, Bounds.RemovalThreshold));
    }

    /// <summary>
    /// Updates the removal threshold for out-of-bounds objects
    /// </summary>
    public void SetRemovalThreshold(float threshold)
    {
        UpdateBounds(new SimulationBounds(Bounds.Width, Bounds.Height, threshold));
    }

    /// <summary>
    /// Updates all simulation bounds parameters
    /// </summary>
    public void UpdateBounds(float width, float height, float threshold)
    {
        UpdateBounds(new SimulationBounds(width, height, threshold));
    }

    private void UpdateBounds(SimulationBounds newBounds)
    {
        // Update configuration
        _configuration = _configuration.WithBounds(
            newBounds.Width, 
            newBounds.Height, 
            newBounds.RemovalThreshold
        );

        // Update broadphase if needed
        UpdateBroadPhase();
    }

    #endregion

    public AeroWorldConfiguration GetConfiguration() => _configuration;

    public void UpdateConfiguration(AeroWorldConfiguration newConfig)
    {
        _configuration = newConfig;
        Gravity = newConfig.Gravity;
        PerformanceMonitoringEnabled = newConfig.EnablePerformanceMonitoring;
        CollisionSystem = new CollisionSystem(newConfig.CollisionSystemConfiguration);
    }

    public IReadOnlyList<IPhysicsObject2D> GetObjects() => _dynamicObjects.Concat(_staticObjects).ToList();
    
    public IReadOnlyList<IPhysicsObject2D> GetDynamicObjects() => _dynamicObjects.Where(x => !x.IsStatic).ToList();
    
    public IReadOnlyList<IPhysicsObject2D> GetStaticObjects() => _staticObjects.Where(x => !x.IsStatic).ToList();
    
    public IReadOnlyList<IBody2D> GetDynamicBodies() => _dynamicObjects.OfType<IBody2D>().ToList();
    
    public IReadOnlyList<AeroParticle2D> GetDynamicParticles() => _dynamicObjects.OfType<AeroParticle2D>().ToList();

    public IEnumerable<CollisionManifold> GetCollisions() => CollisionSystem.Collisions;

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
        _dynamicObjects.Clear();
        _globalForces.Clear();
        _forceRegistry.Clear();
        CollisionSystem.Clear();
    }

    public void AddPhysicsObject(IPhysicsObject2D obj)
    {
        if (obj.IsStatic)
            _staticObjects.Add(obj);
        else
            _dynamicObjects.Add(obj);
    }

    // Remove by direct object reference
    public void RemovePhysicsObject(IPhysicsObject2D obj)
    {
        if (obj.IsStatic)
            _staticObjects.Remove(obj);
        else
            _dynamicObjects.Remove(obj);
    }

    public void AddForceGenerator(IPhysicsObject2D particle, IForceGenerator generator)
    {
        _forceRegistry.Add(particle, generator);
    }

    public void AddGlobalForce(AeroVec2 force)
    {
        _globalForces.Add(force);
    }

    public void ClearGlobalForces()
    {
        _globalForces.Clear();
    }

    public void Update(float dt)
    {
        if(PerformanceMonitoringEnabled)
            _performanceMonitor.BeginStep();

        _forceRegistry.UpdateForces(dt);
        
        RemoveOutOfBoundsObjects();

        foreach (var physicsObject in _dynamicObjects)
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
            
            _configuration.Integrator.IntegrateLinear(physicsObject, dt);
            _configuration.Integrator.IntegrateAngular(physicsObject, dt);
            physicsObject.UpdateGeometry();
        }
        
        // Also update static bodies - they might be rotated/moved by the user
        foreach (var physicsObject in _staticObjects)
        {
            physicsObject.UpdateGeometry();
        }
        
        //TODO: Maybe optimize how the collision system handles both static/dynamic objects.
        // Currently this has allocate a new list in O(n) time each from which is not great.

        for (int n = 0; n < 1; n++)
        {
            CollisionSystem.HandleCollisions(GetObjects());
        }
 
        if(PerformanceMonitoringEnabled)
            _performanceMonitor.EndStep(this);
    }
    
    #endregion
    
    #region Private Methods
    
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
    {
        return Bounds.IsOutsideRemovalThreshold(obj.Position);
    }

    
    #endregion
}