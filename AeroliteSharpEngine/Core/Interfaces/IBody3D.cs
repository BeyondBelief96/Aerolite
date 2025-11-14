using AeroliteSharpEngine.AeroMath;

namespace AeroliteSharpEngine.Core.Interfaces
{
    public interface IBody3D : IPhysicsObject3D
    {
        public AeroVec3 AngularVelocity { get; set; }

        public AeroVec3 AngularAcceleration { get; set; }

        public AeroVec3 AngularMomentum { get; set; }

        public AeroQuaternion Orientation { get; set; }

        public AeroMat3x4 TransformMatrix { get; set; }
    }
}
