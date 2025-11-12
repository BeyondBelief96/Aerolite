using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core;

/// <summary>
/// Extension of <see cref="AeroParticle2D"/> to include electrical properties
/// </summary>
public class ChargedParticle2D(
    float x,
    float y,
    float mass,
    float charge,
    float restitution = 0.5f,
    float friction = 0.5f,
    float radius = 10.0f)
    : AeroParticle2D(x, y, mass, restitution, friction, radius), IChargedObject
{
    public float Charge { get; } = charge;
}