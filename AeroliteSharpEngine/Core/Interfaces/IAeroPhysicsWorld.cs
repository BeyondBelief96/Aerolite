using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Core.Interfaces;

/// <summary>
/// Interface for using and interacting with the created physics world. This is the core of the Aerolite physics engine.
/// </summary>
public interface IAeroPhysicsWorld
{
    /// <summary>
    /// The gravitational constant used for the world. If this is anything but 0, this will be applied to all non-static objects
    /// during the time step update.
    /// </summary>
    float Gravity { get; set; }


    /// <summary>
    /// Used to toggle performance monitoring for the physics engine.
    /// </summary>
    bool PerformanceMonitoringEnabled { get; set; }
    
    /// <summary>
    /// Provides access to configure the physics engine's underlying collision system.
    /// </summary>
    /// <returns></returns>
    ICollisionSystem CollisionSystem { get; }

    /// <summary>
    /// Returns an iterable list of the current objects present in the world.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IPhysicsObject2D> GetObjects();

    /// <summary>
    /// Returns any physics objects in the world that implement <see cref="IBody2D"/>
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IBody2D> GetBodies();

    /// <summary>
    /// Returns any physics object in the world that are of type <see cref="AeroParticle2D"/>
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<AeroParticle2D> GetParticles();

    /// <summary>
    /// Returns an iterable list of the current collisions for current time step of the world.
    /// </summary>
    /// <returns></returns>
    IEnumerable<CollisionManifold> GetCollisions();
    
    /// <summary>
    /// Returns the current world configuration.
    /// </summary>
    /// <returns></returns>
    AeroWorldConfiguration GetConfiguration();
    
    /// <summary>
    /// Used to update the world with a new configuration.
    /// </summary>
    /// <param name="config"></param>
    void UpdateConfiguration(AeroWorldConfiguration config);

    /// <summary>
    /// Allows a user to register a force generator with a specific physics object. 
    /// These force generators will be applied to their registered objects during the time 
    /// step update.
    /// </summary>
    /// <param name="physicsObject2D"></param>
    /// <param name="generator"></param>
    void AddForceGenerator(IPhysicsObject2D physicsObject2D, IForceGenerator generator);

    /// <summary>
    /// Adds a global force to the world. These apply to all non-static objects during the physics time step update.
    /// Could be used to simulate a constant wind for example.
    /// </summary>
    /// <param name="force"></param>
    void AddGlobalForce(AeroVec2 force);

    /// <summary>
    /// Adds a physics object to the world to be simulated.
    /// </summary>
    /// <param name="physicsObject2D"></param>
    void AddPhysicsObject(IPhysicsObject2D physicsObject2D);

    /// <summary>
    /// Clears the world of all physics objects, and forces.
    /// </summary>
    void ClearWorld();

    /// <summary>
    /// The main workhorse of the physics world. This simulates the current timestep for all current
    /// physics objects in the world, and applies all forces and performs integration.
    /// </summary>
    /// <param name="dt"></param>
    void Update(float dt);
}