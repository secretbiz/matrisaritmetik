using System;
using System.Collections.Generic;
using System.Linq;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class MatrisArithmeticService<T> : IMatrisArithmeticService<T>
    {
        #region MatrisArithmeticService Methods

        public MatrisBase<T> Transpose(MatrisBase<T> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

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
            List<List<T>> filteredResult = A.Copy().GetValues();
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
                while (float.Parse(result.GetValues()[p][p].ToString()) == (float)0.0)
                {
                    if (p + 1 < nr)
                    {
                        if (result.IsZeroRow(p, 0, (float)0.0))
                        {
                            nr--;
                        }

                        if (!Sub(result, p, nr, p, p + 1, 0).IsZeroCol(1))
                        {
                            result.SwapToEnd(p, 0);
                            swapCount++;
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
                                    result[i].Insert(j, (dynamic)(float)0.0);
                                }
                            }
                            result.SetCol(A.Col);
                        }
                        result.FixMinusZero();
                        result.SwapCount = swapCount;
                        return result;
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
                            result[p + r][c] = (dynamic)((float.Parse(result.GetValues()[p][c].ToString()) * x) + float.Parse(result.GetValues()[p + r][c].ToString()));
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
            result.SwapCount = swapCount;
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

            MatrisBase<T> result = A.Copy();

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

            return result;
        }

        public float Determinant(MatrisBase<T> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            if (A.IsZero((float)0.0))
            {
                return (float)0.0;
            }

            if (A.Row == 1)
            {
                return float.Parse(A.GetValues()[0][0].ToString());
            }

            if (A.Row == 2)
            {
                return (float.Parse(A.GetValues()[0][0].ToString()) * float.Parse(A.GetValues()[1][1].ToString()))
                       - (float.Parse(A.GetValues()[0][1].ToString()) * float.Parse(A.GetValues()[1][0].ToString()));
            }

            using MatrisBase<T> ech = Echelon(A.Copy());

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

        public int Rank(MatrisBase<T> A)
        {
            using MatrisBase<T> ech = Echelon(A.Copy());
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

        public float Trace(MatrisBase<T> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID);
            }
            float trace = float.Parse(A[0, 0].ToString());
            int m = Math.Min(A.Row, A.Col);
            for (int i = 1; i < m; i++)
            {
                trace += float.Parse(A[i, i].ToString());
            }

            return trace;
        }

        public MatrisBase<T> Inverse(MatrisBase<T> A)
        {
            if (!A.IsSquare())
            {
                throw new Exception(CompilerMessage.MAT_NOT_SQUARE);
            }

            if (Determinant(A) == (float)0.0)
            {
                throw new Exception(CompilerMessage.MAT_DET_ZERO_NO_INV);
            }

            using MatrisBase<T> temp = Concatenate(A.Copy(), (dynamic)((ISpecialMatricesService)new SpecialMatricesService()).Identity(A.Row), 1);

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

                    throw new Exception("Genelleştirilmiş ters matris hatası:\n", err);
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

                    throw new Exception("Genelleştirilmiş ters matris hatası:\n", err);
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

        public MatrisBase<T> MatrisMul(MatrisBase<T> A,
                                       MatrisBase<T> B)
        {
            return CompilerUtils.MatrisMul(A, B);
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

                MatrisBase<T> res = new MatrisBase<T>() { Row = A.Row + B.Row, Col = A.Col };
                res.SetValues(newvals);
                return res;
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

                MatrisBase<T> res = new MatrisBase<T>() { Row = A.Row, Col = A.Col + B.Col };
                res.SetValues(newvals);
                return res;
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
            List<List<T>> vals = A.GetValues();
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

        public int RowDim(MatrisBase<T> A)
        {
            return A.IsValid() ? A.Row : 0;
        }

        public int ColDim(MatrisBase<T> A)
        {
            return A.IsValid() ? A.Col : 0;
        }

        public MatrisBase<T> Get(MatrisBase<T> A,
                                 int i,
                                 int j,
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

            MatrisBase<T> res = new MatrisBase<T>() { Row = 1, Col = 1 };
            res.SetValues(new List<List<T>>() { new List<T>() { A[r: i - based, c: j - based] } });
            return res;
        }

        public MatrisBase<T> Set(MatrisBase<T> A,
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

            List<List<T>> newlis = A.Copy().GetValues();
            newlis[i - based][j - based] = (dynamic)value;

            return new MatrisBase<T>(newlis);
        }

        public MatrisBase<T> Row(MatrisBase<T> A,
                                 int i,
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
            return A.RowMat(i, based);
        }

        public MatrisBase<T> Col(MatrisBase<T> A,
                                 int j,
                                 int based = 0)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }
            if (j - based < 0 || j - based >= A.Col)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - 1));
            }
            return A.ColMat(j, based);
        }

        public MatrisBase<T> Sub(MatrisBase<T> A,
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

            if (r2 >= A.Row)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", based, A.Row - 1));
            }
            if (c2 >= A.Col)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("sütun", based, A.Col - 1));
            }

            return new MatrisBase<T>(A[new Range(r1, r2), new Range(c1, c2)]);
        }

        public MatrisBase<T> Resize(MatrisBase<T> A,
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
            List<List<T>> vals = A.GetValues();

            List<List<T>> newlis = new List<List<T>>();

            dynamic val;
            for (int r = 0; r < row; r++)
            {
                newlis.Add(new List<T>());
            }

            for (int r = 0; r < m; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    val = vals[r][c];
                    newlis[((r * n) + c) / col].Add(val);
                }
            }

            return new MatrisBase<T>(newlis);
        }

        public MatrisBase<T> Sign(MatrisBase<T> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            int m = A.Row;
            int n = A.Col;
            List<List<T>> vals = A.GetValues();
            List<List<T>> newvals = new List<List<T>>();
            dynamic val;
            for (int i = 0; i < m; i++)
            {
                newvals.Add(new List<T>());
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
            return new MatrisBase<T>(newvals);
        }

        public MatrisBase<T> Abs(MatrisBase<T> A)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            int m = A.Row;
            int n = A.Col;
            List<List<T>> vals = A.GetValues();
            List<List<T>> newvals = new List<List<T>>();

            for (int i = 0; i < m; i++)
            {
                newvals.Add(new List<T>());
                for (int j = 0; j < n; j++)
                {
                    newvals[i].Add((dynamic)Math.Abs(float.Parse(vals[i][j].ToString())));
                }
            }
            return new MatrisBase<T>(newvals);
        }

        public MatrisBase<T> Round(MatrisBase<T> A,
                                   int n = 0)
        {
            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            if (n < 0)
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("n", " Sıfırdan büyük olmalı."));
            }

            return A.Round(n);
        }

        public MatrisBase<T> Shuffle(MatrisBase<T> A,
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
                return A.Copy();
            }

            if (axis == 0)
            {
                if (m == 1)
                {
                    return A.Copy();
                }

                List<int> indices = new List<int>();
                for (int c = 0; c < m; c++)
                {
                    indices.Add(c);
                }

                indices = indices.OrderBy(x => Guid.NewGuid()).ToList();
                List<List<T>> newvals = new List<List<T>>();
                List<List<T>> vals = A.GetValues();

                int i = 0;
                foreach (int k in indices)
                {
                    newvals.Add(new List<T>());
                    for (int j = 0; j < n; j++)
                    {
                        newvals[i].Add(vals[k][j]);
                    }
                    i++;
                }

                return new MatrisBase<T>(newvals);
            }
            else if (axis == 1)
            {
                if (n == 1)
                {
                    return A.Copy();
                }

                List<int> indices = new List<int>();
                for (int c = 0; c < n; c++)
                {
                    indices.Add(c);
                }

                indices = indices.OrderBy(x => Guid.NewGuid()).ToList();
                List<List<T>> newvals = new List<List<T>>();
                List<List<T>> vals = A.GetValues();

                for (int i = 0; i < m; i++)
                {
                    newvals.Add(new List<T>());
                    foreach (int k in indices)
                    {
                        newvals[i].Add(vals[i][k]);
                    }
                }

                return new MatrisBase<T>(newvals);
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
                List<List<T>> newvals = new List<List<T>>();
                List<List<T>> vals = A.GetValues();

                int c = 0;
                int r = -1;
                foreach (int k in indices)
                {
                    if (c % n == 0)
                    {
                        newvals.Add(new List<T>());
                        r++;
                    }

                    newvals[r].Add(vals[k / n][k % n]);
                    c++;
                }

                return new MatrisBase<T>(newvals);
            }
            else
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("axis", "Satır: 0, Sütun: 1, Rastgele:2 olmalı"));
            }

        }

        public MatrisBase<T> Replace(MatrisBase<T> A,
                                     float old,
                                     float with,
                                     float TOL = (float)1e-6)
        {
            static bool inRange(float num, float min, float max)
            {
                return (num <= max) && (num >= min);
            }

            if (!A.IsValid())
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            if (TOL < 0)
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("TOL", " Sıfırdan büyük-eşit olmalı."));
            }

            List<List<T>> newVals = new List<List<T>>();

            int m = A.Row;
            int n = A.Col;
            List<List<T>> vals = A.GetValues();

            dynamic val;
            float max = old + TOL;
            float min = old - TOL;

            for (int i = 0; i < m; i++)
            {
                newVals.Add(new List<T>());
                for (int j = 0; j < n; j++)
                {
                    val = (dynamic)float.Parse(vals[i][j].ToString());
                    if (inRange(val, min, max))
                    {
                        newVals[i].Add((dynamic)with);
                    }
                    else
                    {
                        newVals[i].Add(val);
                    }
                }
            }

            return new MatrisBase<T>(newVals);
        }
        #endregion
    }
}
