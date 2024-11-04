﻿using System;
using System.Collections.Generic;
using AeroliteEngine2DTestbed.Helpers;
using AeroliteSharpEngine.Collisions.Detection;
using AeroliteSharpEngine.Collisions.Resolution.Resolvers;
using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Shapes;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AeroliteEngine2DTestbed.Scenes
{
    public class ProjectionMethodDebugScene : Scene
    {
        private readonly AeroWorld2D world;
        private readonly List<(string name, Func<float, float, AeroShape2D> creator)> shapeCreators;
        private int currentShapeIndex = 0;
        private readonly AeroBody2D? centerCircle;
        private readonly Color centerColor = new Color(100, 150, 255);
        private readonly Color spawnedColor = new Color(255, 150, 100);

        public ProjectionMethodDebugScene(Game game, Screen screen, Sprites sprites, Shapes shapes)
            : base(game, screen, sprites, shapes)
        {
            // Create shape creators
            shapeCreators = new List<(string, Func<float, float, AeroShape2D>)>
            {
                ("Circle", (x, y) => new AeroCircle(25)),
                ("Box", (x, y) => new AeroBox(40, 40)),
                ("Triangle", (x, y) => new AeroRegularPolygon(3, 30)),
                ("Pentagon", (x, y) => new AeroRegularPolygon(5, 30)),
                ("Hexagon", (x, y) => new AeroRegularPolygon(6, 30))
            };

            // Initialize world with no gravity
            var config = AeroWorldConfiguration.Default
                .WithGravity(0)
                .WithPerformanceMonitoring(true)
                .WithCollisionSystemConfiguration(CollisionSystemConfiguration.Default()
                    .WithCollisionResolver(new ProjectionCollisionResolver()));
            
            world = new AeroWorld2D(config);
            
            // Create large center circle
            centerCircle = new AeroBody2D(
                screen.Width / 2,
                screen.Height / 2,
                0.0f, // Heavy mass for stability
                new AeroCircle(100) // Large radius
            );
            
            world.AddPhysicsObject(centerCircle);
            
            _camera.Zoom = 1;
        }

        public override void Update(GameTime gameTime)
        {
            // Handle shape cycling with space key
            if (FlatKeyboard.Instance.IsKeyClicked(Keys.Space))
            {
                currentShapeIndex = (currentShapeIndex + 1) % shapeCreators.Count;
            }

            // Handle mouse click for spawning shapes
            if (FlatMouse.Instance.IsLeftMouseButtonPressed())
            {
                Vector2 mousePos = FlatMouse.Instance.GetMouseScreenPosition(_game, _screen);
                SpawnShapeAtPosition(mousePos.X, mousePos.Y);
            }

            world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void SpawnShapeAtPosition(float x, float y)
        {
            var (_, creator) = shapeCreators[currentShapeIndex];
            var body = new AeroBody2D(x, y, 1.0f, creator(x, y));

            world.AddPhysicsObject(body);
        }

        public override void Draw(GameTime gameTime)
        {
            _screen.Set();
            _game.GraphicsDevice.Clear(new Color(10, 10, 20));
            _shapes.Begin(_camera);

            // Draw center circle
            if (centerCircle != null)
            {
                AeroDrawingHelpers.DrawBody(centerCircle, centerColor, _shapes, _screen);
            }
            
            AeroDrawingHelpers.DrawGrid(_screen, _shapes, 100);

            // Draw all other bodies
            foreach (var body in world.GetObjects())
            {
                if (!body.Equals(centerCircle))
                {
                    AeroDrawingHelpers.DrawBody(body as AeroBody2D, spawnedColor, _shapes, _screen);
                }
            }

            // Draw collision contact points
            foreach (var manifold in world.CollisionSystem.Collisions)
            {
                AeroDrawingHelpers.DrawCollisionInfo(manifold, _screen, _shapes);
            }

            _shapes.End();
            _screen.Unset();
            _screen.Present(_sprites);
        }
    }
}