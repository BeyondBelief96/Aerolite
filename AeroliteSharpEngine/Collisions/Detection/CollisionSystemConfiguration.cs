using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Collisions.Detection.NarrowPhase;
using AeroliteSharpEngine.Collisions.Resolution.Interfaces;
using AeroliteSharpEngine.Collisions.Resolution.Resolvers;

namespace AeroliteSharpEngine.Collisions.Detection;

public class CollisionSystemConfiguration
{
    /// <summary>
    /// The spatial broad phase algorithm to use.
    /// </summary>
    public IBroadPhase BroadPhase { get; private set; }

    /// <summary>
    /// The narrow phase collision system. This is determined based on <see cref="CollisionSystemType"/>.
    /// </summary>
    public INarrowPhase NarrowPhase { get; private set; }

    /// <summary>
    /// The type of shapes the collision system is processing. This will determine the types of
    /// algorithms used during narrow phase detection.
    /// </summary>
    public CollisionSystemType Type { get; private set; }
    
    /// <summary>
    /// The collision resolver implementation to use for resolving contacts between bodies.
    /// </summary>
    public ICollisionResolver CollisionResolver { get; private set; }
    
    /// <summary>
    /// The type of bounding area to use for early out collision detection tests.
    /// </summary>
    public BoundingAreaType BoundingAreaType { get; private set; }
    
    /// <summary>
    /// Boolean value to toggle validation of convex shapes at runtime. Can be toggled off to
    /// save performance if you're confident in the models/shapes being used.
    /// </summary>
    public bool ValidateConvexShapes { get; private set; }

    public CollisionSystemConfiguration(CollisionSystemType type, IBroadPhase broadPhase, ICollisionResolver collisionResolver, BoundingAreaType boundingAreaType = BoundingAreaType.AABB,  bool validateConvexShapes = false)
    {
        Type = type;
        BroadPhase = broadPhase;
        BoundingAreaType = boundingAreaType;
        CollisionResolver = collisionResolver;
        if (type == CollisionSystemType.ConvexOnly)
        {
            ValidateConvexShapes = validateConvexShapes;
            NarrowPhase = new ConvexShapeCollisionDetector();
        }
        else
        {
            ValidateConvexShapes = false;
            NarrowPhase = new GeneralShapeCollisionDetector();
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="CollisionSystemConfiguration"/> with the given <see cref="CollisionSystemType"/>
    /// </summary>
    /// <param name="type">The collision system type to use.</param>
    /// <returns></returns>
    public CollisionSystemConfiguration WithType(CollisionSystemType type)
    {
        Type = type;
        return this;
    }

    /// <summary>
    /// Creates an instance of <see cref="CollisionSystemConfiguration"/> with the given <see cref="IBroadPhase"/>
    /// </summary>
    /// <param name="broadPhase">The broad phase implementation to use.</param>
    /// <returns></returns>
    public CollisionSystemConfiguration WithBroadPhase(IBroadPhase broadPhase)
    {
        BroadPhase = broadPhase;
        return this;
    }
    
    /// <summary>
    /// Creates an instance of <see cref="CollisionSystemConfiguration"/> with the given <see cref="BoundingAreaType"/>
    /// </summary>
    /// <param name="boundingAreaType">The bounding area type to use.</param>
    /// <returns></returns>
    public CollisionSystemConfiguration WithBoundingAreaType(BoundingAreaType boundingAreaType)
    {
        BoundingAreaType = boundingAreaType;
        return this;
    }
    
    /// <summary>
    /// Creates an instance of <see cref="CollisionSystemConfiguration"/> with the given <see cref="ICollisionResolver"/>
    /// </summary>
    /// <param name="collisionResolver">The collision resolver implementation to use.</param>
    /// <returns></returns>
    public CollisionSystemConfiguration WithCollisionResolver(ICollisionResolver collisionResolver)
    {
        CollisionResolver = collisionResolver;
        return this;
    }
    
    /// <summary>
    /// Create the engine's collision system with default values. These include:
    /// 1. Supporting only convex shapes.
    /// 2. A uniform grid with 100x100 cell width for broad phase testing.
    /// 3. Axis-Aligned bounding boxes for early out testing.
    /// </summary>
    /// <returns></returns>
    public static CollisionSystemConfiguration Default()
    {
        return new CollisionSystemConfiguration(CollisionSystemType.ConvexOnly, new UniformGrid(BoundingAreaType.AABB), new ProjectionCollisionResolver());
    }
}