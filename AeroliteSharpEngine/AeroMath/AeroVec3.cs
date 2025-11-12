using System.Runtime.CompilerServices;

namespace AeroliteSharpEngine.AeroMath
{
    /// <summary>
    /// Represents a three-dimensional vector with single-precision floating-point components.
    /// Provides common vector operations and properties.
    /// </summary>
    public struct AeroVec3 : IEquatable<AeroVec3>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the Z component of the vector.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Gets the magnitude (length) of the vector.
        /// </summary>
        public float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        /// Gets the squared magnitude of the vector.
        /// </summary>
        public float MagnitudeSquared => X * X + Y * Y + Z * Z;

        /// <summary>
        /// Returns a vector with all components set to zero (0, 0, 0).
        /// </summary>
        public static AeroVec3 Zero => new(0, 0, 0);

        /// <summary>
        /// Returns a vector with all components set to one (1, 1, 1).
        /// </summary>
        public static AeroVec3 One => new(1, 1, 1);

        /// <summary>
        /// Returns the unit vector pointing right (1, 0, 0).
        /// </summary>
        public static AeroVec3 Right => new(1, 0, 0);

        /// <summary>
        /// Returns the unit vector pointing left (-1, 0, 0).
        /// </summary>
        public static AeroVec3 Left => new(-1, 0, 0);

        /// <summary>
        /// Returns the unit vector pointing up (0, 1, 0).
        /// </summary>
        public static AeroVec3 Up => new(0, 1, 0);

        /// <summary>
        /// Returns the unit vector pointing down (0, -1, 0).
        /// </summary>
        public static AeroVec3 Down => new(0, -1, 0);

        /// <summary>
        /// Returns the unit vector pointing forward (0, 0, 1).
        /// </summary>
        public static AeroVec3 Forward => new(0, 0, 1);

        /// <summary>
        /// Returns the unit vector pointing backward (0, 0, -1).
        /// </summary>
        public static AeroVec3 Back => new(0, 0, -1);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroVec3"/> struct with the specified components.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public AeroVec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified vector to this vector component-wise.
        /// </summary>
        /// <param name="v">The vector to add.</param>
        /// <returns>The resulting vector after addition.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec3 Add(AeroVec3 v)
        {
            X += v.X;
            Y += v.Y;
            Z += v.Z;
            return this;    
        }

        /// <summary>
        /// Adds two vectors component-wise.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The sum of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeroVec3 Add(AeroVec3 a, AeroVec3 b)
        {
            return new AeroVec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts the specified vector from this vector component-wise.
        /// </summary>
        /// <param name="v">The vector to subtract.</param>
        /// <returns>The resulting vector after subtraction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec3 Subtract(AeroVec3 v)
        {
            X -= v.X;
            Y -= v.Y;
            Z -= v.Z;
            return this;
        }

        /// <summary>
        /// Subtracts one vector from another component-wise.
        /// </summary>
        /// <param name="a">The vector to subtract from.</param>
        /// <param name="b">The vector to subtract.</param>
        /// <returns>The difference between the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeroVec3 Subtract(AeroVec3 a, AeroVec3 b)
        {
            return new AeroVec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Scales this vector by the specified scalar.
        /// </summary>
        /// <param name="s">The scalar value.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec3 Scale(float s)
        {
            X *= s;
            Y *= s;
            Z *= s;
            return this;
        }

        /// <summary>
        /// Normalizes this vector to unit length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            if(AeroMathExtensions.IsNearlyZero(Magnitude))
            {
                X = float.Epsilon;
                Y = float.Epsilon;
            }
            else
            {
                float mag = Magnitude;
                X /= mag;
                Y /= mag;
                Z /= mag;
            }
        }

        /// <summary>
        /// Returns the unit vector in the same direction as this vector.
        /// </summary>
        /// <returns>The normalized unit vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AeroVec3 UnitVector()
        {
            return AeroMathExtensions.IsNearlyZero(Magnitude) ? new AeroVec3(float.Epsilon, float.Epsilon, float.Epsilon) : new AeroVec3(X / Magnitude, Y / Magnitude, Z / Magnitude);
        }

        /// <summary>
        /// Computes the dot product of this vector and the specified vector.
        /// </summary>
        /// <param name="v">The other vector.</param>
        /// <returns>The dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(AeroVec3 v) => X * v.X + Y * v.Y + Z * v.Z;

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(AeroVec3 v1, AeroVec3 v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;

        /// <summary>
        /// Computes the 2D cross product of this vector and the specified vector (ignoring Z).
        /// </summary>
        /// <param name="v">The other vector.</param>
        /// <returns>The cross product (scalar).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Cross(AeroVec3 v) => X * v.Y - Y * v.X;

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The cross product vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AeroVec3 Cross(AeroVec3 v1, AeroVec3 v2)
        {
            return new AeroVec3(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        /*
        /// <summary>
        /// Computes the distance from this vector to the specified vector.
        /// </summary>
        /// <param name="v">The other vector.</param>
        /// <returns>The distance between the vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceTo(AeroVec3 v) => (v - this).Magnitude;
        */

        /// <summary>
        /// Computes the distance between two vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The distance between the vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(AeroVec3 v1, AeroVec3 v2) => (v1 - v2).Magnitude;

        /// <summary>
        /// Determines whether this vector is equal to another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>True if the vectors are equal; otherwise, false.</returns>
        public bool Equals(AeroVec3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Adds two vectors component-wise.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static AeroVec3 operator +(AeroVec3 a, AeroVec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>
        /// Subtracts one vector from another component-wise.
        /// </summary>
        /// <param name="a">The vector to subtract from.</param>
        /// <param name="b">The vector to subtract.</param>
        /// <returns>The difference between the two vectors.</returns>
        public static AeroVec3 operator -(AeroVec3 a, AeroVec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <param name="scalar">The scalar multiplier.</param>
        /// <returns>The scaled vector.</returns>
        public static AeroVec3 operator *(AeroVec3 a, float scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

        /// <summary>
        /// Multiplies a scalar by a vector.
        /// </summary>
        /// <param name="scalar">The scalar multiplier.</param>
        /// <param name="a">The vector.</param>
        /// <returns>The scaled vector.</returns>
        public static AeroVec3 operator *(float scalar, AeroVec3 a) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

        /// <summary>
        /// Applies the dot product component-wise between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A scalar value representing the dot product between the two vectors.</returns>
        public static float operator *(AeroVec3 a, AeroVec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <param name="scalar">The scalar divisor.</param>
        /// <returns>The scaled vector.</returns>
        public static AeroVec3 operator /(AeroVec3 a, float scalar) => new(a.X / scalar, a.Y / scalar, a.Z / scalar);

        /// <summary>
        /// Divides a scalar by each component of a vector.
        /// </summary>
        /// <param name="scalar">The scalar dividend.</param>
        /// <param name="a">The vector divisor.</param>
        /// <returns>A vector with scalar divided by each component.</returns>
        public static AeroVec3 operator /(float scalar, AeroVec3 a) => new(scalar / a.X, scalar / a.Y, scalar / a.Z);

        /// <summary>
        /// Negates a vector (inverts all components).
        /// </summary>
        /// <param name="a">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static AeroVec3 operator -(AeroVec3 a) => new(-a.X, -a.Y, -a.Z);

        /// <summary>
        /// Determines whether two vectors are approximately equal within epsilon tolerance.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>True if all components are within epsilon of each other; otherwise, false.</returns>
        public static bool operator ==(AeroVec3 a, AeroVec3 b) =>
            System.Math.Abs(a.X - b.X) < float.Epsilon &&
            System.Math.Abs(a.Y - b.Y) < float.Epsilon &&
            System.Math.Abs(a.Z - b.Z) < float.Epsilon;

        /// <summary>
        /// Determines whether two vectors are not approximately equal within epsilon tolerance.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>True if any component differs by more than epsilon; otherwise, false.</returns>
        public static bool operator !=(AeroVec3 a, AeroVec3 b) => !(a == b);

        /// <summary>
        /// Determines whether this vector is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is an <see cref="AeroVec3"/> and is equal; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is AeroVec3 && Equals((AeroVec3)obj);
        }

        /// <summary>
        /// Returns the hash code for this vector.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        #endregion
    }
}
    