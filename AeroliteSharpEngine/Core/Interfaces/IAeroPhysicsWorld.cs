using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.CollisionPrimitives;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Core.Interfaces;

/// <summary>
/// Interface for the core physics simulation world. This is the main entry point for interacting
/// with the Aerolite physics engine.
/// </summary>
public interface IAeroPhysicsWorld
{
    #region Properties and Events

    /// <summary>
    /// The gravitational acceleration applied to all non-static objects.
    /// </summary>
    float Gravity { get; set; }

    /// <summary>
    /// Controls whether performance monitoring is enabled.
    /// </summary>
    bool PerformanceMonitoringEnabled { get; set; }

    /// <summary>
    /// The collision detection and resolution system.
    /// </summary>
    ICollisionSystem CollisionSystem { get; }

    /// <summary>
    /// The current bounds of the simulation space.
    /// </summary>
    SimulationBounds Bounds { get; }

    /// <summary>
    /// Triggered when simulation bounds are changed.
    /// </summary>
    event Action<SimulationBounds>? OnBoundsChanged;

    /// <summary>
    /// Triggered when an object is removed from the simulation.
    /// </summary>
    event Action<IPhysicsObject2D>? OnObjectRemoved;

    #endregion

    #region Configuration

    /// <summary>
    /// Gets the current world configuration.
    /// </summary>
    AeroWorldConfiguration GetConfiguration();

    /// <summary>
    /// Updates the world with a new configuration.
    /// </summary>
    void UpdateConfiguration(AeroWorldConfiguration config);

    #endregion

    #region Object Management

    /// <summary>
    /// Adds a physics object to the simulation.
    /// </summary>
    void AddPhysicsObject(IPhysicsObject2D obj);

    /// <summary>
    /// Removes a physics object from the simulation.
    /// </summary>
    void RemovePhysicsObject(IPhysicsObject2D obj);

    /// <summary>
    /// Clears all objects and forces from the world.
    /// </summary>
    void ClearWorld();

    /// <summary>
    /// Clears only dynamic objects, leaving static objects intact.
    /// </summary>
    void ClearDynamicObjects();

    #endregion

    #region Force Management

    /// <summary>
    /// Registers a force generator with a specific physics object.
    /// </summary>
    void AddForceGenerator(IPhysicsObject2D obj, IForceGenerator generator);

    /// <summary>
    /// Adds a global force that affects all non-static objects.
    /// </summary>
    void AddGlobalForce(AeroVec2 force);

    /// <summary>
    /// Removes all global forces from the simulation.
    /// </summary>
    void ClearGlobalForces();

    #endregion

    #region Bounds Management

    /// <summary>
    /// Updates the simulation space dimensions.
    /// </summary>
    void ResizeSimulation(float width, float height);

    /// <summary>
    /// Sets the threshold distance beyond bounds at which objects are removed.
    /// </summary>
    void SetRemovalThreshold(float threshold);

    #endregion

    #region Queries

    /// <summary>
    /// Returns all objects (static and dynamic) in the simulation.
    /// </summary>
    IReadOnlyList<IPhysicsObject2D> GetObjects();

    /// <summary>
    /// Returns all non-static objects in the simulation.
    /// </summary>
    IReadOnlyList<IPhysicsObject2D> GetDynamicObjects();

    /// <summary>
    /// Returns all static objects in the simulation.
    /// </summary>
    IReadOnlyList<IPhysicsObject2D> GetStaticObjects();

    /// <summary>
    /// Returns all rigid bodies in the simulation.
    /// </summary>
    IReadOnlyList<IBody2D> GetDynamicBodies();

    /// <summary>
    /// Returns all particles in the simulation.
    /// </summary>
    IReadOnlyList<AeroParticle2D> GetDynamicParticles();

    /// <summary>
    /// Returns current collision manifolds from the last update.
    /// </summary>
    IEnumerable<CollisionManifold> GetCollisions();

    #endregion

    #region Simulation

    /// <summary>
    /// Advances the simulation by the specified time step.
    /// </summary>
    void Update(float dt);

    #endregion
}