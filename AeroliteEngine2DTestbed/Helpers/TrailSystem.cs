using System;
using System.Collections.Generic;
using System.Linq;
using AeroliteSharpEngine.Core.Interfaces;
using Microsoft.Xna.Framework;
using Flat.Graphics;

namespace AeroliteEngine2DTestbed.Helpers;

/// <summary>
/// Stores and manages visual trails for physics objects
/// </summary>
public class TrailSystem
{
    private readonly Dictionary<IPhysicsObject2D, Queue<Vector2>> trails = new();
    private readonly int maxLength;
    private readonly float opacityDecay;

    public TrailSystem(int maxLength = 50, float opacityDecay = 0.95f)
    {
        this.maxLength = maxLength;
        this.opacityDecay = opacityDecay;
    }

    /// <summary>
    /// Update the trail for a specific object
    /// </summary>
    public void UpdateTrail(IPhysicsObject2D obj, Vector2 position)
    {
        if (!trails.ContainsKey(obj))
        {
            trails[obj] = new Queue<Vector2>();
        }

        var trail = trails[obj];
        trail.Enqueue(position);

        while (trail.Count > maxLength)
        {
            trail.Dequeue();
        }
    }

    /// <summary>
    /// Draw the trail for a specific object
    /// </summary>
    public void DrawTrail(IPhysicsObject2D obj, Color baseColor, Shapes shapes)
    {
        if (!trails.ContainsKey(obj) || trails[obj].Count < 2) return;

        var positions = trails[obj].ToArray();
        
        // Draw trail segments with decreasing opacity
        for (int i = 0; i < positions.Length - 1; i++)
        {
            float opacity = MathF.Pow(opacityDecay, positions.Length - i - 1);
            Color trailColor = baseColor * opacity * 0.5f;
            
            shapes.DrawLine(positions[i], positions[i + 1], trailColor);
        }
    }

    /// <summary>
    /// Remove trails for objects that no longer exist
    /// </summary>
    public void CleanupTrails(IEnumerable<IPhysicsObject2D> activeObjects)
    {
        var deadObjects = trails.Keys.Where(obj => 
            !activeObjects.Contains(obj)).ToList();
        
        foreach (var obj in deadObjects)
        {
            trails.Remove(obj);
        }
    }

    /// <summary>
    /// Clear all trails
    /// </summary>
    public void ClearTrails()
    {
        trails.Clear();
    }
}

/// <summary>
/// Extension methods for easy trail system integration
/// </summary>
public static class TrailSystemExtensions
{
    public static void DrawWithTrail(
        this IPhysicsObject2D physicsObject,
        TrailSystem trailSystem,
        Color color,
        Shapes shapes,
        Screen screen)
    {
        // Update and draw trail
        var renderPos = CoordinateSystem.ScreenToRender(
            new Vector2(physicsObject.Position.X, physicsObject.Position.Y),
            screen.Width,
            screen.Height
        );
        
        trailSystem.UpdateTrail(physicsObject, renderPos);
        trailSystem.DrawTrail(physicsObject, color, shapes);
        
        // Draw the actual object
        AeroDrawingHelpers.DrawPhysicsObject2D(physicsObject, color, shapes, screen);
    }
}