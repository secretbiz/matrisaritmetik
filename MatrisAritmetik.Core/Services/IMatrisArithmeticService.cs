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

        /// <summary>
        /// Row dimension of matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get row dimension of</param>
        /// <returns>Row dimension of <paramref name="A"/> or 0 if <paramref name="A"/> is invalid</returns>
        int RowDim(MatrisBase<T> A);

        /// <summary>
        /// Column dimension of matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get column dimension of</param>
        /// <returns>Column dimension of <paramref name="A"/> or 0 if <paramref name="A"/> is invalid</returns>
        int ColDim(MatrisBase<T> A);

        /// <summary>
        /// Gets the element at index(base=<paramref name="based"/>) (<paramref name="i"/>,<paramref name="j"/>) of <paramref name="A"/> as a scalar matrix
        /// </summary>
        /// <param name="A">Matrix to index over</param>
        /// <param name="i">Row index</param>
        /// <param name="j">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>Scalar matrix with value at <paramref name="A"/>[<paramref name="i"/>-<paramref name="based"/>, <paramref name="j"/>-<paramref name="based"/>]</returns>
        MatrisBase<T> Get(MatrisBase<T> A,
                          int i,
                          int j,
                          int based = 0);

        /// <summary>
        /// Gets <paramref name="i"/>th row of matrix <paramref name="A"/> with index base <paramref name="based"/>
        /// </summary>
        /// <param name="A">Matrix to get row of</param>
        /// <param name="i">Row index</param>
        /// <param name="based">Index base</param>
        /// <returns>A deep copy of the <paramref name="i"/>th row of <paramref name="A"/></returns>
        MatrisBase<T> Row(MatrisBase<T> A,
                          int i,
                          int based = 0);

        /// <summary>
        /// Gets <paramref name="i"/>th column of matrix <paramref name="A"/> with index base <paramref name="based"/>
        /// </summary>
        /// <param name="A">Matrix to get column of</param>
        /// <param name="i">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>A deep copy of the <paramref name="i"/>th column of <paramref name="A"/></returns>
        MatrisBase<T> Col(MatrisBase<T> A,
                          int j,
                          int based = 0);

        /// <summary>
        /// Gets the rows and column with given indices, <paramref name="r2"/> and <paramref name="c2"/> exclusive, index base <paramref name="based"/>
        /// </summary>
        /// <param name="A">Matrix to get a sub-matrix from</param>
        /// <param name="r1">Starting row index</param>
        /// <param name="r2">Ending row index, exclusive</param>
        /// <param name="c1">Starting column index</param>
        /// <param name="c2">Ending column index, exclusive</param>
        /// <param name="based">Index base</param>
        /// <returns>Deep copy of elements within given indices from matrix <paramref name="A"/></returns>
        MatrisBase<T> Sub(MatrisBase<T> A,
                          int r1,
                          int r2,
                          int c1,
                          int c2,
                          int based = 0);

        /// <summary>
        /// Resize matrix <paramref name="A"/> with new dimensions <paramref name="row"/>x<paramref name="col"/>
        /// </summary>
        /// <param name="A">Matrix to resize</param>
        /// <param name="row">New row dimension</param>
        /// <param name="col">New column dimension</param>
        /// <returns>Copy of elements in a new matrix with dimensions <paramref name="row"/>x<paramref name="col"/></returns>
        MatrisBase<T> Resize(MatrisBase<T> A,
                             int row,
                             int col);

        /// <summary>
        /// Signs of the elements of matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get signs of values from</param>
        /// <returns>A new matrix with 1's for values>=0 and -1's otherwise</returns>
        MatrisBase<T> Sign(MatrisBase<T> A);

        /// <summary>
        /// Absolute values of the elements of matrix <paramref name="A"/>
        /// </summary>
        /// <param name="A">Matrix to get absolute values of</param>
        /// <returns>A new matrix with absolute values</returns>
        MatrisBase<T> Abs(MatrisBase<T> A);

        /// <summary>
        /// Round values in matrix <paramref name="A"/> after <paramref name="n"/> decimal places
        /// </summary>
        /// <param name="A">Matrix to round values of</param>
        /// <param name="n">Decimal places to rount to</param>
        /// <returns>A new matrix with rounded values</returns>
        MatrisBase<T> Round(MatrisBase<T> A, int n = 0);

        /// <summary>
        /// Replace <paramref name="old"/> values within range [<paramref name="old"/>-<paramref name="TOL"/>, <paramref name="old"/>+<paramref name="TOL"/>] with '<paramref name="with"/>'
        /// </summary>
        /// <param name="A">Matrix to replace values of</param>
        /// <param name="old">Value to replace</param>
        /// <param name="with">New value to replace <paramref name="old"/> with</param>
        /// <param name="TOL">Tolerance for <paramref name="old"/></param>
        /// <returns>A new matrix with replaced values</returns>
        MatrisBase<T> Replace(MatrisBase<T> A,
                              float old,
                              float with,
                              float TOL = (float)1e-6);

        /// <summary>
        /// Shuffle matrix <paramref name="A"/> along given <paramref name="axis"/>; 0 = rows , 1 = cols , 2 = both axis
        /// </summary>
        /// <param name="A">Matrix to shuffle values of</param>
        /// <param name="axis">0 = row shuffle, 1 = column shuffle, 2 = shuffle both</param>
        /// <returns>Copies of values in <paramref name="A"/> shuffled</returns>
        MatrisBase<T> Shuffle(MatrisBase<T> A,
                              int axis = 2);
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
