using System;
using System.Collections.Generic;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.ForceGenerators;
using AeroliteSharpEngine.Integrators;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class CoulombDemoScene : Scene
{
    private readonly AeroWorld2D world;
    private readonly Color protonColor = new Color(255, 50, 50);    
    private readonly Color electronColor = new Color(50, 50, 255);  
    private readonly Random random = new Random();
    
    // Demo configuration
    private const float PARTICLE_MASS = 1.0f;
    private const float PARTICLE_RADIUS = 8.0f;
    private const float PROTON_CHARGE = 1.0f;
    private const float ELECTRON_CHARGE = -1.0f;
    private const float FORCE_SCALE = 2e-6f;
    private const float DAMPING = 0.999f;
    private const int MAX_PARTICLES = 700;
    private const float SPAWN_INTERVAL = 0.01f;
    
    private readonly TrailSystem trailSystem;
    
    // Pattern configuration
    private float spawnTimer = 0f;
    private int currentMode = 0;
    private const int MODE_COUNT = 7;
    private const float ROTATION_SPEED = 0.5f;
    private readonly List<ChargedParticle2D> staticCharges = new();

    public CoulombDemoScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
        : base(game, screen, sprites, shapes)
    {  
        var config = AeroWorldConfiguration.Default
            .WithGravity(0.0f)
            .WithPerformanceMonitoring(true)
            .WithIntegrator(new RK4Integrator())
            .WithCollisionSystemConfiguration(CollisionSystemConfiguration.Default().WithBroadPhase(new UniformGrid(BoundingAreaType.BoundingCircle, 20.0f)));
        
        world = new AeroWorld2D(config);
        Camera.Zoom = 1;
        
        trailSystem = new TrailSystem(maxLength: 50, opacityDecay: 0.95f);
        
        SetupCurrentMode();
    }

    private void SetupCurrentMode()
    {
        world.ClearWorld();
        staticCharges.Clear();
        trailSystem.ClearTrails(); // Clear trails when changing modes
        
        switch (currentMode)
        {
            case 0: SetupAtomicModel(); break;
            case 1: SetupMoleculeModel(); break;
            case 2: SetupThreeBodyProblem(); break;
            case 3: SetupDoubleSpiral(); break;
            case 4: SetupParticleAccelerator(); break;
            case 5: SetupElectricField(); break;
            case 6: SetupSolarSystemAnalog(); break;
        }
    }
    
    private void SetupMoleculeModel()
    {
        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        float moleculeSpacing = 150f;

        // Create two nuclei (like a diatomic molecule)
        Vector2[] nucleiPositions = 
        {
            new(center.X - moleculeSpacing/2, center.Y),
            new(center.X + moleculeSpacing/2, center.Y)
        };

        // Add protons to each nucleus
        foreach (var pos in nucleiPositions)
        {
            int nucleusSize = 2;
            float nucleusRadius = PARTICLE_RADIUS * 2;
            
            for (int i = 0; i < nucleusSize; i++)
            {
                float angle = (float)(i * Math.PI * 2 / nucleusSize);
                float x = pos.X + nucleusRadius * (float)Math.Cos(angle);
                float y = pos.Y + nucleusRadius * (float)Math.Sin(angle);
                
                var proton = new ChargedParticle2D(
                    x, y, mass: 0f,
                    charge: PROTON_CHARGE * 3,
                    radius: PARTICLE_RADIUS * 1.2f
                );
                world.AddPhysicsObject(proton);
                staticCharges.Add(proton);
            }
        }

        // Add orbital guides in figure-8 pattern around both nuclei
        int orbitalPoints = 16;
        for (int i = 0; i < orbitalPoints; i++)
        {
            float t = i * MathF.PI * 2 / orbitalPoints;
            float x = center.X + moleculeSpacing * 0.7f * (float)Math.Cos(t);
            float y = center.Y + moleculeSpacing * 0.4f * (float)Math.Sin(2 * t);

            var guide = new ChargedParticle2D(
                x, y, mass: 0f,
                charge: ELECTRON_CHARGE * 0.3f,
                radius: PARTICLE_RADIUS * 0.5f
            );
            world.AddPhysicsObject(guide);
            staticCharges.Add(guide);
        }
    }

    private void SetupThreeBodyProblem()
    {
        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        float orbitRadius = Math.Min(Screen.Width, Screen.Height) * 0.2f;
        
        // Create three massive charged bodies in a triangular configuration
        for (int i = 0; i < 3; i++)
        {
            float angle = i * MathF.PI * 2 / 3;
            float x = center.X + orbitRadius * (float)Math.Cos(angle);
            float y = center.Y + orbitRadius * (float)Math.Sin(angle);

            var body = new ChargedParticle2D(
                x, y, mass: 0f,
                charge: (i % 2 == 0) ? PROTON_CHARGE * 4 : ELECTRON_CHARGE * 4,
                radius: PARTICLE_RADIUS * 2
            );
            world.AddPhysicsObject(body);
            staticCharges.Add(body);
        }
    }

    private void SetupSolarSystemAnalog()
    {
        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        
        // Central "sun" (strong positive charge)
        var sun = new ChargedParticle2D(
            center.X, center.Y,
            mass: 0f,
            charge: PROTON_CHARGE * 5,
            radius: PARTICLE_RADIUS * 2.5f
        );
        world.AddPhysicsObject(sun);
        staticCharges.Add(sun);

        // Create planetary orbits with alternating charges
        int numOrbits = 4;
        for (int orbit = 0; orbit < numOrbits; orbit++)
        {
            float orbitRadius = Math.Min(Screen.Width, Screen.Height) * (0.1f + orbit * 0.08f);
            int pointsInOrbit = 8 + orbit * 4;
            
            for (int i = 0; i < pointsInOrbit; i++)
            {
                float angle = (float)(i * Math.PI * 2 / pointsInOrbit);
                float x = center.X + orbitRadius * (float)Math.Cos(angle);
                float y = center.Y + orbitRadius * (float)Math.Sin(angle);

                var guide = new ChargedParticle2D(
                    x, y, mass: 0f,
                    charge: orbit % 2 == 0 ? ELECTRON_CHARGE * 0.4f : PROTON_CHARGE * 0.4f,
                    radius: PARTICLE_RADIUS * 0.4f
                );
                world.AddPhysicsObject(guide);
                staticCharges.Add(guide);
            }
        }
    }

    private void SetupDoubleSpiral()
    {
        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        float radius = Math.Min(Screen.Width, Screen.Height) * 0.35f;
        int numSpirals = 2;
        int pointsPerSpiral = 8;

        for (int spiral = 0; spiral < numSpirals; spiral++)
        {
            for (int i = 0; i < pointsPerSpiral; i++)
            {
                float angle = (float)(i * Math.PI * 2 / pointsPerSpiral + spiral * Math.PI / numSpirals);
                float spiralRadius = radius * (1 - (float)i / (pointsPerSpiral * 1.5f));
                float x = center.X + spiralRadius * (float)Math.Cos(angle);
                float y = center.Y + spiralRadius * (float)Math.Sin(angle);

                var charge = new ChargedParticle2D(
                    x, y, mass: 0f,
                    charge: spiral == 0 ? PROTON_CHARGE * 2 : ELECTRON_CHARGE * 2,
                    radius: PARTICLE_RADIUS
                );
                
                world.AddPhysicsObject(charge);
                staticCharges.Add(charge);
            }
        }
    }

    private void SetupAtomicModel()
    {
        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        
        // Central nucleus (cluster of protons)
        int nucleusSize = 3;
        float nucleusRadius = PARTICLE_RADIUS * 3;
        for (int i = 0; i < nucleusSize; i++)
        {
            float angle = (float)(i * Math.PI * 2 / nucleusSize);
            float x = center.X + nucleusRadius * (float)Math.Cos(angle);
            float y = center.Y + nucleusRadius * (float)Math.Sin(angle);
            
            var proton = new ChargedParticle2D(
                x, y, mass: 0f,
                charge: PROTON_CHARGE * 3,
                radius: PARTICLE_RADIUS * 1.5f
            );
            world.AddPhysicsObject(proton);
            staticCharges.Add(proton);
        }

        // Orbital guides (invisible charges to help shape electron paths)
        int orbits = 2;
        int pointsPerOrbit = 8;
        for (int orbit = 0; orbit < orbits; orbit++)
        {
            float orbitRadius = Math.Min(Screen.Width, Screen.Height) * (0.15f + orbit * 0.1f);
            for (int i = 0; i < pointsPerOrbit; i++)
            {
                float angle = (float)(i * Math.PI * 2 / pointsPerOrbit);
                float x = center.X + orbitRadius * (float)Math.Cos(angle);
                float y = center.Y + orbitRadius * (float)Math.Sin(angle);

                var guide = new ChargedParticle2D(
                    x, y, mass: 0f,
                    charge: ELECTRON_CHARGE * 0.5f,
                    radius: PARTICLE_RADIUS * 0.5f
                );
                world.AddPhysicsObject(guide);
                staticCharges.Add(guide);
            }
        }
    }

    private void SetupParticleAccelerator()
    {
        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        int segments = 16;
        float innerRadius = Math.Min(Screen.Width, Screen.Height) * 0.2f;
        float outerRadius = Math.Min(Screen.Width, Screen.Height) * 0.35f;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)(i * Math.PI * 2 / segments);
            
            // Inner ring
            float x1 = center.X + innerRadius * (float)Math.Cos(angle);
            float y1 = center.Y + innerRadius * (float)Math.Sin(angle);
            
            // Outer ring
            float x2 = center.X + outerRadius * (float)Math.Cos(angle);
            float y2 = center.Y + outerRadius * (float)Math.Sin(angle);

            var innerCharge = new ChargedParticle2D(
                x1, y1, mass: 0f,
                charge: i % 2 == 0 ? PROTON_CHARGE * 2 : ELECTRON_CHARGE * 2,
                radius: PARTICLE_RADIUS
            );
            
            var outerCharge = new ChargedParticle2D(
                x2, y2, mass: 0f,
                charge: i % 2 == 0 ? ELECTRON_CHARGE * 2 : PROTON_CHARGE * 2,
                radius: PARTICLE_RADIUS
            );

            world.AddPhysicsObject(innerCharge);
            world.AddPhysicsObject(outerCharge);
            staticCharges.Add(innerCharge);
            staticCharges.Add(outerCharge);
        }
    }

    private void SetupElectricField()
    {
        int rows = 30;
        int cols = 30;
        float spacing = Math.Min(Screen.Width, Screen.Height) * 0.15f;
        Vector2 start = new(
            Screen.Width / 2f - (cols - 1) * spacing / 2f,
            Screen.Height / 2f - (rows - 1) * spacing / 2f
        );

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float x = start.X + j * spacing;
                float y = start.Y + i * spacing;

                var charge = new ChargedParticle2D(
                    x, y, mass: 0f,
                    charge: ((i + j) % 4 < 2) ? PROTON_CHARGE * 2 : ELECTRON_CHARGE * 2,
                    radius: PARTICLE_RADIUS
                );
                
                world.AddPhysicsObject(charge);
                staticCharges.Add(charge);
            }
        }
    }

    // Modify SpawnDynamicParticle for the three-body problem mode
    private void SpawnDynamicParticle()
    {
        if (world.GetDynamicObjects().Count >= MAX_PARTICLES) return;

        Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
        float spawnRadius = Math.Min(Screen.Width, Screen.Height) * 0.45f;
        float angle = (float)(random.NextDouble() * Math.PI * 2);
    
        // Spawn position
        float x = center.X + spawnRadius * (float)Math.Cos(angle);
        float y = center.Y + spawnRadius * (float)Math.Sin(angle);

        // Adjust velocity based on mode
        float speedFactor = currentMode == 2 ? 100f : 150f; // Slower for three-body problem
        AeroVec2 velocity;
        
        if (currentMode == 2) // Three-body problem
        {
            // Add slight inward spiral for more interesting orbits
            velocity = new AeroVec2(
                (center.X - x),
                (center.Y - y)
            );
        }
        else
        {
            // Original tangential velocity
            velocity = new AeroVec2(
                -(y - center.Y),
                (x - center.X)
            );
        }
        
        velocity = velocity.UnitVector() * speedFactor;

        bool isProton = world.GetDynamicObjects().Count % 2 == 0;
        
        var particle = new ChargedParticle2D(
            x, y, PARTICLE_MASS * 0.1f, // Lighter particles for better orbital motion
            isProton ? PROTON_CHARGE : ELECTRON_CHARGE,
            radius: PARTICLE_RADIUS
        )
        {
            Damping = currentMode == 2 ? 0.9999f : DAMPING, // Less damping for three-body
            Velocity = velocity
        };

        // Add Coulomb interactions
        foreach (var obj in world.GetObjects())
        {
            if (obj is ChargedParticle2D existingParticle)
            {
                world.AddForceGenerator(particle, 
                    new CoulombForceGenerator(existingParticle, FORCE_SCALE));
            
                if (!existingParticle.IsStatic)
                {
                    world.AddForceGenerator(existingParticle, 
                        new CoulombForceGenerator(particle, FORCE_SCALE));
                }
            }
        }

        world.AddPhysicsObject(particle);
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Auto-spawn particles
        spawnTimer += dt;
        if (spawnTimer >= SPAWN_INTERVAL)
        {
            SpawnDynamicParticle();
            spawnTimer = 0;
        }

        // Rotate static charges in some modes
        if (currentMode == 0 || currentMode == 2) // Double spiral and accelerator modes
        {
            Vector2 center = new(Screen.Width / 2f, Screen.Height / 2f);
            foreach (var charge in staticCharges)
            {
                float dx = charge.Position.X - center.X;
                float dy = charge.Position.Y - center.Y;
                float radius = (float)Math.Sqrt(dx * dx + dy * dy);
                float angle = (float)Math.Atan2(dy, dx) + ROTATION_SPEED * dt;
                
                charge.Position = new AeroVec2(
                    center.X + radius * (float)Math.Cos(angle),
                    center.Y + radius * (float)Math.Sin(angle)
                );
            }
        }

        // Change mode with space
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.Space))
        {
            currentMode = (currentMode + 1) % MODE_COUNT;
            SetupCurrentMode();
        }

        // Reset scene if R is pressed
        if (FlatKeyboard.Instance.IsKeyClicked(Keys.R))
        {
            SetupCurrentMode();
        }

        // Keep particles in bounds
        AeroDrawingHelpers.KeepParticleInScreenBounds(world.GetDynamicObjects(), Screen);

        world.Update(dt);
    }

    protected override void DrawScene(GameTime gameTime)
    {
        // Draw grid for reference
        //AeroDrawingHelpers.DrawGrid(Screen, Shapes, 50f);

        // Draw all particles with trails
        foreach (var obj in world.GetObjects())
        {
            if (obj is ChargedParticle2D particle)
            {
                Color color = particle.Charge > 0 ? protonColor : electronColor;
                if (particle.IsStatic)
                {
                    color *= (currentMode == 1 && Math.Abs(particle.Charge) < PROTON_CHARGE) ? 0.2f : 0.6f;
                }
                
                // Using the extension method for easy drawing with trails
                particle.DrawWithTrail(trailSystem, color, Shapes, Screen);
            }
        }

        // Cleanup any trails for destroyed particles
        trailSystem.CleanupTrails(world.GetDynamicObjects());
    }
}