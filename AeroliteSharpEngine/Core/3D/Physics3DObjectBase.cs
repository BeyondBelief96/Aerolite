using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using System;

namespace AeroliteSharpEngine.Core._3D
{
    /// <summary>
    /// Base class that implements the ID system and common IPhysicsObject3D properties
    /// </summary>
    public abstract class Physics3DObjectBase : IPhysicsObject3D
    {
        #region Fields

        private static int _nextId;
        private static readonly object IdLock = new object();

        #endregion  

        #region Properties

        /// <summary>
        /// Unique identifier for this physics object.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Damping constant to help mitigate numerical errors.
        /// </summary>
        public float Damping { get; set; }

        private float _restitution;
        /// <summary>
        /// Represents the coefficient of restitution of the object. Ranges between 0 - 1.
        /// </summary>
        public float Restitution
        {
            get => _restitution;
            set => _restitution = AeroMathExtensions.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Represents the coefficient of friction of the object.
        /// </summary>
        public float StaticFriction { get; set; }
        /// <summary>
        /// The position of the object's center of mass.
        /// </summary>
        public AeroVec3 Position { get; set; }
        /// <summary>
        /// Stores the previous position of the object. Used for Verlet integration.
        /// </summary>
        public AeroVec3 PreviousPosition { get; set; }
        /// <summary>
        /// The linear velocity of the object's center of mass.
        /// </summary>
        public AeroVec3 Velocity { get; set; }
        /// <summary>
        /// The linear acceleration of the object's center of mass.
        /// </summary>
        public AeroVec3 Acceleration { get; set; }
        /// <summary>
        /// The net force acting on this object's center of mass.
        /// </summary>
        public AeroVec3 NetForce { get; set; }
        /// <summary>
        /// The mass of the object, concentrated at its center.
        /// </summary>
        public float Mass { get; protected set; }
        /// <summary>
        /// 1 / Mass. Cached to speed up calculations.
        /// </summary>
        public float InverseMass { get; protected set; }
        /// <summary>
        /// Returns whether the object is considered static. Static objects are created by setting the mass to zero exactly.
        /// Static objects do not participate in integration and collision resolution.
        /// </summary>
        public bool IsStatic { get; private set; }
        /// <summary>
        /// Returns whether this object has a non-infinite mass (0.0 is infinite).
        /// </summary>
        public bool HasFiniteMass => !AeroMathExtensions.AreEqual(InverseMass, 0.0f);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Physics3DObjectBase"/> class with the specified mass, restitution, and friction.
        /// </summary>
        /// <param name="mass">The mass of the object. If zero, the object is static.</param>
        /// <param name="restitution">The coefficient of restitution (default 0.5).</param>
        /// <param name="friction">The coefficient of friction (default 0.5).</param>
        protected Physics3DObjectBase(float mass, float restitution = 0.5f, float friction = 0.5f)
        {
            // Generate unique ID
            lock (IdLock)
            {
                Id = ++_nextId;
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

            Position = new AeroVec3();
            PreviousPosition = new AeroVec3();
            Velocity = new AeroVec3();
            Acceleration = new AeroVec3();
            NetForce = new AeroVec3();
            Damping = 0.99f;
            Restitution = restitution;
            StaticFriction = friction;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the supplied force to the object's current net force.
        /// </summary>
        /// <param name="force">The force vector to apply.</param>
        public virtual void ApplyForce(AeroVec3 force)
        {
            if (IsStatic) return;
            NetForce += force;
        }

        /// <summary>
        /// Removes all forces from the object.
        /// </summary>
        public virtual void ClearForces()
        {
            NetForce = new AeroVec3();
        }

        /// <summary>
        /// Applies an impulse to the object, changing its velocity instantaneously.
        /// </summary>
        /// <param name="j">The impulse vector.</param>
        public virtual void ApplyImpulse(AeroVec3 j)
        {
            if (IsStatic) return;
            Velocity += j * InverseMass;
        }

        /// <summary>
        /// Updates the underlying shape geometry that needs to be transformed based on the current simulation state.
        /// Base implementation does nothing; override in derived classes if needed.
        /// </summary>
        public virtual void UpdateGeometry()
        {
            // Base implementation does nothing; override in derived classes if needed
        }

        /// <summary>
        /// Returns the hash code for this physics object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether this object is equal to another physics object by comparing their IDs.
        /// </summary>
        /// <param name="other">The other physics object.</param>
        /// <returns>True if the objects have the same ID; otherwise, false.</returns>
        public bool Equals(IPhysicsObject3D? other)
        {
            return other != null && Id == other.Id;
        }

        /// <summary>
        /// Determines whether this object is equal to the specified object by comparing their IDs.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is a <see cref="Physics3DObjectBase"/> and has the same ID; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Physics3DObjectBase other)
            {
                return Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// Determines whether this object is equal to another <see cref="Physics3DObjectBase"/> by comparing their IDs.
        /// </summary>
        /// <param name="other">The other physics object.</param>
        /// <returns>True if the objects have the same ID; otherwise, false.</returns>
        protected bool Equals(Physics3DObjectBase other)
        {
            return Id == other.Id;
        }

        #endregion
    }
}
