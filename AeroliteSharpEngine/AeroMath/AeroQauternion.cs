namespace AeroliteSharpEngine.AeroMath
{
    /// <summary>
    /// Represents a quaternion for 3D rotations and orientation calculations.
    /// Provides methods for normalization, rotation, and vector operations.
    /// </summary>
    public struct AeroQuaternion
    {
        #region Properties

        /// <summary>
        /// Gets the real (scalar) part of the quaternion.
        /// </summary>
        public float R { get; private set; }

        /// <summary>
        /// Gets the first imaginary component (i) of the quaternion.
        /// </summary>
        public float I { get; private set; }

        /// <summary>
        /// Gets the second imaginary component (j) of the quaternion.
        /// </summary>
        public float J { get; private set; }

        /// <summary>
        /// Gets the third imaginary component (k) of the quaternion.
        /// </summary>
        public float K { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroQuaternion"/> struct with the specified components.
        /// </summary>
        /// <param name="r">The real (scalar) part.</param>
        /// <param name="i">The first imaginary component.</param>
        /// <param name="j">The second imaginary component.</param>
        /// <param name="k">The third imaginary component.</param>
        public AeroQuaternion(float r, float i, float j, float k)
        {
            R = r;
            I = i;
            J = j;
            K = k;
        }

        public AeroQuaternion(float angle, AeroVec3 axis)
        {
            float halfAngle = angle * 0.5f;
            float sinHalfAngle = (float)Math.Sin(halfAngle);
            R = (float)Math.Cos(halfAngle);
            I = axis.X * sinHalfAngle;
            J = axis.Y * sinHalfAngle;
            K = axis.Z * sinHalfAngle;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalizes the quaternion to unit length.
        /// If the magnitude is nearly zero, resets to the identity quaternion.
        /// </summary>
        public void Normalize()
        {
            float magnitude = (float)Math.Sqrt(I * I + J * J + K * K + R * R);
            if (!AeroMathExtensions.IsNearlyZero(magnitude))
            {
                float d = 1.0f / magnitude;
                R *= d;
                I *= d;
                J *= d;
                K *= d;

            }
            else
            {
                // Handle zero magnitude case (if needed)
                R = 1.0f;
                I = 0.0f;
                J = 0.0f;
                K = 0.0f;
            }
        }

        /// <summary>
        /// Rotates this quaternion by a vector, modifying its orientation.
        /// </summary>
        /// <param name="v">The vector to rotate by.</param>
        public void RotateByVector(AeroVec3 v)
        {
            AeroQuaternion quat = new AeroQuaternion(0, v.X, v.Y, v.Z);
            this *= quat;
        }

        /// <summary>
        /// Adds a scaled vector to this quaternion, modifying its orientation.
        /// </summary>
        /// <param name="v">The vector to add.</param>
        /// <param name="scale">The scale factor for the vector.</param>
        public void AddScaledVector(AeroVec3 v, float scale)
        {
            AeroQuaternion q = new AeroQuaternion(0, v.X * scale, v.Y * scale, v.Z * scale);
            q *= this;
            R += q.R * 0.5f;
            I += q.I * 0.5f;
            J += q.J * 0.5f;
            K += q.K * 0.5f;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Multiplies two quaternions, combining their rotations.
        /// </summary>
        /// <param name="q1">The first quaternion.</param>
        /// <param name="q2">The second quaternion.</param>
        /// <returns>The product of the two quaternions.</returns>
        public static AeroQuaternion operator *(AeroQuaternion q1, AeroQuaternion q2)
        {
            return new AeroQuaternion(
                q1.R * q2.R - q1.I * q2.I - q1.J * q2.J - q1.K * q2.K,
                q1.R * q2.I + q1.I * q2.R + q1.J * q2.K - q1.K * q2.J,
                q1.R * q2.J + q1.J * q2.R + q1.K * q2.I - q1.I * q2.K,
                q1.R * q2.K + q1.K * q2.R + q1.I * q2.J - q1.J * q2.I
            );
        }

        /// <summary>
        /// Multiplies this quaternion by another quaternion and assigns the result to this instance.
        /// </summary>
        /// <param name="other">The quaternion to multiply by.</param>
        public void operator *=(AeroQuaternion other)
        {
            this = this * other;
        }

        #endregion
    }
}
