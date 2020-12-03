using System;
using System.Collections.Generic;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class MatrisArithmeticService<T> : IMatrisArithmeticService<T>
    {
        #region Private Generic Operations
        /// <summary>
        /// Parses <paramref name="a"/> and <paramref name="b"/> as <see cref="float"/>s and adds them together
        /// </summary>
        /// <param name="a">Any numerically parsable value</param>
        /// <param name="b">Any numerically parsable value</param>
        /// <returns>Result of <paramref name="a"/>+<paramref name="b"/> parsed as <see cref="float"/>s</returns>
        private T Add(T a, T b)
        {
            return (dynamic)(float.Parse(a.ToString()) + float.Parse(b.ToString()));
        }

        /// <summary>
        /// Parses <paramref name="a"/> and <paramref name="b"/> as <see cref="float"/>s and multiplies them together
        /// </summary>
        /// <param name="a">Any numerically parsable value</param>
        /// <param name="b">Any numerically parsable value</param>
        /// <returns>Result of <paramref name="a"/>*<paramref name="b"/> parsed as <see cref="float"/>s</returns>
        private T Mul(T a, T b)
        {
            return (dynamic)(float.Parse(a.ToString()) * float.Parse(b.ToString()));
        }

        /// <summary>
        /// Dot product of given lists <paramref name="a"/> and <paramref name="b"/>
        /// <para>Values in <paramref name="a"/> and <paramref name="b"/> are parsed as <see cref="float"/>s during calculation</para>
        /// </summary>
        /// <param name="a">List of numerically parsable values</param>
        /// <param name="b">List of numerically parsable values</param>
        /// <returns>Dot product casted as <typeparamref name="T"/></returns>
        private T DotProduct(List<T> a,
                             List<T> b)
        {
            if (a.Count != b.Count)
            {
                throw new Exception("Boyutlar hatalı");
            }

            if (a.Count == 0)
            {
                throw new Exception("Boyut sıfır");
            }

            T res = Mul(a[0], b[0]);

            for (int i = 1; i < a.Count; i++)
            {
                res = Add(res, Mul(a[i], b[i]));
            }

            return res;
        }
        #endregion

        #region MatrisArithmeticService Methods

        public MatrisBase<T> Transpose(MatrisBase<T> A)
        {
            List<List<T>> result = new List<List<T>>();

            for (int j = 0; j < A.Col; j++)
            {
                result.Add(A.ColList(j, 0));
            }

            return new MatrisBase<T>(result);
        }

        public MatrisBase<T> Conjugate(MatrisBase<T> A)
        {
            return Transpose(A);
        }

        public MatrisBase<T> Echelon(MatrisBase<T> A)
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

            MatrisBase<T> result;

            int nr = A.Row;
            int nc = A.Col;

            List<int> zeroCols = new List<int>();
            List<List<T>> filteredResult = A.Copy().Values;
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
            result = new MatrisBase<T>(filteredResult);

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
                while ((float.Parse(result.Values[p][p].ToString())) == (float)0.0)
                {
                    if (p + 1 < nr)
                    {
                        if (result.IsZeroRow(p, 0, (float)0.0))
                        {
                            nr--;
                        }

                        result.SwapToEnd(p, 0);
                        next = true;
                        swapCount++;

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
                                    result[i].Insert(j, (dynamic)(float)0.0);
                                }
                            }
                            result.SetCol(A.Col);
                        }
                        result.FixMinusZero();
                        result.swapCount = swapCount;
                        return result;
                    }
                }

                if (next)
                {
                    continue;
                }

                for (; (r >= 1 && r < (nr - p)); r++)
                {
                    if (float.Parse(result.Values[p + r][p].ToString()) != (float)0.0)
                    {
                        float x = -(float.Parse(result.Values[p + r][p].ToString()) / float.Parse(result.Values[p][p].ToString()));
                        for (int c = p; c < nc; c++)
                        {
                            result.Values[p + r][c] = (dynamic)((float.Parse(result.Values[p][c].ToString()) * x) + float.Parse(result.Values[p + r][c].ToString()));
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
                        result[i].Insert(j, (dynamic)(float)0.0);
                    }
                }
                result.SetCol(A.Col);
            }

            result.FixMinusZero();
            result.swapCount = swapCount;
            return result;
        }

        public MatrisBase<T> RREchelon(MatrisBase<T> A)
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

            MatrisBase<T> result = Echelon(A);

            int rowCount = A.Row;
            while (result.IsZeroRow(rowCount, 1, (float)0.0))
            {
                rowCount--;
            }

            if (rowCount < 1)
            {
                return A;
            }

            int colCount = A.Col;

            for (int i = rowCount - 1; i >= 0; i--)
            {
                if (result.IsZeroRow(i, 0, (float)0.0))
                {
                    continue;
                }

                int pivotindex = 0;
                while (float.Parse(result.Values[i][pivotindex].ToString()) == (float)0.0)
                {
                    pivotindex++;
                }

                result.MulRow(i, (float)1.0 / float.Parse(result.Values[i][pivotindex].ToString()), 0);

                for (int e = i - 1; e >= 0; e--)
                {
                    if (float.Parse(result.Values[e][pivotindex].ToString()) == (float)0.0)
                    {
                        continue;
                    }

                    float factor = -float.Parse(result.Values[e][pivotindex].ToString());
                    for (int j = pivotindex; j < colCount; j++)
                    {
                        result.Values[e][j] = (dynamic)(float.Parse(result.Values[e][j].ToString()) + (float.Parse(result.Values[i][j].ToString()) * factor));
                    }
                }
            }

            result.FixMinusZero();

            return result;
        }

        public float Determinant(MatrisBase<T> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            if (A.Row == 1)
            {
                return float.Parse(A.Values[0][0].ToString());
            }

            if (A.Row == 2)
            {
                return (float.Parse(A.Values[0][0].ToString()) * float.Parse(A.Values[1][1].ToString()))
                       - (float.Parse(A.Values[0][1].ToString()) * float.Parse(A.Values[1][0].ToString()));
            }

            MatrisBase<T> ech = Echelon(A);

            float det = float.Parse(ech.Values[0][0].ToString());
            if (ech.swapCount % 2 == 1)
            {
                det *= -1;
            }

            int dim = A.Row;

            for (int i = 1; i < dim; i++)
            {
                det *= float.Parse(ech.Values[i][i].ToString());
            }
            return det;
        }

        public int Rank(MatrisBase<T> A)
        {
            MatrisBase<T> ech = Echelon(A);
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

        public MatrisBase<T> Inverse(MatrisBase<T> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            if (Determinant(A) == (float)(0.0))
            {
                throw new Exception(CompilerMessage.MAT_DET_ZERO_NO_INV);
            }

            MatrisBase<T> temp = Concatenate(A.Copy(), (dynamic)((ISpecialMatricesService)new SpecialMatricesService()).Identity(A.Row), 1);

            return new MatrisBase<T>(vals: RREchelon(temp)[new Range(new Index(0), new Index(temp.Row)), new Range(new Index(A.Col), new Index(temp.Col))]);
        }

        public MatrisBase<T> PseudoInverse(MatrisBase<T> A,
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

            string sidename = side == -1 ? "sol" : "sağ";

            // Left inverse
            if (side == -1)
            {
                try
                {
                    return MatrisMul(Inverse(MatrisMul(Conjugate(A), A)), Conjugate(A));
                }
                catch (Exception err)
                {
                    if (err.Message == CompilerMessage.MAT_DET_ZERO_NO_INV)
                    {
                        throw new Exception(CompilerMessage.MAT_PSEINV_DET_ZERO(sidename));
                    }

                    throw err;
                }
            }
            else  // Right inverse
            {
                try
                {
                    return MatrisMul(Conjugate(A), Inverse(MatrisMul(A, Conjugate(A))));
                }
                catch (Exception err)
                {
                    if (err.Message == CompilerMessage.MAT_DET_ZERO_NO_INV)
                    {
                        throw new Exception(CompilerMessage.MAT_PSEINV_DET_ZERO(sidename));
                    }

                    throw err;
                }
            }
        }

        public MatrisBase<T> Adjoint(MatrisBase<T> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            List<List<T>> adj = new List<List<T>>();
            int r = A.Row;
            int c = A.Col;
            for (int i = 0; i < r; i++)
            {
                adj.Add(new List<T>());
                for (int j = 0; j < c; j++)
                {
                    adj[i].Add((dynamic)(((i + j) % 2 == 1 ? -1 : 1) * Minor(A, i, j, 0)));
                }
            }
            return Transpose(new MatrisBase<T>(adj));
        }

        public MatrisBase<T> MatrisMul(MatrisBase<T> A, MatrisBase<T> B)
        {
            if (A.Col != B.Row)
            {
                throw new Exception(CompilerMessage.MAT_MUL_BAD_SIZE);
            }

            List<List<T>> result = new List<List<T>>();

            for (int i = 0; i < A.Row; i++)
            {
                result.Add(new List<T>());
                for (int j = 0; j < B.Col; j++)
                {
                    result[i].Add(DotProduct(A.Values[i], B.ColList(j, 0)));
                }
            }

            return new MatrisBase<T>(result);

        }

        public MatrisBase<T> Concatenate(MatrisBase<T> A,
                                         MatrisBase<T> B,
                                         int axis = 0)
        {
            if (axis == 0)
            {
                if (A.Col != B.Col)
                {
                    throw new Exception(CompilerMessage.MAT_CONCAT_DIM_ERROR("Satır"));
                }

                List<List<T>> newvals = new List<List<T>>();

                for (int r1 = 0; r1 < A.Row; r1++)
                {
                    newvals.Add(A.RowList(r1, 0));
                }
                for (int r2 = 0; r2 < B.Row; r2++)
                {
                    newvals.Add(B.RowList(r2, 0));
                }

                return new MatrisBase<T>() { Row = A.Row + B.Row, Col = A.Col, Values = newvals };
            }
            else if (axis == 1)
            {
                if (A.Row != B.Row)
                {
                    throw new Exception(CompilerMessage.MAT_CONCAT_DIM_ERROR("Sütun"));
                }

                List<List<T>> newvals = new List<List<T>>();

                for (int r = 0; r < A.Row; r++)
                {
                    newvals.Add(A.RowList(r, 0));
                    newvals[r].AddRange(B.RowList(r, 0));
                }

                return new MatrisBase<T>() { Row = A.Row, Col = A.Col + B.Col, Values = newvals };

            }
            else
            {
                throw new Exception(CompilerMessage.MAT_CONCAT_AXIS_ERROR);
            }
        }

        public float Minor(MatrisBase<T> A,
                           int row,
                           int col,
                           int based = 0)
        {
            return Determinant(MinorMatris(A, row, col, based));
        }

        public MatrisBase<T> MinorMatris(MatrisBase<T> A,
                                         int row,
                                         int col,
                                         int based = 0)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            List<List<T>> newlis = new List<List<T>>();
            List<List<T>> vals = A.Values;
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
                newlis.Add(new List<T>());
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
                newlis.Add(new List<T>());
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

            return new MatrisBase<T>(newlis);
        }

        #endregion
    }
}
