# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

### Building the Solution
```bash
# Build entire solution
dotnet build Aerolite.sln

# Build specific projects
dotnet build AeroliteSharpEngine/AeroliteSharpEngine.csproj
dotnet build AeroliteEngine2DTestbed/AeroliteEngine2DTestbed.csproj

# Build in Release mode
dotnet build Aerolite.sln -c Release

# Build for specific platform
dotnet build Aerolite.sln -c Debug --arch x64
```

### Running the Demo Application
```bash
# Run the 2D testbed demo
dotnet run --project AeroliteEngine2DTestbed/AeroliteEngine2DTestbed.csproj
```

### Important Configuration
- Target Framework: .NET 10.0
- Platforms: AnyCPU, x64
- **USE_DOUBLE**: Conditional compilation symbol defined for double precision (instead of float)

---

## Project Structure

### 1. AeroliteSharpEngine (Core Physics Engine)

Pure .NET physics simulation library with no graphics dependencies.

#### AeroMath
Custom math types optimized for physics:
- **AeroVec2**, **AeroVec3**: 2D/3D vectors
- **AeroMat3x3**, **AeroMat3x4**: Matrices for transformations
- **AeroQuaternion**: 3D rotations

#### Physics Objects - 2D
- **IPhysicsObject2D**: Base interface for all 2D physics objects
- **AeroParticle2D**: Point-mass particle (no rotation)
- **AeroBody2D**: Rigid body with rotation and angular dynamics

#### Physics Objects - 3D (Recently Added)
- **IPhysicsObject3D**: Base interface for 3D physics objects
- **IBody3D**: Interface for 3D rigid bodies
- **AeroParticle3D**: 3D point-mass particle
- **AeroBody3D**: Full 3D rigid body with quaternion-based orientation

#### Physics World Management
- **IAeroPhysicsWorld**: Interface for physics world
- **AeroWorld2D**: 2D physics world implementation
- **AeroWorldConfiguration**: Configuration object with builder pattern for world settings (gravity, bounds, integrator, collision system)

#### Force System
- **IForceGenerator**: Interface for force generators
- **ForceRegistry2D**: Manages associations between objects and force generators
- Force implementations: **GravitationalForceGenerator**, **DragForceGenerator**, **CoulombForceGenerator**

#### Shapes (2D)
- **AeroShape2D**: Base class for all shapes
- Implementations: **AeroCircle**, **AeroBox**, **AeroPolygon**, **AeroTriangle**

#### Collision System
- **Broad Phase**: Spatial partitioning to reduce collision checks
  - **UniformGrid**: Fixed-size cells, good for evenly distributed objects
  - **DynamicQuadTree**: Adaptive subdivision, good for clustered objects
- **Narrow Phase**: Precise collision detection
  - **SAT** (Separating Axis Theorem): Fast for convex polygons
  - **GJK** (Gilbert-Johnson-Keerthi): General convex algorithm
- **Resolution**: **ImpulseMethodCollisionResolver** applies physics-based collision response

#### Integration Schemes
- **EulerIntegrator**: Fast, first-order (default)
- **VerletIntegrator**: Better stability, position-based
- **Rk4Integrator**: Fourth-order Runge-Kutta, most accurate

#### Object Pooling
Performance optimization via pooling:
- **ObjectPool<T>**: Generic pool
- **CollisionManifoldPool**, **ContactPointPool**: Reduce GC pressure

---

### 2. Flat (2D Graphics/Input Wrapper)

Wrapper around MonoGame providing 2D graphics and input:
- **FlatCamera**: 2D camera system
- **FlatScreen**: Screen management
- **Shapes**, **Sprites**: Rendering primitives
- **FlatInput**, **FlatKeyboard**, **FlatMouse**, **FlatGamePad**: Input handling
- **FlatMath**, **FlatUtil**: Helper utilities

---

### 3. AeroliteMonoGraphics (3D Graphics Foundation)

3D rendering foundation with MonoGame:
- **Renderer**: 3D rendering pipeline
- **Camera**: 3D camera
- **Screen**: Display management
- **Scene**: 3D scene management

---

### 4. AeroliteEngine2DTestbed (2D Demo Application)

MonoGame-based demo application running at 60 FPS:
- **Game1**: Main game class
- **Scene**: Base class for demo scenes
- **Demo Scenes**:
  - **CollisionDetectionDebugScene**: Visual collision feedback
  - **CoulombDemoScene**: Electrostatic force simulation
  - **QuadTreeDebugScene**: Quad tree visualization
  - **ImpulseTestScene**: Impulse-based collision testing
  - **SolarSystemScene**: N-body gravitational simulation
  - **UniformGridDebugScene**: Spatial grid visualization
- **Helpers**: **AeroDrawingHelpers**, **DebugDrawing**, **TrailRenderer**, **Utils**

---

## Core Architecture

### Simulation Loop (Per Frame)
1. **Update Forces**: Apply force generators to objects
2. **Integrate**: Update positions and velocities using integration scheme
3. **Collision Detection**: Broad phase → Narrow phase
4. **Collision Resolution**: Apply impulse-based response
5. **Update Geometry**: Transform shape vertices based on new positions/rotations

### Design Patterns
- **Interface Segregation**: IPhysicsObject2D/3D, IForceGenerator, IIntegrator
- **Strategy Pattern**: Swappable collision detection, integration, and resolution algorithms
- **Factory Pattern**: CollisionDetectorFactory for creating appropriate detectors
- **Configuration Objects**: Builder pattern for AeroWorldConfiguration
- **Object Pooling**: Reduce allocations and GC pressure
- **Event-Driven**: Events for object addition/removal, bounds changes

---

## Key Interfaces & Hierarchy

### Physics Object Hierarchy
```
IPhysicsObject2D/3D
  └─ Physics2DObjectBase/Physics3DObjectBase (abstract base with common properties)
      └─ AeroParticle2D/3D (point mass, no rotation)
      └─ IBody2D/3D (interface adding rotation)
          └─ AeroBody2D/3D (full rigid body implementation)
```

### Configuration
**AeroWorldConfiguration** provides builder methods:
- `.WithGravity(value)`
- `.WithBounds(width, height)`
- `.WithIntegrator(type)`
- `.WithCollisionSystem(broadPhase, narrowPhase)`

### Collision Pipeline
**ICollisionSystem** orchestrates:
1. Broad Phase (spatial partitioning)
2. Narrow Phase (precise detection)
3. Resolution (physics response)

---

## 2D Physics Details

- **Coordinate System**: X → right, Y → up
- **Mass**: Scalar value (0 = static/infinite mass)
- **Inertia**: Calculated per-shape based on mass and geometry
- **Damping**: Default 0.99 (energy loss per frame)
- **Forces**: Accumulated and cleared each frame during integration
- **Impulses**: Instantaneous velocity changes (used in collision resolution)

---

## 3D Physics Details (Recent Addition)

The codebase is actively adding 3D physics support:

- **Math Types**: AeroVec3, AeroQuaternion, AeroMat3x4
- **6-DOF Motion**: 3 linear + 3 rotational degrees of freedom
- **Orientation**: Quaternion-based (avoids gimbal lock)
- **Angular Dynamics**: AngularVelocity, AngularAcceleration, AngularMomentum
- **Transforms**: `AeroMat3x4.FromQuaternionAndPosition()` for world transforms

### Why Quaternions?
- **Compact**: 4 values vs 9 for rotation matrix
- **Smooth interpolation**: SLERP (Spherical Linear Interpolation)
- **No gimbal lock**: Unlike Euler angles
- **Efficient composition**: Quaternion multiplication

---

## Recent Changes (from Git History)

Based on recent commits:
- "Refactor physics engine and remove legacy code"
- "Add 3D physics support and refactor core systems"
- "Refactoring a few things regarding the force registry"
- "Adding OnObjectAdded event"
- "Cleaning up code"

Modified files in working directory:
- `AeroMath/AeroQuaternion.cs` - Quaternion improvements
- `Core/Interfaces/IPhysicsObject3D.cs` - 3D physics object interface
- `Core/3D/AeroBody3D.cs` - New 3D rigid body
- `Core/Interfaces/IBody3D.cs` - New 3D body interface

---

## Usage Examples

### Creating a 2D Physics World
```csharp
var config = AeroWorldConfiguration.Default
    .WithGravity(9.81f)
    .WithBounds(1024, 768)
    .WithIntegrator(IntegratorType.Euler)
    .WithCollisionSystem(BroadPhaseType.UniformGrid, NarrowPhaseType.SAT);

var world = new AeroWorld2D(config);
```

### Adding Objects and Simulating
```csharp
// Create a particle
var particle = new AeroParticle2D(x: 100, y: 100, mass: 1.0f, radius: 5f);
world.AddPhysicsObject(particle);

// Create a rigid body with a box shape
var body = new AeroBody2D(mass: 2.0f, new AeroBox(width: 50, height: 30));
body.Position = new AeroVec2(200, 300);
world.AddPhysicsObject(body);

// Update simulation (typically called each frame)
world.Update(deltaTime: 1f/60f);
```

### Applying Forces
```csharp
// Create and register a drag force
var dragForce = new DragForceGenerator(k1: 0.1f, k2: 0.05f);
world.ForceRegistry.Register(particle, dragForce);

// Apply custom force directly
particle.AddForce(new AeroVec2(100, 0)); // Apply force to the right
```

### Working with 3D Objects
```csharp
var body3d = new AeroBody3D(mass: 5.0f);
body3d.Position = new AeroVec3(0, 10, 0);

// Set orientation using quaternion (45° rotation around Y-axis)
body3d.Orientation = new AeroQuaternion(
    angle: MathF.PI / 4,
    axis: new AeroVec3(0, 1, 0)
);

// Apply angular velocity
body3d.AngularVelocity = new AeroVec3(0, 1, 0); // Spin around Y-axis
```

---

## Performance Considerations

### Collision Detection Trade-offs
1. **Broad Phase**:
   - **UniformGrid**: Best for evenly distributed objects, predictable performance
   - **DynamicQuadTree**: Adapts to clustering, better for variable density

2. **Narrow Phase**:
   - **SAT**: Fast for simple convex polygons
   - **GJK**: More general but slower

3. **Integration**:
   - **Euler**: Fastest, acceptable for most games
   - **RK4**: Most accurate, use for stable simulations

4. **Object Pooling**: Reduces GC allocations - already implemented for manifolds/contacts

5. **Validation**: Can disable shape validation after development for performance

---

## Common Pitfalls

1. **Static Objects Not Colliding**: Objects with mass=0 are static and can collide but won't move
2. **Unstable Simulations**: Try RK4 integrator or reduce timestep
3. **Performance Drops**: Check broad phase choice - DynamicQuadTree may help with clusters
4. **Penetration Artifacts**: Increase collision resolution iterations or reduce timestep
5. **Missing Collisions**: Ensure configuration matches shape types (SAT requires convex shapes)

---

## Event System

**AeroWorld2D** provides events:
- **OnObjectAdded**: Fired when object enters simulation
- **OnObjectRemoved**: Fired when object leaves simulation
- **OnBoundsChanged**: Fired when simulation space changes
- **GetCollisions()**: Access current frame's collision manifolds

Use for UI updates, sound effects, visual feedback, game logic.

---

## Configuration Best Practices

### For Stability (physics simulations, scientific)
- Use **Rk4Integrator**
- Use **DynamicQuadTree** for broad phase
- Use **GJK** for narrow phase
- Lower timestep (1/120 or 1/240)

### For Performance (fast-paced games)
- Use **EulerIntegrator**
- Use **UniformGrid** for broad phase
- Use **SAT** for narrow phase
- Standard timestep (1/60)

### For Prototyping
- Use **AeroWorldConfiguration.Default**

---

## Integration with Flat Graphics

Typical rendering loop:
1. **Update Physics**: `world.Update(deltaTime)`
2. **Draw Shapes**: Use **AeroDrawingHelpers** to bridge physics and graphics
3. **Render**: `shapes.DrawShape(physicsObject.Shape, color)`

**AeroDrawingHelpers** converts physics shapes to renderable graphics primitives.

---

## Extending the Engine

### Custom Force Generators
```csharp
public class CustomForce : IForceGenerator
{
    public void UpdateForce(IPhysicsObject2D obj, float dt)
    {
        // Calculate and apply force
        obj.AddForce(calculatedForce);
    }
}

// Register with world
world.ForceRegistry.Register(myObject, new CustomForce());
```

### Custom Integration Schemes
```csharp
public class CustomIntegrator : IIntegrator
{
    public void IntegrateLinear(IPhysicsObject2D obj, float dt) { /* ... */ }
    public void IntegrateAngular(IBody2D body, float dt) { /* ... */ }
}
```

### Custom Collision Resolvers
Extend **ICollisionResolver** and implement specialized collision response logic.

---

## Debugging Tips

1. **Enable Performance Monitoring**: Set `PerformanceMonitoringEnabled = true` in configuration
2. **Visualize Collisions**: Use debug drawing helpers in demo scenes
3. **Check Configuration**: Ensure collision system matches shape types
4. **Validate Shapes**: Enable `ValidateConvexShapes` (has performance cost)
5. **Monitor Object Count**: `world.GetObjects().Count`
6. **Frame-by-frame Analysis**: Use demo scenes with keyboard controls to step through simulation

---

## Key Files to Understand First

### Core Architecture
- `AeroliteSharpEngine/Core/Interfaces/IAeroPhysicsWorld.cs` - World interface
- `AeroliteSharpEngine/Core/2D/AeroWorld2D.cs` - 2D world implementation
- `AeroliteSharpEngine/Collisions/Detection/Interfaces/ICollisionSystem.cs` - Collision pipeline
- `AeroliteSharpEngine/Core/Configuration/AeroWorldConfiguration.cs` - Configuration system

### For 3D Understanding
- `AeroliteSharpEngine/Core/Interfaces/IPhysicsObject3D.cs` - 3D physics interface
- `AeroliteSharpEngine/Core/3D/AeroBody3D.cs` - 3D rigid body implementation
- `AeroliteSharpEngine/AeroMath/AeroQuaternion.cs` - Quaternion math

### For Examples
- `AeroliteEngine2DTestbed/Game1.cs` - Main demo application entry point
- `AeroliteEngine2DTestbed/Scenes/` - Various demo scenes showing different features

---

## Summary

Aerolite is a modular, well-architected physics engine featuring:
- **Clean Separation**: Physics core is independent of graphics
- **Flexible Configuration**: Strategy patterns and builder configuration
- **2D & 3D Support**: Mature 2D system with emerging 3D capabilities
- **Performance Optimized**: Object pooling, spatial partitioning, swappable algorithms
- **Event-Driven**: Easy integration with games and applications
- **Well-Documented Examples**: Multiple demo scenes showing usage patterns

**Main Entry Point**: Create `AeroWorld2D`, add physics objects, call `Update()` each frame.
**Graphics Integration**: Use Flat library for 2D visualization and input handling.
