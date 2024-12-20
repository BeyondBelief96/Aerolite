﻿using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Core
{
    /// <summary>
    /// Base class that implements the ID system and common IPhysicsObject properties
    /// </summary>
    public abstract class Physics2DObject2DBase : IPhysicsObject2D
    {
        #region Fields
        
        private static int _nextId;
        private static readonly object IdLock = new object();
        
        #endregion
        
        #region Properties
        
        public int Id { get; }
        public float Damping { get; set; }

        private float _restitution;
        public float Restitution
        {
            get => _restitution;
            set => _restitution = AeroMathExtensions.Clamp(value, 0, 1);
        }

        public float StaticFriction { get; set; }
        public AeroVec2 Position { get; set; }
        public AeroVec2 PreviousPosition { get; set; }
        public AeroVec2 Velocity { get; set; }
        public AeroVec2 Acceleration { get; set; }
        public AeroVec2 NetForce { get; set; }
        public float Mass { get; protected set; }
        public float InverseMass { get; protected set; }
        public bool IsStatic { get; private set; }
        public bool HasFiniteMass => !AeroMathExtensions.AreEqual(InverseMass, 0.0f);
        public AeroShape2D Shape { get; private set; }
        
        #endregion

        protected Physics2DObject2DBase(float mass, AeroShape2D shape, float restitution = 0.5f, float friction = 0.5f)
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

            Position = new AeroVec2();
            PreviousPosition = new AeroVec2();
            Velocity = new AeroVec2();
            Acceleration = new AeroVec2();
            NetForce = new AeroVec2();
            Damping = 0.99f;
            Restitution = restitution;
            StaticFriction = friction;
            Shape = shape;
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

        public virtual void ApplyImpulse(AeroVec2 j)
        {
            if (IsStatic) return;
            Velocity += j * InverseMass;
        }
        
        public virtual void UpdateGeometry()
        {
            // Base implementation does nothing since not all physics objects need geometry updates e.g. particles/circles
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(IPhysicsObject2D? other)
        {
            return other != null && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Physics2DObject2DBase other)
            {
                return Id == other.Id;
            }
            return false;
        }

        protected bool Equals(Physics2DObject2DBase other)
        {
            return Id == other.Id;
        }
    }
}