using System;
using System.Collections.Generic;
using System.Linq;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Services
{
    public class MatrisArithmeticService : IMatrisArithmeticService<object>
    {
        #region MatrisArithmeticService Methods

        public MatrisBase<object> Transpose(MatrisBase<object> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            List<List<object>> result = new List<List<object>>();

            for (int j = 0; j < A.Col; j++)
            {
                result.Add(A.ColList(j, 0));
            }

            return A is Dataframe df
                ? new Dataframe(result,
                                df.Delimiter,
                                df.NewLine,
                                Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                df.GetRowSettings().Copy(),
                                df.GetColSettings().Copy())
                : new MatrisBase<object>(result);
        }

        public MatrisBase<object> Conjugate(MatrisBase<object> A)
        {
            return Transpose(A);
        }

        private void InnerEchelon(MatrisBase<object> A, MatrisBase<object> result)
        {
            int nr = A.Row;
            int nc = A.Col;
            List<int> zeroCols = new List<int>();
            List<List<object>> filteredResult = A.Copy().GetValues();
            for (int j = 0; j < nc; j++)
            {
                if (A.IsZeroCol(j, 0, (float)0.0))
                {
                    for (int i = 0; i < nr; i++)
                    {
                        filteredResult[i].RemoveAt(j - zeroCols.Count);
                    }

                    zeroCols.Add(j);
                    nc--;
                }
            }

            result.SetValues(filteredResult);

            for (int r = 0; r < nr; r++)
            {
                if (result.IsZeroRow(r, 0, (float)0.0))
                {
                    result.SwapToEnd(r, 0);
                    nr--;
                }
            }

            int p = 0;
            bool next;
            int swapCount = 0;

            while (p < nr && p < nc)
            {
                next = false;
                int r = 1;
                while (float.Parse(result.GetValues()[p][p].ToString()) == (float)0.0)
                {
                    if (p + 1 < nr)
                    {
                        if (result.IsZeroRow(p, 0, (float)0.0))
                        {
                            nr--;
                        }

                        if (!Sub(result, p, nr, p, p + 1, 0).IsZeroCol(0, 0))
                        {
                            for (int ri = p + 1; ri < nr; ri++)
                            {
                                if (Math.Abs(float.Parse(result.GetValues()[ri][p].ToString())) > 1e-6)
                                {
                                    result.Swap(p, ri, based: 0);
                                    swapCount++;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            p++;
                        }
                        next = true;
                        break;
                    }
                    else
                    {
                        // Zeros to bottom
                        for (int i = 0; i < A.Row; i++)
                        {
                            if (result.IsZeroRow(i, 0, (float)0.0))
                            {
                                result.SwapToEnd(i, 0);
                            }
                        }
                        // Restore zero columns
                        if (zeroCols.Count > 0)
                        {
                            foreach (int j in zeroCols)
                            {
                                for (int i = 0; i < result.Row; i++)
                                {
                                    result.GetValues()[i].Insert(j, (dynamic)(float)0.0);
                                }
                            }
                            result.SetCol(A.Col);
                        }
                        result.FixMinusZero();
                        result.SwapCount = swapCount;
                        return;
                    }
                }

                if (next)
                {
                    continue;
                }

                for (; r >= 1 && r < (nr - p); r++)
                {
                    if (float.Parse(result.GetValues()[p + r][p].ToString()) != (float)0.0)
                    {
                        float x = -(float.Parse(result.GetValues()[p + r][p].ToString()) / float.Parse(result.GetValues()[p][p].ToString()));
                        for (int c = p; c < nc; c++)
                        {
                            result.GetValues()[p + r][c] = (dynamic)((float.Parse(result.GetValues()[p][c].ToString()) * x) + float.Parse(result.GetValues()[p + r][c].ToString()));
                        }
                    }
                }
                p++;
            }

            // Zeros to bottom
            for (int i = 0; i < A.Row; i++)
            {
                if (result.IsZeroRow(i, 0, (float)0.0))
                {
                    result.SwapToEnd(i, 0);
                }
            }
            // Restore zero columns
            if (zeroCols.Count > 0)
            {
                foreach (int j in zeroCols)
                {
                    for (int i = 0; i < result.Row; i++)
                    {
                        result.GetValues()[i].Insert(j, (dynamic)(float)0.0);
                    }
                }
                result.SetCol(A.Col);
            }

            result.FixMinusZero();
            result.SwapCount = swapCount;
        }

        public MatrisBase<object> Echelon(MatrisBase<object> A)
        {
            // Bad dimensions
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            // Zero matrix
            if (A.IsZero((float)0.0))
            {
                return A;
            }

            if (A is Dataframe df)
            {
                CompilerUtils.AssertMatrixValsAreNumbers(A);
                Dataframe result = df.Copy();
                InnerEchelon(df, result);
                return result;
            }
            else
            {
                MatrisBase<object> result = A.Copy();
                InnerEchelon(A, result);
                return result;
            }

        }

        private static void InnerRREchelon(MatrisBase<object> A, MatrisBase<object> result)
        {
            int lead = 0;
            int nr = A.Row;
            int nc = A.Col;

            for (int r = 0; r < nr; r++)
            {
                if (nc <= lead)
                {
                    break;
                }
                int i = r;
                while (float.Parse(result[i, lead].ToString()) == (float)0.0)
                {
                    i++;
                    if (nr == i)
                    {
                        i = r;
                        lead++;
                        if (nc == lead)
                        {
                            break;
                        }
                    }
                }

                result.Swap(i, r, 0, 0);

                if (float.Parse(result[r, lead].ToString()) != (float)0.0)
                {
                    result.MulRow(r, (float)1.0 / float.Parse(result[r, lead].ToString()), 0);
                }
                for (int j = 0; j < nr; j++)
                {
                    if (j != r)
                    {
                        for (int k = 0; k < nc; k++)
                        {
                            result.MulThenSubFromOtherRow(r, float.Parse(result[j, lead].ToString()), j, 0);
                        }
                    }
                }
                lead++;
            }

            result.FixMinusZero();

        }

        public MatrisBase<object> RREchelon(MatrisBase<object> A)
        {
            // Bad dimensions
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            // Zero matrix
            if (A.IsZero((float)0.0))
            {
                return A;
            }

            if (A is Dataframe df)
            {
                CompilerUtils.AssertMatrixValsAreNumbers(A);

                Dataframe result = df.Copy();
                InnerRREchelon(df, result);
                return result;
            }
            else
            {
                MatrisBase<object> result = A.Copy();
                InnerRREchelon(A, result);
                return result;
            }

        }

        public float Determinant(MatrisBase<object> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            if (A.IsZero((float)0.0))
            {
                CompilerUtils.AssertMatrixValsAreNumbers(A);
                return (float)0.0;
            }

            if (A.Row == 1)
            {
                CompilerUtils.AssertMatrixValsAreNumbers(A);
                return float.Parse(A.GetValues()[0][0].ToString());
            }

            if (A.Row == 2)
            {
                CompilerUtils.AssertMatrixValsAreNumbers(A);
                return (float.Parse(A.GetValues()[0][0].ToString()) * float.Parse(A.GetValues()[1][1].ToString()))
                       - (float.Parse(A.GetValues()[0][1].ToString()) * float.Parse(A.GetValues()[1][0].ToString()));
            }

            using MatrisBase<object> ech = Echelon(A.Copy());

            float det = float.Parse(ech.GetValues()[0][0].ToString());
            if (ech.SwapCount % 2 == 1)
            {
                det *= -1;
            }

            int dim = A.Row;

            for (int i = 1; i < dim; i++)
            {
                det *= float.Parse(ech.GetValues()[i][i].ToString());
            }
            return det;
        }

        public int Rank(MatrisBase<object> A)
        {
            using MatrisBase<object> ech = Echelon(A.Copy());
            int zeroCount = 0;
            if (A.Row <= A.Col)
            {
                for (int i = ech.Row - 1; i >= 0; i--)
                {
                    if (ech.IsZeroRow(i, 0, (float)0.0))
                    {
                        zeroCount++;
                    }
                }
                return ech.Row - zeroCount;
            }
            else
            {
                for (int i = ech.Col - 1; i >= 0; i--)
                {
                    if (ech.IsZeroCol(i, 0, (float)0.0))
                    {
                        zeroCount++;
                    }
                }
                return ech.Col - zeroCount;
            }
        }

        public float Trace(MatrisBase<object> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID);
            }
            float trace = float.Parse(A[0, 0].ToString());
            int m = Math.Min(A.Row, A.Col);
            for (int i = 1; i < m; i++)
            {
                if (float.TryParse(A[i, i].ToString(), out float res))
                {
                    trace += res;
                }
                else
                {
                    return float.NaN;
                }
            }

            return trace;
        }

        public MatrisBase<object> Inverse(MatrisBase<object> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            if (Determinant(A) == (float)0.0)
            {
                throw new Exception(CompilerMessage.MAT_DET_ZERO_NO_INV);
            }

            using MatrisBase<object> temp = Concatenate(A is Dataframe ? ((Dataframe)A.Copy()) : A.Copy(),
                                                        (dynamic)new SpecialMatricesService().Identity(A.Row),
                                                        1);

            return A is Dataframe df
                    ? new Dataframe(RREchelon(temp)[new Range(new Index(0), new Index(temp.Row)), new Range(new Index(A.Col), new Index(temp.Col))],
                                    df.Delimiter,
                                    df.NewLine,
                                    Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                    Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                    df.GetRowSettings().Copy(),
                                    df.GetColSettings().Copy())
                    : new MatrisBase<object>(vals: RREchelon(temp)[new Range(new Index(0), new Index(temp.Row)), new Range(new Index(A.Col), new Index(temp.Col))]);
        }

        public MatrisBase<object> PseudoInverse(MatrisBase<object> A,
                                                int side = -1)
        {
            if (Rank(A) != Math.Min(A.Row, A.Col))
            {
                throw new Exception(CompilerMessage.MAT_PSEINV_NOT_FULL_RANK);
            }

            if (side != -1 && side != 1)
            {
                throw new Exception(CompilerMessage.MAT_PSEINV_BAD_SIDE);
            }

            CompilerUtils.AssertMatrixValsAreNumbers(A);

            string sidename = side == -1 ? "sol" : "sağ";

            // Left inverse
            if (side == -1)
            {
                try
                {
                    return A is Dataframe df
                        ? new Dataframe(MatrisMul(Inverse(MatrisMul(Conjugate(A), A)), Conjugate(A)).GetValues(),
                                             df.Delimiter,
                                             df.NewLine,
                                             Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                             Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                             df.GetRowSettings().Copy(),
                                             df.GetColSettings().Copy())
                        : MatrisMul(Inverse(MatrisMul(Conjugate(A), A)), Conjugate(A));
                }
                catch (Exception err)
                {
                    if (err.Message == CompilerMessage.MAT_DET_ZERO_NO_INV)
                    {
                        throw new Exception(CompilerMessage.MAT_PSEINV_DET_ZERO(sidename));
                    }

                    throw new Exception("Genelleştirilmiş ters matris hatası:\n", err);
                }
            }
            else  // Right inverse
            {
                try
                {
                    return A is Dataframe df
                        ? new Dataframe(MatrisMul(Conjugate(A), Inverse(MatrisMul(A, Conjugate(A)))).GetValues(),
                                             df.Delimiter,
                                             df.NewLine,
                                             Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                             Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                             df.GetRowSettings().Copy(),
                                             df.GetColSettings().Copy())
                        : MatrisMul(Conjugate(A), Inverse(MatrisMul(A, Conjugate(A))));
                }
                catch (Exception err)
                {
                    if (err.Message == CompilerMessage.MAT_DET_ZERO_NO_INV)
                    {
                        throw new Exception(CompilerMessage.MAT_PSEINV_DET_ZERO(sidename));
                    }

                    throw new Exception("Genelleştirilmiş ters matris hatası:\n", err);
                }
            }
        }

        public MatrisBase<object> Adjoint(MatrisBase<object> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            CompilerUtils.AssertMatrixValsAreNumbers(A);

            List<List<object>> adj = new List<List<object>>();
            int r = A.Row;
            int c = A.Col;
            for (int i = 0; i < r; i++)
            {
                adj.Add(new List<object>());
                for (int j = 0; j < c; j++)
                {
                    adj[i].Add((dynamic)(((i + j) % 2 == 1 ? -1 : 1) * Minor(A, i, j, 0)));
                }
            }

            return A is Dataframe df
                ? new Dataframe(Transpose(new MatrisBase<object>(adj)).GetValues(),
                                     df.Delimiter,
                                     df.NewLine,
                                     Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                     Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : Transpose(new MatrisBase<object>(adj));
        }

        public MatrisBase<object> MatrisMul(MatrisBase<object> A,
                                            MatrisBase<object> B)
        {
            CompilerUtils.AssertMatrixValsAreNumbers(A);
            CompilerUtils.AssertMatrixValsAreNumbers(B);

            return A is Dataframe df
                ? new Dataframe(CompilerUtils.MatrisMul(A, B).GetValues(),
                                df.Delimiter,
                                df.NewLine,
                                null,
                                null,
                                df.GetRowSettings().Copy(),
                                df.GetColSettings().Copy())
                : CompilerUtils.MatrisMul(A, B);
        }

        public MatrisBase<object> Concatenate(MatrisBase<object> A,
                                              MatrisBase<object> B,
                                              int axis = 0)
        {
            if (axis == 0)
            {
                if (A.Col != B.Col)
                {
                    throw new Exception(CompilerMessage.MAT_CONCAT_DIM_ERROR("Satır"));
                }

                List<List<object>> newvals = new List<List<object>>();

                for (int r1 = 0; r1 < A.Row; r1++)
                {
                    newvals.Add(A.RowList(r1, 0));
                }
                for (int r2 = 0; r2 < B.Row; r2++)
                {
                    newvals.Add(B.RowList(r2, 0));
                }

                return A is Dataframe df
                    ? new Dataframe(newvals,
                                         df.Delimiter,
                                         df.NewLine,
                                         null,
                                         null,
                                         df.GetRowSettings().Copy(),
                                         df.GetColSettings().Copy())
                    : new MatrisBase<object>(newvals);
            }
            else if (axis == 1)
            {
                if (A.Row != B.Row)
                {
                    throw new Exception(CompilerMessage.MAT_CONCAT_DIM_ERROR("Sütun"));
                }

                List<List<object>> newvals = new List<List<object>>();

                for (int r = 0; r < A.Row; r++)
                {
                    newvals.Add(A.RowList(r, 0));
                    newvals[r].AddRange(B.RowList(r, 0));
                }

                return A is Dataframe df
                    ? new Dataframe(newvals,
                                         df.Delimiter,
                                         df.NewLine,
                                         null,
                                         null,
                                         df.GetRowSettings().Copy(),
                                         df.GetColSettings().Copy())
                    : new MatrisBase<object>(newvals);
            }
            else
            {
                throw new Exception(CompilerMessage.MAT_CONCAT_AXIS_ERROR);
            }
        }

        public float Minor(MatrisBase<object> A,
                           int row,
                           int col,
                           int based = 0)
        {
            return Determinant(MinorMatris(A, row, col, based));
        }

        public MatrisBase<object> MinorMatris(MatrisBase<object> A,
                                              int row,
                                              int col,
                                              int based = 0)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            CompilerUtils.AssertMatrixValsAreNumbers(A);

            List<List<object>> newlis = new List<List<object>>();
            List<List<object>> vals = A.GetValues();
            row -= based;
            col -= based;

            if (row < 0 || row >= A.Row)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", based, A.Row - based));
            }

            if (col < 0 || col >= A.Col)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - based));
            }

            int rowindex = 0;
            for (int i = 0; i < row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < col; j++)
                {
                    newlis[rowindex].Add(vals[i][j]);
                }
                for (int j = col + 1; j < A.Col; j++)
                {
                    newlis[rowindex].Add(vals[i][j]);
                }
                rowindex++;
            }

            for (int i = row + 1; i < A.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < col; j++)
                {
                    newlis[rowindex].Add(vals[i][j]);
                }
                for (int j = col + 1; j < A.Col; j++)
                {
                    newlis[rowindex].Add(vals[i][j]);
                }
                rowindex++;
            }

            return new MatrisBase<object>(newlis);
        }

        public int RowDim(MatrisBase<object> A)
        {
            return A.IsValid() ? A.Row : 0;
        }

        public int ColDim(MatrisBase<object> A)
        {
            return A.IsValid() ? A.Col : 0;
        }

        public MatrisBase<object> Get(MatrisBase<object> A,
                                      int i,
                                      int j,
                                      int based = 0)
        {
            return !A.IsValid()
                ? throw new Exception(CompilerMessage.MAT_INVALID_SIZE)
                : i - based < 0 || i - based >= A.Row
                ? throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", based, A.Row - 1))
                : j - based < 0 || j - based >= A.Col
                ? throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - 1))
                : A is Dataframe df
                    ? new Dataframe(new List<List<object>>() { new List<object>() { A[r: i - based, c: j - based] } },
                                    df.Delimiter,
                                    df.NewLine,
                                    null,
                                    null,
                                    df.GetRowSettings().Copy(),
                                    df.GetColSettings().Copy())
                    : new MatrisBase<object>(new List<List<object>>() { new List<object>() { A[r: i - based, c: j - based] } });
        }

        public MatrisBase<object> Set(MatrisBase<object> A,
                                      int i,
                                      int j,
                                      float value,
                                      int based = 0)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }
            if (i - based < 0 || i - based >= A.Row)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", based, A.Row - 1));
            }
            if (j - based < 0 || j - based >= A.Col)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - 1));
            }

            List<List<object>> newlis = A is Dataframe ? ((Dataframe)A.Copy()).GetValues() : A.Copy().GetValues();
            newlis[i - based][j - based] = (dynamic)value;

            return A is Dataframe df
                ? new Dataframe(newlis,
                                df.Delimiter,
                                df.NewLine,
                                Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                df.GetRowSettings().Copy(),
                                df.GetColSettings().Copy())
                : new MatrisBase<object>(newlis);
        }

        public MatrisBase<object> Row(MatrisBase<object> A,
                                      int i,
                                      int based = 0)
        {
            return !A.IsValid()
                ? throw new Exception(CompilerMessage.MAT_INVALID_SIZE)
                : i - based < 0 || i - based >= A.Row
                ? throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", based, A.Row - 1))
                : A.RowMat(i, based);
        }

        public MatrisBase<object> Col(MatrisBase<object> A,
                                      int j,
                                      int based = 0)
        {
            return !A.IsValid()
                ? throw new Exception(CompilerMessage.MAT_INVALID_SIZE)
                : j - based < 0 || j - based >= A.Col
                ? throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - 1))
                : A.ColMat(j, based);
        }

        public MatrisBase<object> Sub(MatrisBase<object> A,
                                      int r1,
                                      int r2,
                                      int c1,
                                      int c2,
                                      int based = 0)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            r1 -= based;
            r2 -= based;
            if (r2 <= r1 || r1 < 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_ROW_INDICES);
            }

            c1 -= based;
            c2 -= based;
            if (c2 <= c1 || c1 < 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_COL_INDICES);
            }

            return r2 > A.Row
                ? throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", based, A.Row - 1))
                : c2 > A.Col
                ? throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - 1))
                : A is Dataframe df
                ? new Dataframe(A[new Range(r1, r2), new Range(c1, c2)],
                                     df.Delimiter,
                                     df.NewLine,
                                     null,
                                     null,
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : new MatrisBase<object>(A[new Range(r1, r2), new Range(c1, c2)]);
        }

        public MatrisBase<object> Resize(MatrisBase<object> A,
                                         int row,
                                         int col)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            if (A.ElementCount != row * col)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_RESIZE);
            }

            int m = A.Row;
            int n = A.Col;
            List<List<object>> vals = A.GetValues();

            List<List<object>> newlis = new List<List<object>>();

            dynamic val;
            for (int r = 0; r < row; r++)
            {
                newlis.Add(new List<object>());
            }

            for (int r = 0; r < m; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    val = vals[r][c];
                    newlis[((r * n) + c) / col].Add(val);
                }
            }

            return A is Dataframe df
                ? new Dataframe(newlis,
                                     df.Delimiter,
                                     df.NewLine,
                                     null,
                                     null,
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : new MatrisBase<object>(newlis);
        }

        public MatrisBase<object> Sign(MatrisBase<object> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            CompilerUtils.AssertMatrixValsAreNumbers(A);

            int m = A.Row;
            int n = A.Col;
            List<List<object>> vals = A.GetValues();
            List<List<object>> newvals = new List<List<object>>();
            dynamic val;
            for (int i = 0; i < m; i++)
            {
                newvals.Add(new List<object>());
                for (int j = 0; j < n; j++)
                {
                    val = float.Parse(vals[i][j].ToString());
                    if (val < 0)
                    {
                        newvals[i].Add((dynamic)(-1));
                    }
                    else
                    {
                        newvals[i].Add((dynamic)1);
                    }
                }
            }

            return A is Dataframe df
                ? new Dataframe(newvals,
                                     df.Delimiter,
                                     df.NewLine,
                                     Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                     Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : new MatrisBase<object>(newvals);
        }

        public MatrisBase<object> Abs(MatrisBase<object> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            CompilerUtils.AssertMatrixValsAreNumbers(A);

            int m = A.Row;
            int n = A.Col;
            List<List<object>> vals = A.GetValues();
            List<List<object>> newvals = new List<List<object>>();

            for (int i = 0; i < m; i++)
            {
                newvals.Add(new List<object>());
                for (int j = 0; j < n; j++)
                {
                    newvals[i].Add((dynamic)Math.Abs(float.Parse(vals[i][j].ToString())));
                }
            }

            return A is Dataframe df
                ? new Dataframe(newvals,
                                     df.Delimiter,
                                     df.NewLine,
                                     Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                     Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : new MatrisBase<object>(newvals);
        }

        public MatrisBase<object> Round(MatrisBase<object> A,
                                        int n = 0)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            CompilerUtils.AssertMatrixValsAreNumbers(A);

            return n < 0
                ? throw new Exception(CompilerMessage.ARG_INVALID_VALUE("n", " Sıfırdan büyük olmalı."))
                : A is Dataframe df
                ? new Dataframe(A.Round(n).GetValues(),
                                     df.Delimiter,
                                     df.NewLine,
                                     Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                     Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : A.Round(n);
        }

        public MatrisBase<object> Shuffle(MatrisBase<object> A,
                                          int axis = 2)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            int m = A.Row;
            int n = A.Col;

            if (m == 1 && n == 1)
            {
                return A is Dataframe ? ((Dataframe)A.Copy()) : A.Copy();
            }

            if (axis == 0)
            {
                if (m == 1)
                {
                    return A is Dataframe ? ((Dataframe)A.Copy()) : A.Copy();
                }

                List<int> indices = new List<int>();
                for (int c = 0; c < m; c++)
                {
                    indices.Add(c);
                }

                indices = indices.OrderBy(x => Guid.NewGuid()).ToList();
                List<List<object>> newvals = new List<List<object>>();
                List<List<object>> vals = A.GetValues();

                int i = 0;
                foreach (int k in indices)
                {
                    newvals.Add(new List<object>());
                    for (int j = 0; j < n; j++)
                    {
                        newvals[i].Add(vals[k][j]);
                    }
                    i++;
                }

                return A is Dataframe data
                    ? new Dataframe(newvals,
                                    data.Delimiter,
                                    data.NewLine,
                                    null,
                                    Dataframe.GetCopyOfLabels(data.GetColLabels()),
                                    null,
                                    data.GetColSettings().Copy(),
                                    true)
                    : new MatrisBase<object>(newvals);
            }
            else if (axis == 1)
            {
                if (n == 1)
                {
                    return A is Dataframe ? ((Dataframe)A.Copy()) : A.Copy();
                }

                List<int> indices = new List<int>();
                for (int c = 0; c < n; c++)
                {
                    indices.Add(c);
                }

                indices = indices.OrderBy(x => Guid.NewGuid()).ToList();
                List<List<object>> newvals = new List<List<object>>();
                List<List<object>> vals = A.GetValues();

                for (int i = 0; i < m; i++)
                {
                    newvals.Add(new List<object>());
                    foreach (int k in indices)
                    {
                        newvals[i].Add(vals[i][k]);
                    }
                }

                return A is Dataframe data
                    ? new Dataframe(newvals,
                                    data.Delimiter,
                                    data.NewLine,
                                    Dataframe.GetCopyOfLabels(data.GetRowLabels()),
                                    null,
                                    data.GetRowSettings().Copy(),
                                    null,
                                    true)
                    : new MatrisBase<object>(newvals);
            }
            else if (axis == 2)
            {
                if (m == 1)
                {
                    return Shuffle(A, 1);
                }
                else if (n == 1)
                {
                    return Shuffle(A, 0);
                }

                List<int> indices = new List<int>();
                for (int k = 0; k < n * m; k++)
                {
                    indices.Add(k);
                }

                indices = indices.OrderBy(x => Guid.NewGuid()).ToList();
                List<List<object>> newvals = new List<List<object>>();
                List<List<object>> vals = A.GetValues();

                int c = 0;
                int r = -1;
                foreach (int k in indices)
                {
                    if (c % n == 0)
                    {
                        newvals.Add(new List<object>());
                        r++;
                    }

                    newvals[r].Add(vals[k / n][k % n]);
                    c++;
                }

                return A is Dataframe data
                    ? new Dataframe(newvals,
                                    data.Delimiter,
                                    data.NewLine,
                                    forceLabelsWhenNull: true)
                    : new MatrisBase<object>(newvals);
            }
            else
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("axis", "Satır: 0, Sütun: 1, Rastgele:2 olmalı"));
            }

        }

        public MatrisBase<object> Replace(MatrisBase<object> A,
                                          dynamic old = null,
                                          dynamic with = null,
                                          float TOL = (float)1e-6)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            if (TOL < 0)
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("TOL", " Sıfırdan büyük-eşit olmalı."));
            }

            List<List<object>> newVals = new List<List<object>>();

            int m = A.Row;
            int n = A.Col;
            List<List<object>> vals = A.GetValues();

            if (with is null)
            {
                with = new None();
            }

            if (old is null || old is None)
            {
                for (int i = 0; i < m; i++)
                {
                    newVals.Add(new List<object>());
                    for (int j = 0; j < n; j++)
                    {
                        if (vals[i][j] is null || vals[i][j] is None)
                        {
                            newVals[i].Add(with);
                        }
                        else
                        {
                            newVals[i].Add(vals[i][j]);
                        }
                    }
                }
            }
            else if (float.TryParse(((object)old).ToString(), out float search))
            {
                if (float.IsNaN(search))
                {
                    for (int i = 0; i < m; i++)
                    {
                        newVals.Add(new List<object>());
                        for (int j = 0; j < n; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float found)
                                && float.IsNaN(found))
                            {
                                newVals[i].Add(with);
                            }
                            else
                            {
                                newVals[i].Add(vals[i][j]);
                            }
                        }
                    }
                }
                else if (float.IsNegativeInfinity(search))
                {
                    for (int i = 0; i < m; i++)
                    {
                        newVals.Add(new List<object>());
                        for (int j = 0; j < n; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float found)
                                && float.IsNegativeInfinity(found))
                            {
                                newVals[i].Add(with);
                            }
                            else
                            {
                                newVals[i].Add(vals[i][j]);
                            }
                        }
                    }
                }
                else if (float.IsPositiveInfinity(search))
                {
                    for (int i = 0; i < m; i++)
                    {
                        newVals.Add(new List<object>());
                        for (int j = 0; j < n; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float found)
                                && float.IsPositiveInfinity(found))
                            {
                                newVals[i].Add(with);
                            }
                            else
                            {
                                newVals[i].Add(vals[i][j]);
                            }
                        }
                    }
                }
                else
                {
                    float max = search + TOL;
                    float min = search - TOL;
                    for (int i = 0; i < m; i++)
                    {
                        newVals.Add(new List<object>());
                        for (int j = 0; j < n; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float found)
                                && (found <= max)
                                && (found >= min))
                            {
                                newVals[i].Add(with);
                            }
                            else
                            {
                                newVals[i].Add(vals[i][j]);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < m; i++)
                {
                    newVals.Add(new List<object>());
                    for (int j = 0; j < n; j++)
                    {
                        if (!(vals[i][j] is None) && !(vals[i][j] is null) && vals[i][j].ToString() == ((object)old).ToString())
                        {
                            newVals[i].Add(with);
                        }
                        else
                        {
                            newVals[i].Add(vals[i][j]);
                        }
                    }
                }
            }

            return A is Dataframe df
                ? new Dataframe(newVals,
                                     df.Delimiter,
                                     df.NewLine,
                                     Dataframe.GetCopyOfLabels(df.GetRowLabels()),
                                     Dataframe.GetCopyOfLabels(df.GetColLabels()),
                                     df.GetRowSettings().Copy(),
                                     df.GetColSettings().Copy())
                : new MatrisBase<object>(newVals);
        }
        #endregion
    }
}
