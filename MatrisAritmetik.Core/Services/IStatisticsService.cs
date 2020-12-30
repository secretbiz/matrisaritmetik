using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    /// <summary>
    /// Class for methods of common statistical methods
    /// </summary>
    public interface IStatisticsService
    {
        /// <summary>
        /// 'Descire' given <paramref name="df"/> by concatenating: Min, Median, Max, Mode, Mean, Sdev, Var results as columns
        /// <para> and column labels as row labels</para>
        /// </summary>
        /// <param name="df">Dataframe or Matrix to describe</param>
        /// <param name="usePopulation">0 for samples, 1 for population</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Dataframe describing each column of <paramref name="df"/></returns>
        MatrisBase<object> Describe(MatrisBase<object> df,
                                    int usePopulation = 0,
                                    int numberOnly = 1);

        /// <summary>
        /// First <paramref name="n"/> rows of the given matrix or dataframe <paramref name="df"/>
        /// </summary>
        /// <param name="df">Matrix or dataframe to choose rows from</param>
        /// <param name="n">Amount of rows to return, if higher than row dimension of <paramref name="df"/>, copy of <paramref name="df"/>'s rows are returned</param>
        /// <returns>First <paramref name="n"/> rows of <paramref name="df"/></returns>
        MatrisBase<object> Head(MatrisBase<object> df,
                                int n = 5);

        /// <summary>
        /// Last <paramref name="n"/> rows of the given matrix or dataframe <paramref name="df"/>
        /// </summary>
        /// <param name="df">Matrix or dataframe to choose rows from</param>
        /// <param name="n">Amount of rows to return, if higher than row dimension of <paramref name="df"/>, copy of <paramref name="df"/>'s rows are returned</param>
        /// <returns>Last <paramref name="n"/> rows of <paramref name="df"/></returns>
        MatrisBase<object> Tail(MatrisBase<object> df,
                                int n = 5);

        /// <summary>
        /// Random <paramref name="n"/> rows of the given matrix or dataframe <paramref name="df"/>
        /// </summary>
        /// <param name="df">Matrix or dataframe to choose rows from</param>
        /// <param name="n">Amount of rows to return, if higher than row dimension of <paramref name="df"/>, copy of <paramref name="df"/>'s rows are returned</param>
        /// <returns>Random <paramref name="n"/> rows of <paramref name="df"/></returns>
        MatrisBase<object> Sample(MatrisBase<object> df,
                                  int n = 5);

        /// <summary>
        /// Sum of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Sum of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Sum(MatrisBase<object> df,
                               int numberOnly = 1);

        /// <summary>
        /// Multiplication of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Multiplication of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Mul(MatrisBase<object> df,
                               int numberOnly = 1);

        /// <summary>
        /// Minimum value of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Minimum value of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Min(MatrisBase<object> df,
                               int numberOnly = 1);

        /// <summary>
        /// Maximum value of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Maximum value of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Max(MatrisBase<object> df,
                               int numberOnly = 1);

        /// <summary>
        /// Mean of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Mean of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Mean(MatrisBase<object> df,
                                int numberOnly = 1);

        /// <summary>
        /// Mode of each column of<paramref name="df"/>, (most repeated value)
        /// <para>If there are multiple values repeated same amount, first one to reach that amount is used</para>
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Mode of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Mode(MatrisBase<object> df,
                                int numberOnly = 1);

        /// <summary>
        /// Median of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Median of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Median(MatrisBase<object> df,
                                  int numberOnly = 1);

        /// <summary>
        /// Standard deviation of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Standard deviation of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> SDev(MatrisBase<object> df,
                                int usePopulation = 0,
                                int numberOnly = 1);

        /// <summary>
        /// Variance of each column of<paramref name="df"/>, value will be <see cref="float.NaN"/> if column had non-number value(s)
        /// </summary>
        /// <param name="df">Matrix or dataframe to use</param>
        /// <param name="numberOnly">1 to use only the numbers in columns, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Variance of individual columns of <paramref name="df"/></returns>
        MatrisBase<object> Var(MatrisBase<object> df,
                               int usePopulation = 0,
                               int numberOnly = 1);

    }
}
