namespace AeroliteSharpEngine.AeroMath
{
    /// <summary>
    /// Represents a 3x3 matrix with single-precision floating-point elements.
    /// Provides common matrix operations and properties for 3D math.
    /// </summary>
    public struct AeroMat3x3
    {
        #region Properties

        /// <summary>
        /// Gets the underlying data array of the matrix in row-major order.
        /// </summary>
        public readonly float[] Data { get; init; }

        /// <summary>
        /// Gets the number of rows in the matrix (always 3).
        /// </summary>
        public readonly int Rows => 3;

        /// <summary>
        /// Gets the number of columns in the matrix (always 3).
        /// </summary>
        public readonly int Columns => 3;

        /// <summary>
        /// Gets the determinant of the matrix.
        /// </summary>
        public readonly float Determinant
        {
            get
            {
                return
                    Data[0] * (Data[4] * Data[8] - Data[5] * Data[7]) -
                    Data[1] * (Data[3] * Data[8] - Data[5] * Data[6]) +
                    Data[2] * (Data[3] * Data[7] - Data[4] * Data[6]);
            }
        }   

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroMat3x3"/> struct from a float array.
        /// </summary>
        /// <param name="data">A float array of length 9 representing the matrix elements in row-major order.</param>
        /// <exception cref="ArgumentException">Thrown if the array is null or not of length 9.</exception>
        public AeroMat3x3(float[] data)
        {
            if (data == null || data.Length != 9)
                throw new ArgumentException("Data must be a float array of length 9.");
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroMat3x3"/> struct from individual elements.
        /// </summary>
        /// <param name="m11">Element at row 1, column 1.</param>
        /// <param name="m12">Element at row 1, column 2.</param>
        /// <param name="m13">Element at row 1, column 3.</param>
        /// <param name="m21">Element at row 2, column 1.</param>
        /// <param name="m22">Element at row 2, column 2.</param>
        /// <param name="m23">Element at row 2, column 3.</param>
        /// <param name="m31">Element at row 3, column 1.</param>
        /// <param name="m32">Element at row 3, column 2.</param>
        /// <param name="m33">Element at row 3, column 3.</param>
        public AeroMat3x3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            Data =
            [
                m11, m12, m13,
                m21, m22, m23,
                m31, m32, m33
            ];
        }

        /// <summary>
        /// Gets the identity matrix (diagonal elements are 1, others are 0).
        /// </summary>
        public static AeroMat3x3 Identity => new AeroMat3x3(
            1f, 0f, 0f,
            0f, 1f, 0f,
            0f, 0f, 1f
        );

        #endregion

        #region Methods

        /// <summary>
        /// Transforms a vector by this matrix.
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
        public AeroMat3x3 Multiply(AeroMat3x3 other)
        {
            return this * other;
        }

        /// <summary>
        /// Returns the inverse of this matrix.
        /// </summary>
        /// <returns>The inverse matrix.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        public AeroMat3x3 Inverse()
        {
            AeroMat3x3 result = new AeroMat3x3(new float[9]);
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
        /// Returns the transpose of this matrix.
        /// </summary>
        /// <returns>The transposed matrix.</returns>
        public AeroMat3x3 Transposed()
        {
            AeroMat3x3 result = new AeroMat3x3(new float[9]);
            result.SetTranspose(this);
            return result;
        }
        
        /// <summary>
        /// Transposes this matrix in place.
        /// </summary>
        public void Transpose()
        {
            SetTranspose(this);
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="q">The quaternion representing the rotation.</param>
        /// <returns>The rotation matrix.</returns>
        public static AeroMat3x3 FromQuaternion(AeroQuaternion q)
        {
            var mat = new AeroMat3x3(new float[9]);
            mat.SetOrientation(q);
            return mat;
        }

        /// <summary>
        /// Sets the orientation of this matrix from a quaternion.
        /// </summary>
        /// <param name="q">The quaternion representing the rotation.</param>
        public void SetOrientationFromQuaternion(AeroQuaternion q)
        {
            SetOrientation(q);
        }

        /// <summary>
        /// Sets this matrix to the inverse of another matrix.
        /// </summary>
        /// <param name="m">The matrix to invert.</param>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        private void SetInverse(AeroMat3x3 m)
        {
            // Calculate the determinant
            float det =
                m.Data[0] * (m.Data[4] * m.Data[8] - m.Data[5] * m.Data[7]) -
                m.Data[1] * (m.Data[3] * m.Data[8] - m.Data[5] * m.Data[6]) +
                m.Data[2] * (m.Data[3] * m.Data[7] - m.Data[4] * m.Data[6]);
            if (AeroMathExtensions.IsNearlyZero(det))
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            float invDet = 1.0f / det;
            Data[0] = (m.Data[4] * m.Data[8] - m.Data[5] * m.Data[7]) * invDet;
            Data[1] = (m.Data[2] * m.Data[7] - m.Data[1] * m.Data[8]) * invDet;
            Data[2] = (m.Data[1] * m.Data[5] - m.Data[2] * m.Data[4]) * invDet;
            Data[3] = (m.Data[5] * m.Data[6] - m.Data[3] * m.Data[8]) * invDet;
            Data[4] = (m.Data[0] * m.Data[8] - m.Data[2] * m.Data[6]) * invDet;
            Data[5] = (m.Data[2] * m.Data[3] - m.Data[0] * m.Data[5]) * invDet;
            Data[6] = (m.Data[3] * m.Data[7] - m.Data[4] * m.Data[6]) * invDet;
            Data[7] = (m.Data[1] * m.Data[6] - m.Data[0] * m.Data[7]) * invDet;
            Data[8] = (m.Data[0] * m.Data[4] - m.Data[1] * m.Data[3]) * invDet;
        }

        /// <summary>
        /// Sets this matrix to the transpose of another matrix.
        /// </summary>
        /// <param name="m">The matrix to transpose.</param>
        private void SetTranspose(AeroMat3x3 m)
        {
            Data[0] = m.Data[0];
            Data[1] = m.Data[3];
            Data[2] = m.Data[6];
            Data[3] = m.Data[1];
            Data[4] = m.Data[4];
            Data[5] = m.Data[7];
            Data[6] = m.Data[2];
            Data[7] = m.Data[5];
            Data[8] = m.Data[8];
        }   

        /// <summary>
        /// Sets this matrix to represent a rotation defined by a quaternion.
        /// </summary>
        /// <param name="q">The quaternion representing the rotation.</param>
        private void SetOrientation(AeroQuaternion q)
        {
            Data[0] = 1 - 2 * (q.J * q.J + q.K * q.K);
            Data[1] = 2 * (q.I * q.J - q.K * q.R);
            Data[2] = 2 * (q.I * q.K + q.J * q.R);
            Data[3] = 2 * (q.I * q.J + q.K * q.R);
            Data[4] = 1 - 2 * (q.I * q.I + q.K * q.K);
            Data[5] = 2 * (q.J * q.K - q.I * q.R);
            Data[6] = 2 * (q.I * q.K - q.J * q.R);
            Data[7] = 2 * (q.J * q.K + q.I * q.R);
            Data[8] = 1 - 2 * (q.I * q.I + q.J * q.J);  
        }

        #endregion  

        #region Operators

        /// <summary>
        /// Multiplies a matrix by a vector.
        /// </summary>
        /// <param name="a">The matrix.</param>
        /// <param name="v">The vector.</param>
        /// <returns>The transformed vector.</returns>
        public static AeroVec3 operator*(AeroMat3x3 a, AeroVec3 v)
        {
            return new AeroVec3(
                v.X * a.Data[0] + v.Y * a.Data[1] + v.Z * a.Data[2],
                v.X * a.Data[3] + v.Y * a.Data[4] + v.Z * a.Data[5],
                v.X * a.Data[6] + v.Y * a.Data[7] + v.Z * a.Data[8]
            );
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The product matrix.</returns>
        public static AeroMat3x3 operator*(AeroMat3x3 a, AeroMat3x3 b)
        {
            return new AeroMat3x3(
                a.Data[0] * b.Data[0] + a.Data[1] * b.Data[3] + a.Data[2] * b.Data[6],
                a.Data[0] * b.Data[1] + a.Data[1] * b.Data[4] + a.Data[2] * b.Data[7],
                a.Data[0] * b.Data[2] + a.Data[1] * b.Data[5] + a.Data[2] * b.Data[8],

                a.Data[3] * b.Data[0] + a.Data[4] * b.Data[3] + a.Data[5] * b.Data[6],
                a.Data[3] * b.Data[1] + a.Data[4] * b.Data[4] + a.Data[5] * b.Data[7],
                a.Data[3] * b.Data[2] + a.Data[4] * b.Data[5] + a.Data[5] * b.Data[8],

                a.Data[6] * b.Data[0] + a.Data[7] * b.Data[3] + a.Data[8] * b.Data[6],
                a.Data[6] * b.Data[1] + a.Data[7] * b.Data[4] + a.Data[8] * b.Data[7],
                a.Data[6] * b.Data[2] + a.Data[7] * b.Data[5] + a.Data[8] * b.Data[8]
                );
        }

        /// <summary>
        /// Multiplies this matrix by another matrix in place.
        /// </summary>
        /// <param name="other">The other matrix this one is being multiplied by.</param>
        public void operator *=(AeroMat3x3 other)
        {
            this = this * other;
        }   

        #endregion
    }
}
