using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine
{
    public class AeroBody2D : IBody2D
    {
        #region Properties

        /// <summary>
        /// Linear Position of the center of mass of the body.
        /// </summary>
        public AeroVec2 Position { get; set; }

        /// <summary>
        /// Linear velocity of the center of mass of the body.
        /// </summary>
        public AeroVec2 Velocity { get; set; }

        /// <summary>
        /// Linear acceleration of the center of mass of the body.
        /// </summary>
        public AeroVec2 Acceleration { get; set; }

        /// <summary>
        /// Sum of net forces on the center of mass of the body.
        /// </summary>
        public AeroVec2 NetForces { get; set; }

        /// <summary>
        /// The mass of the body.
        /// </summary>
        public float Mass { get; private set; }

        /// <summary>
        /// 1 / Mass. Caching the inverse mass is useful as division of floating point numbers is slow. 
        /// </summary>
        public float InverseMass { get; private set; }

        /// <summary>
        /// Used to set the body as static or not. Static bodies do not have their integration steps run and don't respond to collisions.
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Thec cofficient of restitution of the body. 
        /// </summary>
        public float Restitution { get; set; }

        /// <summary>
        /// The coefficient of friction of the body.
        /// </summary>
        public float Friction { get; set; }

        /// <summary>
        /// Angle of the body with respect to horizontal axis.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Angular velocity of the body. In 2D we represent this as a float intead of a vector.
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// Angular acceleration of the body. In 2D we represent this as a float instead of a vector.
        /// </summary>
        public float AngularAcceleration { get; set; }

        /// <summary>
        /// The moment of inertia of the body.
        /// </summary>
        public float Inertia { get; set; }

        /// <summary>
        /// 1 / Inertia. Caching this speeds up calculations.
        /// </summary>
        public float InverseInertia { get; set; }

        /// <summary>
        /// The shape of the body.
        /// </summary>
        public AeroShape Shape { get; private set; }

        /// <summary>
        /// Returns whether the bodies mass is not 0.0 (meaning infinite mass) or not. 
        /// </summary>
        public bool HasFiniteMass => InverseMass != 0.0;

        #endregion

        #region Constructor

        public AeroBody2D(float x, float y, float mass, AeroShape shape)
        {
            Position = new AeroVec2(x, y);
            Velocity = new AeroVec2();
            Acceleration = new AeroVec2();
            NetForces = new AeroVec2();
            Mass = mass;
            if(Mass != 0.0f)
            {
                InverseMass = 1.0f / Mass;
            }
            else
            {
                InverseMass = 0.0f;
            }

            Shape = shape;
        }

        #endregion

        #region Public Methods

        public void ApplyForce(AeroVec2 force)
        {
            NetForces += force;
        }

        public void ApplyTorque(float torque)
        {
            AngularAcceleration += torque * InverseInertia;
        }

        public void Integrate(float dt)
        {
            if (IsStatic) return;

            Acceleration = NetForces * InverseMass;
            Velocity += Acceleration * dt;
            Position += Velocity * dt + Acceleration * dt * dt / 2.0f;

            AngularAcceleration = AngularAcceleration * InverseInertia;
            AngularVelocity += AngularVelocity * dt;
            Angle += AngularVelocity * dt;

            NetForces = new AeroVec2();
            AngularAcceleration = 0.0f;
        }

        #endregion
    }
}
