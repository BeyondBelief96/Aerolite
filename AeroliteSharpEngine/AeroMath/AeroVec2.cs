using System.Runtime.CompilerServices;

namespace AeroliteSharpEngine.AeroMath
{
    /// <summary>
    /// Represents a 2-Dimensional vector in cartesian coordinates.
    /// </summary>
    public struct AeroVec2 : IEquatable<AeroVec2>
    {
        #region Properties
        
        /// <summary>
        /// The X coordinate of the vector.
        /// </summary>
        public float X { get; set; }
        
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public float Y { get; set; }
        
        /// <summary>
        /// The length of the vector.
        /// </summary>
        public float Magnitude => MathF.Sqrt(X * X + Y * Y);
        
        /// <summary>
        /// The squared length of the vector.
        /// </summary>
        public float MagnitudeSquared => X * X + Y * Y;
        
        /// <summary>
        /// Returns a zero vector.
        /// </summary>
        public static AeroVec2 Zero => new AeroVec2(0, 0);

        #endregion

        #region Constructor

        public AeroVec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec2 Add(AeroVec2 v)
        {
            X += v.X;
            Y += v.Y;
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeroVec2 Add(AeroVec2 v1, AeroVec2 v2)
        {
            return new AeroVec2(v1.X + v2.X, v1.Y + v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec2 Subtract(AeroVec2 v)
        {
            X -= v.X;
            Y -= v.Y;
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeroVec2 Subtract(AeroVec2 v1, AeroVec2 v2)
        {
            return new AeroVec2(v1.X - v2.X, v1.Y - v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec2 Scale(float s)
        {
            X *= s;
            Y *= s;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float magnitude = Magnitude;
            if (magnitude > float.Epsilon) // Use epsilon instead of just 0
            {
                X /= magnitude;
                Y /= magnitude;
            }
            else
            {
                X = 0;
                Y = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec2 UnitVector()
        {
            float magnitude = Magnitude;
            if (magnitude > float.Epsilon)
            {
                return new AeroVec2(X / magnitude, Y / magnitude);
            }
            return new AeroVec2(0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec2 Normal() => new AeroVec2(-Y, X).UnitVector();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(AeroVec2 v) => X * v.X + Y * v.Y;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(AeroVec2 v1, AeroVec2 v2) => v1.X * v2.X + v1.Y * v2.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Cross(AeroVec2 v) => X * v.Y - Y * v.X;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(AeroVec2 v1, AeroVec2 v2) => v1.Y * v2.X - v1.X * v2.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceTo(AeroVec2 v) => (v - this).Magnitude;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(AeroVec2 v1, AeroVec2 v2) => (v1 - v2).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec2 Rotate(float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);
            return new AeroVec2(
                X * cos - Y * sin,
                X * sin + Y * cos);
        }
        
        public readonly bool Equals(AeroVec2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        #endregion

        #region Operator Overloads

        public static AeroVec2 operator +(AeroVec2 a, AeroVec2 b) => new AeroVec2(a.X + b.X, a.Y + b.Y);
        public static AeroVec2 operator -(AeroVec2 a, AeroVec2 b) => new AeroVec2(a.X - b.X, a.Y - b.Y);
        public static AeroVec2 operator *(AeroVec2 v, float s) => new AeroVec2(v.X * s, v.Y * s);
        public static AeroVec2 operator *(float s, AeroVec2 v) => new AeroVec2(v.X * s, v.Y * s);
        public static AeroVec2 operator *(AeroVec2 v, int i) => new AeroVec2(v.X * i, v.Y * i);
        public static AeroVec2 operator *( int i, AeroVec2 v) => new AeroVec2(v.X * i, v.Y * i);
        public static AeroVec2 operator /(AeroVec2 v, float s) => new AeroVec2(v.X / s, v.Y / s);
        public static AeroVec2 operator -(AeroVec2 v) => new AeroVec2(-v.X, -v.Y);
        public static bool operator ==(AeroVec2 lhs, AeroVec2 rhs) => lhs.Equals(rhs);
        public static bool operator !=(AeroVec2 lhs, AeroVec2 rhs) => !lhs.Equals(rhs);

        public readonly override bool Equals(object? obj)
        {
            if(obj is AeroVec2 other)
            {
                return Equals((AeroVec2)other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        #endregion
    }
}
