namespace AeroliteSharpEngine.Core.Interfaces;

/// <summary>
/// Represents an object that can carry electric charge
/// </summary>
public interface IChargedObject
{
    /// <summary>
    /// The amount of electric charge carried by this object.
    /// </summary>
    float Charge { get; }
}