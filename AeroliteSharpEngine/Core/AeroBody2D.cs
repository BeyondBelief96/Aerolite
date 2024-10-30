using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core
{
    public class AeroBody2D : Physics2DObject2DBase, IBody2D
    {
        public float Restitution { get; set; }
        public float Friction { get; set; }
        public float Angle { get; set; }
        public float PreviousAngle { get; set; }
        public float AngularVelocity { get; set; }
        public float AngularAcceleration { get; set; }
        public float NetTorque { get; set; }
        public float Inertia { get; private set; }
        public float InverseInertia { get; private set; }

        public AeroBody2D(float x, float y, float mass, AeroShape2D shape) : base(mass, shape)
        {
            Position = new AeroVec2(x, y);
            PreviousPosition = new AeroVec2(x, y);

            // Initialize rotation properties
            Inertia = shape.GetMomentOfInertia(mass);
            if (Inertia != 0.0f)
            {
                InverseInertia = 1.0f / Inertia;
            }

            // Set defaults
            Restitution = 0.5f;
            Friction = 0.2f;
        }

        public void ApplyTorque(float torque)
        {
            if (IsStatic) return;
            NetTorque += torque;
        }

        public void ClearTorque()
        {
            if (IsStatic) return;
            NetTorque = 0.0f;
        }
    }
}
