using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core._3D;

namespace AeroliteSharpEngine.Core
{
    public class AeroParticle3D : Physics3DObjectBase
    {
        public AeroParticle3D(float x, float y, float z, float mass, float restitution = 0.5F, float friction = 0.5F) 
            : base(mass, restitution, friction)
        {
            Position = new AeroVec3(x, y, z);
            PreviousPosition = new AeroVec3(x, y, z);
        }
    }
}
