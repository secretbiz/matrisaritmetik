using System;

namespace MatrisAritmetik.Core.Services
{
    /// <summary>
    /// Service for methods to create special matrices
    /// </summary>
    public interface ISpecialMatricesService
    {
        #region Non-Random Matrices
        /// <summary>
        /// Creates an Identity matrix
        /// </summary>
        /// <param name="dimension">Row and column dimension</param>
        /// <returns>A <see cref="MatrisBase{dynamic}"/> with 1's in diagonal, 0's anywhere else</returns>
        MatrisBase<dynamic> Identity(int dimension);
        #endregion

        #region Random Matrices
        /// <summary>
        /// Creates a randomly filled matrix using <see cref="int"/>egers
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use for getting random values</param>
        /// <returns>A <see cref="MatrisBase{dynamic}"/> with dimensions (<paramref name="row"/>,<paramref name="col"/>)  
        /// filled with random <see cref="int"/>eger values within the range [<paramref name="min"/>,<paramref name="max"/>]</returns>
        MatrisBase<dynamic> RandIntMat(int row,
                                       int col,
                                       int min = 0,
                                       int max = 1,
                                       dynamic seed = null);

        /// <summary>
        /// Creates a randomly filled matrix using <see cref="float"/>s
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use for getting random values from <see cref="Random"/></param>
        /// <returns>A <see cref="MatrisBase{dynamic}"/> with dimensions (<paramref name="row"/>,<paramref name="col"/>)  
        /// filled with random <see cref="float"/> values within the range [<paramref name="min"/>,<paramref name="max"/>]</returns>
        MatrisBase<dynamic> RandFloatMat(int row,
                                         int col,
                                         float min = (float)0.0,
                                         float max = (float)1.0,
                                         dynamic seed = null);
        #endregion
    }
}
