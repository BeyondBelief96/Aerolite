using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collision;
using AeroliteSharpEngine.Interfaces;

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
    /// Returns an iterable list of the current objects present in the world.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IPhysicsObject> GetObjects();

    /// <summary>
    /// Returns an iterable list of the current collisions for current time step of the world.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<CollisionManifold> GetCollisions();

    /// <summary>
    /// Allows a user to register a force generator with a specific physics object. 
    /// These force generators will be applied to their registered objects during the time 
    /// step update.
    /// </summary>
    /// <param name="physicsObject"></param>
    /// <param name="generator"></param>
    void AddForceGenerator(IPhysicsObject physicsObject, IForceGenerator generator);

    /// <summary>
    /// Adds a global force to the world. These apply to all non-static objects during the physics time step update.
    /// Could be used to simulate a constant wind for example.
    /// </summary>
    /// <param name="force"></param>
    void AddGlobalForce(AeroVec2 force);

    /// <summary>
    /// Adds a physics object to the world to be simulated.
    /// </summary>
    /// <param name="physicsObject"></param>
    void AddPhysicsObject(IPhysicsObject physicsObject);

    /// <summary>
    /// Clears the world of all physics objects, and forces.
    /// </summary>
    void ClearWorld();

    /// <summary>
    /// Provides a way to change the integrator used for the world. This allows you to update the integrator 
    /// on the fly if you wanted to do so dynamically. 
    /// </summary>
    /// <param name="integrator"></param>
    void SetIntegrator(IIntegrator integrator);

    /// <summary>
    /// The main workhorse of the physics world. This simulates the current timestep for all current
    /// physics objects in the world, and applies all forces and performs integration.
    /// </summary>
    /// <param name="dt"></param>
    void Update(float dt);
}