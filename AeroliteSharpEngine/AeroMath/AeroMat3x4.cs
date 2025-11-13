namespace AeroliteSharpEngine.AeroMath
{
    /// <summary>
    /// Represents a 3x4 transformation matrix with single-precision floating-point elements.
    /// This matrix type is commonly used for 3D transformations including rotation and translation.
    /// The 3x4 format is memory-efficient for affine transformations (no perspective).
    /// </summary>
    public struct AeroMat3x4
    {

        #region Properties

        /// <summary>
        /// Gets the underlying data array of the matrix in row-major order.
        /// The matrix is stored as 12 elements representing a 3x4 matrix.
        /// </summary>
        public readonly float[] Data { get; init; }

        /// <summary>
        /// Gets the number of rows in the matrix (always 3).
        /// </summary>
        public readonly int Rows => 3;

        /// <summary>
        /// Gets the number of columns in the matrix (always 4).
        /// </summary>
        public readonly int Columns => 4;

        /// <summary>
        /// Gets the determinant of the 3x3 rotational component of the matrix.
        /// Note: This calculates the determinant of the upper-left 3x3 submatrix,
        /// ignoring the translation column.
        /// </summary>
        public readonly float Determinant
        {
            get
            {
                return Data[8] * Data[5] * Data[2] +
                Data[4] * Data[9] * Data[2] +
                Data[8] * Data[1] * Data[6] -
                Data[0] * Data[9] * Data[6] -
                Data[4] * Data[1] * Data[10] +
                Data[0] * Data[5] * Data[10];
            }
        }

        /// <summary>
        /// Gets the identity matrix (3x3 identity with zero translation).
        /// The identity matrix leaves vectors unchanged when used for transformation.
        /// </summary>
        public static AeroMat3x4 Identity => new AeroMat3x4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f
        );

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroMat3x4"/> struct from a float array.
        /// </summary>
        /// <param name="data">A float array of length 12 representing the matrix elements in row-major order.</param>
        /// <exception cref="ArgumentException">Thrown if the array is null or not of length 12.</exception>
        public AeroMat3x4(float[] data)
        {
            if (data == null || data.Length != 12)
                throw new ArgumentException("Data must be a float array of length 12.");
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroMat3x4"/> struct from individual elements.
        /// </summary>
        /// <param name="m00">Element at row 0, column 0.</param>
        /// <param name="m01">Element at row 0, column 1.</param>
        /// <param name="m02">Element at row 0, column 2.</param>
        /// <param name="m03">Element at row 0, column 3 (translation X).</param>
        /// <param name="m10">Element at row 1, column 0.</param>
        /// <param name="m11">Element at row 1, column 1.</param>
        /// <param name="m12">Element at row 1, column 2.</param>
        /// <param name="m13">Element at row 1, column 3 (translation Y).</param>
        /// <param name="m20">Element at row 2, column 0.</param>
        /// <param name="m21">Element at row 2, column 1.</param>
        /// <param name="m22">Element at row 2, column 2.</param>
        /// <param name="m23">Element at row 2, column 3 (translation Z).</param>
        public AeroMat3x4(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23)
        {
            Data = new float[12]
            {
                m00, m01, m02, m03,
                m10, m11, m12, m13,
                m20, m21, m22, m23
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms a vector by this matrix (applies rotation and translation).
        /// </summary>
        /// <param name="v">The vector to transform.</param>
        /// <returns>The transformed vector.</returns>
        public AeroVec3 Transform(AeroVec3 v)
        {
            return this * v;
        }

        /// <summary>
        /// Multiplies this matrix by another matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply by.</param>
        /// <returns>The product matrix.</returns>
        public AeroMat3x4 Multiply(AeroMat3x4 other)
        {
            return this * other;
        }

        /// <summary>
        /// Returns the inverse of this matrix.
        /// </summary>
        /// <returns>The inverse matrix.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        public AeroMat3x4 Inverse()
        {
            AeroMat3x4 result = new AeroMat3x4();
            result.SetInverse(this);
            return result;
        }

        /// <summary>
        /// Inverts this matrix in place.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        public void Invert()
        {
            SetInverse(this);
        }

        /// <summary>
        /// Creates a transformation matrix from a quaternion rotation and position vector.
        /// </summary>
        /// <param name="q">The quaternion representing the rotation.</param>
        /// <param name="pos">The position/translation vector.</param>
        /// <returns>The transformation matrix.</returns>
        public static AeroMat3x4 FromQuaternionAndPosition(AeroQuaternion q, AeroVec3 pos)
        {
            AeroMat3x4 result = new AeroMat3x4();
            result.SetOrientationAndPos(q, pos);
            return result;
        }

        /// <summary>
        /// Sets the orientation and position of this matrix from a quaternion and position vector.
        /// </summary>
        /// <param name="q">The quaternion representing the rotation.</param>
        /// <param name="pos">The position/translation vector.</param>
        public void SetOrientationFromQuaternionAndPos(AeroQuaternion q, AeroVec3 pos)
        {
            SetOrientationAndPos(q, pos);
        }

        /// <summary>
        /// Transforms a vector by the inverse of this matrix.
        /// This is equivalent to transforming from world space to local space.
        /// </summary>
        /// <param name="v">The vector to transform.</param>
        /// <returns>The inverse-transformed vector.</returns>
        public AeroVec3 TransformInverse(AeroVec3 v)
        {
            AeroVec3 result = new AeroVec3();
            AeroVec3 tmp = v;
            tmp.X -= Data[3];
            tmp.Y -= Data[7];
            tmp.Z -= Data[11];
            result.X = tmp.X * Data[0] + tmp.Y * Data[4] + tmp.Z * Data[8];
            result.Y = tmp.X * Data[1] + tmp.Y * Data[5] + tmp.Z * Data[9];
            result.Z = tmp.X * Data[2] + tmp.Y * Data[6] + tmp.Z * Data[10];
            return result;
        }

        /// <summary>
        /// Transforms a direction vector by this matrix (applies rotation only, ignores translation).
        /// </summary>
        /// <param name="dir">The direction vector to transform.</param>
        /// <returns>The transformed direction vector.</returns>
        public AeroVec3 TransformDirection(AeroVec3 dir)
        {
            return new AeroVec3(
                dir.X * Data[0] + dir.Y * Data[1] + dir.Z * Data[2],
                dir.X * Data[4] + dir.Y * Data[5] + dir.Z * Data[6],
                dir.X * Data[8] + dir.Y * Data[9] + dir.Z * Data[10]
            );
        }

        /// <summary>
        /// Transforms a direction vector by the inverse of this matrix (applies inverse rotation only).
        /// </summary>
        /// <param name="dir">The direction vector to transform.</param>
        /// <returns>The inverse-transformed direction vector.</returns>
        public AeroVec3 TransformInverseDirection(AeroVec3 dir)
        {
            return new AeroVec3(
                dir.X * Data[0] + dir.Y * Data[4] + dir.Z * Data[8],
                dir.X * Data[1] + dir.Y * Data[5] + dir.Z * Data[9],
                dir.X * Data[2] + dir.Y * Data[6] + dir.Z * Data[10]
            );
        }

        /// <summary>
        /// Transforms a vector from local space to world space using the given world transformation matrix.
        /// </summary>
        /// <param name="local">The vector in local space.</param>
        /// <param name="worldTransform">The world transformation matrix.</param>
        /// <returns>The vector in world space.</returns>
        public static AeroVec3 LocalToWorld(AeroVec3 local, AeroMat3x4 worldTransform)
        {
            return worldTransform.Transform(local);
        }

        /// <summary>
        /// Transforms a vector from world space to local space using the given local transformation matrix.
        /// </summary>
        /// <param name="world">The vector in world space.</param>
        /// <param name="localTransform">The local transformation matrix.</param>
        /// <returns>The vector in local space.</returns>
        public static AeroVec3 WorldToLocal(AeroVec3 world, AeroMat3x4 localTransform)
        {
            return localTransform.TransformInverse(world);
        }

        /// <summary>
        /// Transforms a direction vector from local space to world space using the given world transformation matrix.
        /// </summary>
        /// <param name="localDir">The direction vector in local space.</param>
        /// <param name="worldTransform">The world transformation matrix.</param>
        /// <returns>The direction vector in world space.</returns>
        public static AeroVec3 LocalToWorldDirection(AeroVec3 localDir, AeroMat3x4 worldTransform)
        {
            return worldTransform.TransformDirection(localDir);
        }

        /// <summary>
        /// Transforms a direction vector from world space to local space using the given local transformation matrix.
        /// </summary>
        /// <param name="worldDir">The direction vector in world space.</param>
        /// <param name="localTransform">The local transformation matrix.</param>
        /// <returns>The direction vector in local space.</returns>
        public static AeroVec3 WorldToLocalDirection(AeroVec3 worldDir, AeroMat3x4 localTransform)
        {
            return localTransform.TransformInverseDirection(worldDir);
        }


        /// <summary>
        /// Sets this matrix to the inverse of another matrix.
        /// </summary>
        /// <param name="m">The matrix to invert.</param>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        private void SetInverse(AeroMat3x4 m)
        {
            if (AeroMathExtensions.IsNearlyZero(Determinant))
            {
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            }

            float invDet = 1.0f / Determinant;
            Data[0] = (-m.Data[9] * m.Data[6] + m.Data[5] * m.Data[10]) * invDet;
            Data[4] = (m.Data[8] * m.Data[6] - m.Data[4] * m.Data[10]) * invDet;
            Data[8] = (-m.Data[8] * m.Data[5] + m.Data[4] * m.Data[9]) * invDet;
            Data[1] = (m.Data[9] * m.Data[2] - m.Data[1] * m.Data[10]) * invDet;
            Data[5] = (-m.Data[8] * m.Data[2] + m.Data[0] * m.Data[10]) * invDet;
            Data[9] = (m.Data[8] * m.Data[1] - m.Data[0] * m.Data[9]) * invDet;
            Data[2] = (-m.Data[5] * m.Data[2] + m.Data[1] * m.Data[6]) * invDet;
            Data[6] = (m.Data[4] * m.Data[2] - m.Data[0] * m.Data[6]) * invDet;
            Data[10] = (-m.Data[4] * m.Data[1] + m.Data[0] * m.Data[5]) * invDet;
            Data[3] = (m.Data[9] * m.Data[6] * m.Data[3]
                      - m.Data[5] * m.Data[10] * m.Data[3]
                      - m.Data[9] * m.Data[2] * m.Data[7]
                      + m.Data[1] * m.Data[10] * m.Data[7]
                      + m.Data[5] * m.Data[2] * m.Data[11]
                      - m.Data[1] * m.Data[6] * m.Data[11]) * invDet;
                      Data[7] = (-m.Data[8] * m.Data[6] * m.Data[3]
                      + m.Data[4] * m.Data[10] * m.Data[3]
                      + m.Data[8] * m.Data[2] * m.Data[7]
                      - m.Data[0] * m.Data[10] * m.Data[7]
                      - m.Data[4] * m.Data[2] * m.Data[11]
                      + m.Data[0] * m.Data[6] * m.Data[11]) * invDet;
                      Data[11] = (m.Data[8] * m.Data[5] * m.Data[3]
                      - m.Data[4] * m.Data[9] * m.Data[3]
                      - m.Data[8] * m.Data[1] * m.Data[7]
                      + m.Data[0] * m.Data[9] * m.Data[7]
                      + m.Data[4] * m.Data[1] * m.Data[11]
                      - m.Data[0] * m.Data[5] * m.Data[11]) * invDet;
        }

        /// <summary>
        /// Sets this matrix to represent a transformation defined by a quaternion rotation and position vector.
        /// </summary>
        /// <param name="q">The quaternion representing the rotation.</param>
        /// <param name="pos">The position/translation vector.</param>
        private void SetOrientationAndPos(AeroQuaternion q, AeroVec3 pos)
        {
            Data[0] = 1 - (2 * q.J * q.J + 2 * q.K * q.K);
            Data[1] = 2 * q.I * q.J + 2 * q.K * q.R;
            Data[2] = 2 * q.I * q.K - 2 * q.J * q.R;
            Data[3] = pos.X;
            Data[4] = 2 * q.I * q.J - 2 * q.K * q.R;
            Data[5] = 1 - (2 * q.I * q.I + 2 * q.K * q.K);
            Data[6] = 2 * q.J * q.K + 2 * q.I * q.R;
            Data[7] = pos.Y;
            Data[8] = 2 * q.I * q.K + 2 * q.J * q.R;
            Data[9] = 2 * q.J * q.K - 2 * q.I * q.R;
            Data[10] = 1 - (2 * q.I * q.I + 2 * q.J * q.J);
            Data[11] = pos.Z;

        }

        #endregion

        #region Operators

        /// <summary>
        /// Multiplies a matrix by a vector (applies transformation).
        /// </summary>
        /// <param name="a">The matrix.</param>
        /// <param name="v">The vector.</param>
        /// <returns>The transformed vector.</returns>
        public static AeroVec3 operator*(AeroMat3x4 a, AeroVec3 v)
        {
            return new AeroVec3(
                v.X * a.Data[0] + v.Y * a.Data[1] + v.Z * a.Data[2] + a.Data[3],
                v.X * a.Data[4] + v.Y * a.Data[5] + v.Z * a.Data[6] + a.Data[7],
                v.X * a.Data[8] + v.Y * a.Data[9] + v.Z * a.Data[10] + a.Data[11]
            );
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The product matrix.</returns>
        public static AeroMat3x4 operator*(AeroMat3x4 a, AeroMat3x4 b)
        {
            return new AeroMat3x4(
                a.Data[0] * b.Data[0] + a.Data[1] * b.Data[4] + a.Data[2] * b.Data[8],
                a.Data[0] * b.Data[1] + a.Data[1] * b.Data[5] + a.Data[2] * b.Data[9],
                a.Data[0] * b.Data[2] + a.Data[1] * b.Data[6] + a.Data[2] * b.Data[10],
                a.Data[0] * b.Data[3] + a.Data[1] * b.Data[7] + a.Data[2] * b.Data[11] + a.Data[3],

                a.Data[4] * b.Data[0] + a.Data[5] * b.Data[4] + a.Data[6] * b.Data[8],
                a.Data[4] * b.Data[1] + a.Data[5] * b.Data[5] + a.Data[6] * b.Data[9],
                a.Data[4] * b.Data[2] + a.Data[5] * b.Data[6] + a.Data[6] * b.Data[10],
                a.Data[4] * b.Data[3] + a.Data[5] * b.Data[7] + a.Data[6] * b.Data[11] + a.Data[7],

                a.Data[8] * b.Data[0] + a.Data[9] * b.Data[4] + a.Data[10] * b.Data[8],
                a.Data[8] * b.Data[1] + a.Data[9] * b.Data[5] + a.Data[10] * b.Data[9],
                a.Data[8] * b.Data[2] + a.Data[9] * b.Data[6] + a.Data[10] * b.Data[10],
                a.Data[8] * b.Data[3] + a.Data[9] * b.Data[7] + a.Data[10] * b.Data[11] + a.Data[11]
            );
        }

        /// <summary>
        /// Multiplies this matrix by another matrix in place.
        /// </summary>
        /// <param name="other">The matrix to multiply this one by. </param>
        public void operator *=(AeroMat3x4 other)
        {
            this = this * other;
        }

        #endregion
    }
}
