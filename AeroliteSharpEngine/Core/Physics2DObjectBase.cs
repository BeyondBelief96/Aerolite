using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Core
{
    /// <summary>
    /// Base class that implements the ID system and common IPhysicsObject properties
    /// </summary>
    public abstract class Physics2DObjectBase : IPhysicsObject
    {
        private static int nextId;
        private static readonly object idLock = new object();

        /// <summary>
        /// Unique identifier for this physics object
        /// </summary>
        public int Id { get; }
        public float Damping { get; set; }
        public AeroVec2 Position { get; set; }
        public AeroVec2 PreviousPosition { get; set; }
        public AeroVec2 Velocity { get; set; }
        public AeroVec2 Acceleration { get; set; }
        public AeroVec2 NetForce { get; set; }
        public float Mass { get; protected set; }
        public float InverseMass { get; protected set; }
        public bool IsStatic { get; set; }
        public bool HasFiniteMass => InverseMass != 0.0f;

        protected Physics2DObjectBase(float mass)
        {
            // Generate unique ID
            lock (idLock)
            {
                Id = ++nextId;
            }

            // Initialize physics properties
            Mass = mass;
            if (Mass != 0.0f)
            {
                InverseMass = 1.0f / mass;
            }
            else
            {
                InverseMass = 0.0f;
                IsStatic = true;
            }

            Position = new AeroVec2();
            PreviousPosition = new AeroVec2();
            Velocity = new AeroVec2();
            Acceleration = new AeroVec2();
            NetForce = new AeroVec2();
            Damping = 0.99f;
        }

        public virtual void ApplyForce(AeroVec2 force)
        {
            if (IsStatic) return;
            NetForce += force;
        }

        public virtual void ClearForces()
        {
            NetForce = new AeroVec2();
        }

        // Override to provide custom cleanup if needed
        public virtual void Dispose()
        {
            // Cleanup code here if needed
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is Physics2DObjectBase other)
            {
                return Id == other.Id;
            }
            return false;
        }
    }
}