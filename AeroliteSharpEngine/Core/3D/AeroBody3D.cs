using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Core._3D
{
    public class AeroBody3D : Physics3DObjectBase, IBody3D
    {
        #region Properties

        public AeroVec3 AngularVelocity { get; set;  }
        public AeroQuaternion Orientation { get; set; }
        public AeroMat3x4 TransformMatrix { get; set; }
        public AeroVec3 AngularAcceleration { get; set; }
        public AeroVec3 AngularMomentum { get; set; }

        #endregion

        #region Constructors

        public AeroBody3D(float mass, float restitution = 0.5F, float friction = 0.5F) : base(mass, restitution, friction)
        {
        }

        #endregion

        #region Methods

        public void CalculateDerivedQuantities()
        {
            TransformMatrix = AeroMat3x4.FromQuaternionAndPosition(Orientation, Position);
        }

        #endregion

    }
}
