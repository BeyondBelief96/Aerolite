using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Integrators;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Core;

public struct AeroWorldConfiguration 
{
    /// <summary>
    /// Configuration value for the force of gravity to act on all objects in the physics world every time step.
    /// </summary>
    public float Gravity { get; private set; }
    
    /// <summary>
    /// Configuration object for the engines collision system.
    /// </summary>
    public CollisionSystemConfiguration CollisionSystemConfiguration { get; private set; }
    
    /// <summary>
    /// Configuration object to define the integration scheme used during the physics time step.
    /// </summary>
    public IIntegrator Integrator { get; private set; }
    
    /// <summary>
    /// Configuration value to enable/disable performance metric logging.
    /// </summary>
    public bool EnablePerformanceMonitoring { get; private set; }
    
    /// <summary>
    /// Creates the world with default configurations. These include
    /// 1. Gravity = 0.0f,
    /// 2. Default collision system configuration
    /// 3. Euler integrator
    /// 4. Performance monitoring disabled. 
    /// </summary>
    public static AeroWorldConfiguration Default => new()
    {
        Gravity = 0f,
        CollisionSystemConfiguration = Collisions.Detection.CollisionSystemConfiguration.Default(),
        Integrator = new EulerIntegrator(),
        EnablePerformanceMonitoring = false
    };
    
    /// <summary>
    /// Configures the physics world to have a global gravity value.
    /// </summary>
    /// <param name="gravity">The value to set the global gravity value of the physics world to.</param>
    /// <returns>An instance of <see cref="AeroWorldConfiguration"/></returns>
    public AeroWorldConfiguration WithGravity(float gravity)
    {
        Gravity = gravity;
        return this;
    }
    
    /// <summary>
    /// Configures the physics world to use the passed in integration scheme.
    /// </summary>
    /// <param name="integrator">An instance of <see cref="IIntegrator"/> which defines an integration scheme.</param>
    /// <returns>An instance of <see cref="AeroWorldConfiguration"/></returns>
    public AeroWorldConfiguration WithIntegrator(IIntegrator integrator)
    {
        Integrator = integrator;
        return this;
    }

    public AeroWorldConfiguration WithCollisionSystemConfiguration(
        CollisionSystemConfiguration collisionSystemConfiguration)
    {
        CollisionSystemConfiguration = collisionSystemConfiguration;
        return this;
    }
    
    /// <summary>
    /// Configures the physics world to have performance monitoring enabled/disabled based on the value.
    /// </summary>
    /// <param name="enabled">The value to enable/disable performance monitoring.</param>
    /// <returns>An instance of <see cref="AeroWorldConfiguration"/></returns>
    public AeroWorldConfiguration WithPerformanceMonitoring(bool enabled)
    {
        EnablePerformanceMonitoring = enabled;
        return this;
    }
}