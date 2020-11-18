namespace MatrisAritmetik.Core.Services
{
    /// <summary>
    /// Service for methods related to matrix arithmetic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatrisArithmeticService<T>
    {
        #region Single Matrix Parameter Methods
        /// <summary>
        /// Gets the transpose of the matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get transpose of</param>
        /// <returns>Transpose of <paramref name="A"/>, values deep copied</returns>
        MatrisBase<T> Transpose(MatrisBase<T> A);

        /// <summary>
        /// Gets the conjugate transpose of the matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get transpose of</param>
        /// <returns>Conjugate transpose of <paramref name="A"/>, values deep copied</returns>
        MatrisBase<T> Conjugate(MatrisBase<T> A);

        /// <summary>
        /// Gets the echelon form of the matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get echelon form of</param>
        /// <returns>Echelon form of <paramref name="A"/></returns>
        MatrisBase<T> Echelon(MatrisBase<T> A);

        /// <summary>
        /// Gets the row-reduced echelon form of the matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get row-reduced echelon form of</param>
        /// <returns>Row-reduced echelon form of <paramref name="A"/></returns>
        MatrisBase<T> RREchelon(MatrisBase<T> A);

        /// <summary>
        /// Gets the determinant of the matrix <paramref name="A"/> if it's a square matrix
        /// <para>Throws <see cref="CompilerMessage.MAT_NOT_SQUARE"/> if matrix is not square</para>
        /// </summary>
        /// <param name="A">Matrix to get the determinant of</param>
        /// <returns>Determinant as <see cref="float"/></returns>
        float Determinant(MatrisBase<T> A);

        /// <summary>
        /// Gets the rank of the matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A"></param>
        /// <returns>Rank as <see cref="int"/></returns>
        int Rank(MatrisBase<T> A);

        /// <summary>
        /// Gets the adjoint matrix of matrix <paramref name="A"/> if it's a square matrix
        /// <para>Throws <see cref="CompilerMessage.MAT_NOT_SQUARE"/> if matrix is not square</para>
        /// </summary>
        /// <param name="A">Matrix to get the adjoint matrix from</param>
        /// <returns>Adjoint matrix of <paramref name="A"/></returns>
        MatrisBase<T> Adjoint(MatrisBase<T> A);

        /// <summary>
        /// Gets the inverse of the matrix <paramref name="A"/> if it's a square matrix
        /// <para>Throws <see cref="CompilerMessage.MAT_NOT_SQUARE"/> if matrix is not square</para>
        /// </summary>
        /// <param name="A">Matrix to get the inverse of</param>
        /// <returns>Inverse of matrix <paramref name="A"/></returns>
        MatrisBase<T> Inverse(MatrisBase<T> A);

        /// <summary>
        /// Gets the pseudo-inverse of the matrix <paramref name="A"/> from given side
        /// <para>Throws <see cref="CompilerMessage.MAT_PSEINV_NOT_FULL_RANK"/> if matrix is not full rank</para>
        /// </summary>
        /// <param name="A">Matrix to get the pseudo-inverse of</param>
        /// <param name="side">Side to calculate pseudo-inverse by (left:-1 , right:1)</param>
        /// <returns>left(<paramref name="side"/>:-1) or right(<paramref name="side"/>:1) Pseudo-inverse of matrix <paramref name="A"/></returns>
        MatrisBase<T> PseudoInverse(MatrisBase<T> A,
                                    int side = -1);

        /// <summary>
        /// Gets the minor of the matrix <paramref name="A"/> at index (<paramref name="row"/>,<paramref name="col"/>) with 
        /// index base <paramref name="based"/>
        /// </summary>
        /// <param name="A">Matrix to get the minor of</param>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>Minor of matrix <paramref name="A"/> at index (<paramref name="row"/>,<paramref name="col"/>) with 
        /// index base <paramref name="based"/></returns>
        float Minor(MatrisBase<T> A,
                    int row,
                    int col,
                    int based = 0);
        #endregion

        #region Multiple Matrix Parameter Methods
        /// <summary>
        /// Gets the matrix of which the minor is calculated from at index (<paramref name="row"/>,<paramref name="col"/>) with 
        /// index base <paramref name="based"/> on matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get the minor of</param>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>The matrix of which the minor is calculated from</returns>
        MatrisBase<T> MinorMatris(MatrisBase<T> A,
                                  int row,
                                  int col,
                                  int based = 0);

        /// <summary>
        /// Gets the matrix multiplication of <paramref name="A"/> and <paramref name="B"/> in the given order
        /// </summary>
        /// <param name="A">Matrix on the left</param>
        /// <param name="B">Matrix on the right</param>
        /// <returns>Resulting matrix from the matrix multiplication of <paramref name="A"/> and <paramref name="B"/></returns>
        MatrisBase<T> MatrisMul(MatrisBase<T> A,
                                MatrisBase<T> B);

        /// <summary>
        /// Concatenates matrices <paramref name="A"/> and <paramref name="B"/> by the given axis
        /// </summary>
        /// <param name="A">Matrix to concatenate the other matrix to</param>
        /// <param name="B">Matrix to concatenate</param>
        /// <param name="axis">Axis to concatenate <paramref name="B"/> to <paramref name="A"/> as (row: 0, column: 1)</param>
        /// <returns>A new matrix with <paramref name="B"/> concatenated to <paramref name="A"/> as rows (<paramref name="axis"/>:0) or cols (<paramref name="axis"/>:1)</returns>
        MatrisBase<T> Concatenate(MatrisBase<T> A,
                                  MatrisBase<T> B,
                                  int axis = 0);
        #endregion

    }
}
