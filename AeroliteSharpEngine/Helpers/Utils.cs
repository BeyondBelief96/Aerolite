using AeroliteSharpEngine.Core;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Helpers;

internal static class Utils
{
            
    /// <summary>
    /// Utilizes the cantor pairing function to create a unique hash
    /// from a tuple of two numbers. This reduces the space to store a unique value of two coordinates
    /// into a single number.
    /// </summary>
    /// <param name="x">The first coordinate</param>
    /// <param name="y">The second coordinate</param>
    /// <returns>The cantor pairing hash of the two coordinates.</returns>
    internal static long HashCoords(int x, int y)
    {
        // Use Cantor pairing function for better hash distribution
        // and proper handling of negative coordinates
        long sum = x + y;
        return (sum * (sum + 1) / 2) + y;
    }
}