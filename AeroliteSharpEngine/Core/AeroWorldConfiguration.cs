using AeroliteSharpEngine.Collisions;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Integrators;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Core;

public struct AeroWorldConfiguration 
{
    public float Gravity { get; set; }
    
    public CollisionSystemConfiguration CollisionSystemConfiguration { get; private set; }
    public IIntegrator Integrator { get; private set; }
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
    
    public AeroWorldConfiguration WithGravity(float gravity)
    {
        Gravity = gravity;
        return this;
    }

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

    public AeroWorldConfiguration WithPerformanceMonitoring(bool enabled)
    {
        EnablePerformanceMonitoring = enabled;
        return this;
    }
}