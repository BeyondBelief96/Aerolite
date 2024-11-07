using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.Collisions.Detection.BroadPhase;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes;

public class AngularMotionScene : Scene
{
    private readonly IAeroPhysicsWorld _world;
    private const float RotationForce = 500.0f;
    private readonly Dictionary<IBody2D, float> _bodyTorques = new Dictionary<IBody2D, float>();
    
    public AngularMotionScene(Game game, Screen screen, Sprites sprites, Shapes shapes) 
        : base(game, screen, sprites, shapes)
    {
        var config = AeroWorldConfiguration.Default.WithGravity(0.0f);
        _world = new AeroWorld2D(config);
        CreateTestBodies();
    }

    private void CreateTestBodies()
    {
        // Create a circle
        var circle = new AeroCircle(30f);
        var circleBody = new AeroBody2D(200, Screen.Height / 2, 1.0f, circle);
        _world.AddPhysicsObject(circleBody);
        _bodyTorques[circleBody] = RotationForce;

        // Create a box
        var box = new AeroBox(60f, 60f);
        var boxBody = new AeroBody2D(400, Screen.Height / 2, 1.0f, box);
        _world.AddPhysicsObject(boxBody);
        _bodyTorques[boxBody] = RotationForce;
        
        // Create a triangle
        var triangle = new AeroTriangle(100, 100);
        var triangleBody = new AeroBody2D(600, Screen.Height / 2, 1.0f, triangle);
        _world.AddPhysicsObject(triangleBody);
        _bodyTorques[triangleBody] = RotationForce;

        // Create a pentagon
        var pentagon = new AeroRegularPolygon(3, 50);
        var pentagonBody = new AeroBody2D(800, Screen.Height / 2, 1.0f, pentagon);
        _world.AddPhysicsObject(pentagonBody);
        _bodyTorques[pentagonBody] = RotationForce;

        // Apply initial torques
        foreach (var (body, torque) in _bodyTorques)
        {
            body.ApplyTorque(torque);
        }
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = FlatKeyboard.Instance;
        
        // Camera zoom controls
        if (keyboard.IsKeyDown(Keys.A))
            Camera.IncZoom();
        if (keyboard.IsKeyDown(Keys.Z))
            Camera.DecZoom();

        // Toggle rotation direction with space
        if (keyboard.IsKeyClicked(Keys.Space))
        {
            foreach (var body in _world.GetDynamicBodies())
            {
                _bodyTorques[body] *= -1;  // Reverse the stored torque
            }
        }

        // Reset rotations with R
        if (keyboard.IsKeyClicked(Keys.R))
        {
            foreach (var body in _world.GetDynamicBodies())
            {
                body.Angle = 0;
                body.AngularVelocity = 0;
                _bodyTorques[body] = RotationForce;  // Reset to initial torque
            }
        }

        // Apply stored torques each frame
        foreach (var (body, torque) in _bodyTorques)
        {
            body.ApplyTorque(torque);
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _world.Update(dt);
    }

    protected override void DrawScene(GameTime gameTime)
    {
        foreach (var body in _world.GetDynamicBodies())
        {
            // Transform body position to render space
            var renderPos = CoordinateSystem.ScreenToRender(
                new Vector2(body.Position.X, body.Position.Y),
                Screen.Width,
                Screen.Height
            );
    
            switch (body.Shape)
            {
                case AeroCircle circle:
                    // Draw circle with a radius line to show rotation
                    Shapes.DrawCircle(
                        renderPos.X, 
                        renderPos.Y, 
                        circle.Radius, 
                        32, 
                        Color.White
                    );
                    
                    // Calculate endpoint for rotation indicator line
                    var endPoint = new Vector2(
                        renderPos.X + circle.Radius * MathF.Cos(body.Angle),
                        renderPos.Y + circle.Radius * MathF.Sin(body.Angle)
                    );
                    
                    Shapes.DrawLine(
                        renderPos,
                        endPoint, 
                        Color.Red
                    );
                    break;
    
                case AeroPolygon polygon:
                    // Transform all polygon vertices to render space
                    var vertices = polygon.WorldVertices
                        .Select(v => CoordinateSystem.ScreenToRender(
                            new Vector2(v.X, v.Y),
                            Screen.Width,
                            Screen.Height))
                        .ToList();
                    
                    // Draw polygon edges
                    Shapes.DrawPolygon(vertices.ToArray(), Color.White);
                    
                    // Draw center point
                    Shapes.DrawCircleFill(renderPos.X, renderPos.Y, 2, 8, Color.Red);
                    break;
            }
        }
    
        // Optionally, draw the coordinate system axes for debugging
        var (left, right, bottom, top) = CoordinateSystem.GetRenderBounds(Screen.Width, Screen.Height);
        Shapes.DrawLine(new Vector2(left, 0), new Vector2(right, 0), Color.Yellow);  // X axis
        Shapes.DrawLine(new Vector2(0, bottom), new Vector2(0, top), Color.Yellow);  // Y axis
    }
}