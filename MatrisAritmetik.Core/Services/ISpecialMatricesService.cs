using System;
using MatrisAritmetik.Core.Models;

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

        /// <summary>
        /// Create a range of values from <paramref name="start"/> to inclusively to <paramref name="end"/> with given <paramref name="interval"/>s
        /// <para> Axis: 0 for row vector, 1 for column vector</para>
        /// </summary>
        /// <param name="start">Starting value</param>
        /// <param name="end">Ending range, inclusively</param>
        /// <param name="interval">Interval width</param>
        /// <param name="axis">0 for row vector, 1 for column vector</param>
        /// <param name="digits">Decimal places to round to for each value</param>
        /// <returns>A row or column vector filled with values in given range</returns>
        MatrisBase<dynamic> Range(float start,
                                  float end,
                                  float interval = 1,
                                  int axis = 0,
                                  int digits = 6);
        /// <summary>
        /// Create a matrix filled with <paramref name="fill"/>
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="fill">Value to fill the matrix with</param>
        /// <returns>A <paramref name="row"/>x<paramref name="col"/> matrix filled with <paramref name="fill"/></returns>
        MatrisBase<dynamic> Fill(int row,
                                 int col,
                                 float fill);

        #endregion

        #region Random Matrices

        /// <summary>
        /// Create a symmetric, left-upper-triangular part randomly integer filled matrix 
        /// </summary>
        /// <param name="dimension">Row and column dimension for square matrix</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>Symmetrically filled <see cref="MatrisBase{T}"/> with <see cref="int"/>egers</returns>
        MatrisBase<dynamic> SymInt(int dimension,
                                   int min = 0,
                                   int max = 1,
                                   dynamic seed = null);

        /// <summary>
        /// Create a symmetric, left-upper-triangular part randomly float filled matrix 
        /// </summary>
        /// <param name="dimension">Row and column dimension for square matrix</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>Symmetrically filled <see cref="MatrisBase{T}"/> with <see cref="float"/>s</returns>
        MatrisBase<dynamic> SymFloat(int dimension,
                                     float min = 0,
                                     float max = 1,
                                     dynamic seed = null);

        /// <summary>
        /// Creates a randomly filled matrix using <see cref="int"/>egers
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>A <see cref="MatrisBase{dynamic}"/> with dimensions (<paramref name="row"/>,<paramref name="col"/>)  
        /// filled with random <see cref="int"/>eger values within the range [<paramref name="min"/>,<paramref name="max"/>]</returns>
        MatrisBase<dynamic> RandInt(int row,
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
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>A <see cref="MatrisBase{dynamic}"/> with dimensions (<paramref name="row"/>,<paramref name="col"/>)  
        /// filled with random <see cref="float"/> values within the range [<paramref name="min"/>,<paramref name="max"/>]</returns>
        MatrisBase<dynamic> RandFloat(int row,
                                      int col,
                                      float min = (float)0.0,
                                      float max = (float)1.0,
                                      dynamic seed = null);
        #endregion

        #region Non-Random Dataframe
        /// <summary>
        /// Creates an Identity matrix-like dataframe
        /// </summary>
        /// <param name="dimension">Row and column dimension</param>
        /// <returns>A <see cref="MatrisBase{dynamic}"/> with 1's in diagonal, 0's anywhere else</returns>
        Dataframe IdentityDf(int dimension);

        /// <summary>
        /// Create a range of values from <paramref name="start"/> to inclusively to <paramref name="end"/> with given <paramref name="interval"/>s
        /// <para> Axis: 0 for row vector, 1 for column vector</para>
        /// </summary>
        /// <param name="start">Starting value</param>
        /// <param name="end">Ending range, inclusively</param>
        /// <param name="interval">Interval width</param>
        /// <param name="axis">0 for row vector, 1 for column vector</param>
        /// <param name="digits">Decimal places to round to for each value</param>
        /// <returns>A row or column vector filled with values in given range</returns>
        Dataframe RangeDf(float start,
                          float end,
                          float interval = 1,
                          int axis = 0,
                          int digits = 6);

        /// <summary>
        /// Create a matrix filled with <paramref name="fill"/>
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="fill">Value to fill the matrix with</param>
        /// <returns>A <paramref name="row"/>x<paramref name="col"/> matrix filled with <paramref name="fill"/></returns>
        Dataframe FillDf(int row,
                         int col,
                         float fill);

        #endregion

        #region Random Dataframe

        /// <summary>
        /// Create a symmetric, left-upper-triangular part randomly integer filled matrix 
        /// </summary>
        /// <param name="dimension">Row and column dimension for square matrix</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>Symmetrically filled <see cref="Dataframe"/> with <see cref="int"/>egers</returns>
        Dataframe SymIntDf(int dimension,
                           int min = 0,
                           int max = 1,
                           dynamic seed = null);

        /// <summary>
        /// Create a symmetric, left-upper-triangular part randomly float filled dataframe 
        /// </summary>
        /// <param name="dimension">Row and column dimension for square matrix</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>Symmetrically filled <see cref="Dataframe"/> with <see cref="float"/>s</returns>
        Dataframe SymFloatDf(int dimension,
                             float min = 0,
                             float max = 1,
                             dynamic seed = null);

        /// <summary>
        /// Creates a randomly filled dataframe using <see cref="int"/>egers
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>A <see cref="Dataframe"/> with dimensions (<paramref name="row"/>,<paramref name="col"/>)  
        /// filled with random <see cref="int"/>eger values within the range [<paramref name="min"/>,<paramref name="max"/>]</returns>
        Dataframe RandIntDf(int row,
                            int col,
                            int min = 0,
                            int max = 1,
                            dynamic seed = null);

        /// <summary>
        /// Creates a randomly filled dataframe using <see cref="float"/>s
        /// </summary>
        /// <param name="row">Row dimension</param>
        /// <param name="col">Column dimension</param>
        /// <param name="min">Minimum value for random values</param>
        /// <param name="max">Maximum value for random values</param>
        /// <param name="seed">Seed to use with <see cref="Random"/></param>
        /// <returns>A <see cref="Dataframe"/> with dimensions (<paramref name="row"/>,<paramref name="col"/>)  
        /// filled with random <see cref="float"/> values within the range [<paramref name="min"/>,<paramref name="max"/>]</returns>
        Dataframe RandFloatDf(int row,
                              int col,
                              float min = (float)0.0,
                              float max = (float)1.0,
                              dynamic seed = null);
        #endregion
    }
}
