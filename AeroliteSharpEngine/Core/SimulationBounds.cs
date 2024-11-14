using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Core;

/// <summary>
/// Struct that holds information about the bounds of the simulation space.
/// </summary>
public readonly struct SimulationBounds
{
    public float Left { get; }
    public float Right { get; }
    public float Top { get; }
    public float Bottom { get; }
    public float Width { get; }
    public float Height { get; }
    public float RemovalThreshold { get; }

    public SimulationBounds(float width, float height, float removalThreshold = 100f)
    {
        Width = width;
        Height = height;
        RemovalThreshold = removalThreshold;
        
        // Calculate bounds assuming (0,0) is top-left
        Left = 0;
        Right = width;
        Top = 0;
        Bottom = height;
    }

    public bool IsInBounds(AeroVec2 position, float objectRadius = 0)
    {
        return position.X + objectRadius >= Left && 
               position.X - objectRadius <= Right &&
               position.Y + objectRadius >= Top && 
               position.Y - objectRadius <= Bottom;
    }

    public bool IsOutsideRemovalThreshold(AeroVec2 position, float objectRadius = 0)
    {
        return position.X + objectRadius < Left - RemovalThreshold || 
               position.X - objectRadius > Right + RemovalThreshold ||
               position.Y + objectRadius < Top - RemovalThreshold || 
               position.Y - objectRadius > Bottom + RemovalThreshold;
    }
}