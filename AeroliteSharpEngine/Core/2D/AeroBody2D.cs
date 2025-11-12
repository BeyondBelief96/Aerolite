using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core
{
    public class AeroBody2D : Physics2DObject2DBase, IBody2D
    {
        public float Angle { get; set; }
        public float PreviousAngle { get; set; }
        public float AngularVelocity { get; set; }
        public float AngularAcceleration { get; set; }
        public float NetTorque { get; set; }
        public float Inertia { get; private set; }
        public float InverseInertia { get; private set; }

        public AeroBody2D(float x, float y, float mass, AeroShape2D shape, 
            float restitution = 0.5f, float friction = 0.5f) : base(mass, shape, restitution, friction)
        {
            Position = new AeroVec2(x, y);
            PreviousPosition = new AeroVec2(x, y);

            // Initialize rotation properties
            Inertia = shape.GetMomentOfInertia(mass);
            if (Inertia != 0.0f)
            {
                InverseInertia = 1.0f / Inertia;
            }
        }

        public void ApplyTorque(float torque)
        {
            if (IsStatic) return;
            NetTorque += torque;
        }

        public void ApplyImpulseAtPoint(AeroVec2 j, AeroVec2 r)
        {
            if(IsStatic) return;

            Velocity += j * InverseMass;
            AngularVelocity += r.Cross(j) * InverseInertia;
        }

        public void ClearTorque()
        {
            if (IsStatic) return;
            NetTorque = 0.0f;
        }

        public override void UpdateGeometry()
        {
            if (Shape is AeroPolygon polygon)
            {
                Shape.UpdateVertices(Angle, Position);
            }
        }
    }
}
