namespace AeroliteSharpEngine.AeroMath;

public static class AeroMathExtensions
{
    /// <summary>
    /// The machine epsilon value for float (single precision)
    /// </summary>
    private const float FloatEpsilon = 1.192093E-07f;
    
    /// <summary>
    /// The machine epsilon value for double (double precision)
    /// </summary>
    public const double DoubleEpsilon = 2.2204460492503131E-16;
    
    /// <summary>
    /// Default tolerance for float comparisons, slightly larger than machine epsilon
    /// </summary>
    private const float DefaultTolerance = FloatEpsilon * 10f;

    public static float Clamp(float value, float min, float max)
    {
        if (value < min) value = min;
        if(value > max) value = max;
        return value;
    }
    
    /// <summary>
    /// Determines if a float value is nearly zero within a specified tolerance
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="tolerance">Optional tolerance value, defaults to DEFAULT_TOLERANCE</param>
    /// <returns>True if the value is within the tolerance of zero</returns>
    public static bool IsNearlyZero(float value, float tolerance = DefaultTolerance)
    {
        return Math.Abs(value) <= tolerance;
    }

    /// <summary>
    /// Determines if a double value is nearly zero within a specified tolerance
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="tolerance">Optional tolerance value, defaults to machine epsilon * 10</param>
    /// <returns>True if the value is within the tolerance of zero</returns>
    public static bool IsNearlyZero(double value, double tolerance = DoubleEpsilon * 10)
    {
        return Math.Abs(value) <= tolerance;
    }

    /// <summary>
    /// Compares two float values for equality within a specified tolerance
    /// </summary>
    /// <param name="a">First value to compare</param>
    /// <param name="b">Second value to compare</param>
    /// <param name="tolerance">Optional tolerance value, defaults to DEFAULT_TOLERANCE</param>
    /// <returns>True if the values are within the tolerance of each other</returns>
    public static bool AreEqual(float a, float b, float tolerance = DefaultTolerance)
    {
        return Math.Abs(a - b) <= tolerance;
    }

    /// <summary>
    /// Compares two double values for equality within a specified tolerance
    /// </summary>
    /// <param name="a">First value to compare</param>
    /// <param name="b">Second value to compare</param>
    /// <param name="tolerance">Optional tolerance value, defaults to machine epsilon * 10</param>
    /// <returns>True if the values are within the tolerance of each other</returns>
    public static bool AreEqual(double a, double b, double tolerance = DoubleEpsilon * 10)
    {
        return Math.Abs(a - b) <= tolerance;
    }

    /// <summary>
    /// Compares two float values using relative error tolerance
    /// This is more suitable for comparing larger numbers where absolute tolerance might not be appropriate
    /// </summary>
    /// <param name="a">First value to compare</param>
    /// <param name="b">Second value to compare</param>
    /// <param name="maxRelativeError">Maximum relative error tolerance</param>
    /// <returns>True if the values are within the relative tolerance of each other</returns>
    public static bool AreEqualRelative(float a, float b, float maxRelativeError = DefaultTolerance)
    {
        // Handle exact equality (also handles both values being zero)
        if (a == b) return true;
        
        // Get the absolute values
        float absA = Math.Abs(a);
        float absB = Math.Abs(b);
        
        // Handle case where one value is zero - use absolute comparison
        if (absA == 0 || absB == 0)
            return Math.Abs(a - b) <= maxRelativeError;

        // Find the largest absolute value
        float largest = Math.Max(absA, absB);
        
        // Calculate relative error
        return Math.Abs(a - b) <= largest * maxRelativeError;
    }

    /// <summary>
    /// Determines if a float value is effectively negative zero
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value represents negative zero</returns>
    public static bool IsNegativeZero(float value)
    {
        return BitConverter.GetBytes(value)[3] == 0x80;
    }

    /// <summary>
    /// Safe division that handles division by near-zero values
    /// </summary>
    /// <param name="numerator">The numerator value</param>
    /// <param name="denominator">The denominator value</param>
    /// <param name="tolerance">Optional tolerance for zero check</param>
    /// <returns>The division result, or float.MaxValue if denominator is effectively zero</returns>
    public static float SafeDivide(float numerator, float denominator, float tolerance = DefaultTolerance)
    {
        if (IsNearlyZero(denominator, tolerance))
            return float.MaxValue;
        return numerator / denominator;
    }
}