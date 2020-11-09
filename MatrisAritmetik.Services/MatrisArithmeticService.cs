using System;
using System.Collections.Generic;
using System.Text;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class MatrisArithmeticService<T>: IMatrisArithmeticService<T>
    {
        /*
         * Operations for generics
         */
        private T Add(T a, T b)
        {
            return (dynamic)(float.Parse(a.ToString()) + float.Parse(b.ToString()));
        }

        private T Mul(T a, T b)
        {
            return (dynamic)(float.Parse(a.ToString()) * float.Parse(b.ToString()));
        }

        private T DotProduct(List<T> a, List<T> b)
        {
            if (a.Count != b.Count)
                throw new Exception("Boyutlar hatalı");

            if (a.Count == 0)
                throw new Exception("Boyut sıfır");
            
            T res = Mul(a[0],b[0]);
            
            for(int i = 1; i <a.Count; i++)
            {
                res = Add(res, Mul(a[i], b[i]));
            }

            return res;
        }

        /*
         * Functions with single MatrisBase param
         */
        public MatrisBase<T> Transpose(MatrisBase<T> A)
        {
            List<List<T>> result = new List<List<T>>();

            for (int j = 0; j < A.Col; j++)
            {
                result.Add(A.ColList(j,0));
            }

            return new MatrisBase<T>(result);
        }

        public MatrisBase<T> Conjugate(MatrisBase<T> A)
        {
            return A.Copy();
        }

        public int AbsMaxOfList(List<T> lis)
        {
            if (lis.Count == 0)
                throw new Exception("Liste boş!");

            if (lis.Count == 1)
                return 0;

            int currentmax = 0;
            for(int i = 1; i < lis.Count; i++)
            {
                if (Math.Abs(float.Parse(lis[i].ToString())) > Math.Abs(float.Parse(lis[currentmax].ToString())))
                    currentmax = i;
            }
            return currentmax;
        }

        public T MinOfList(List<T> lis)
        {
            if (lis.Count == 0)
                throw new Exception("Liste boş!");

            if (lis.Count == 1)
                return lis[0];

            T currentmin = lis[0];
            foreach (dynamic val in lis.GetRange(1, lis.Count - 1))
            {
                if ((float.Parse(val.ToString()) < (float.Parse(currentmin.ToString()))))
                    currentmin = val;
            }
            return currentmin;
        }

        public MatrisBase<T> Echelon(MatrisBase<T> A)
        {
            // Bad dimensions
            if (!A.IsValid())
                throw new Exception("Matris boyutları uygun değil!");

            // Zero matrix
            if (A.IsZero((float)0.0))
                return A;

            MatrisBase<T> result;

            int nr = A.Row;
            int nc = A.Col;

            List<int> zeroCols = new List<int>();
            List<List<T>> filteredResult = A.Copy().Values; 
            for(int j=0;j<nc;j++)
            {
                if (A.IsZeroCol(j, 0, (float)0.0))
                {
                    for (int i = 0; i < nr; i++)
                        filteredResult[i].RemoveAt(j - zeroCols.Count);
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

            while( p < nr && p < nc)
            {
                next = false;
                int r = 1;
                while ((float.Parse(result.Values[p][p].ToString())) == (float)0.0)
                {
                    if(p + 1 < nr )
                    {
                        if (result.IsZeroRow(p, 0, (float)0.0))
                            nr--;
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
                                result.SwapToEnd(i, 0);
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
                    continue;

                for(;(r>=1 && r< (nr-p)); r++)
                {
                    if(float.Parse(result.Values[p+r][p].ToString()) != (float)0.0)
                    {
                        float x = -(float.Parse(result.Values[p + r][p].ToString()) / float.Parse(result.Values[p][p].ToString()));
                        for (int c = p; c < nc; c++)
                            result.Values[p + r][c] = (dynamic)(float.Parse(result.Values[p][c].ToString()) * x + float.Parse(result.Values[p + r][c].ToString())); 
                    }
                }
                p++;
            }

            // Zeros to bottom
            for (int i = 0; i < A.Row; i++)
            {
                if (result.IsZeroRow(i, 0, (float)0.0))
                    result.SwapToEnd(i, 0);
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
                throw new Exception("Matris boyutları uygun değil!");

            // Zero matrix
            if (A.IsZero((float)0.0))
                return A;

            MatrisBase<T> result = Echelon(A);

            int rowCount = A.Row;
            while (result.IsZeroRow(rowCount, 1, (float)0.0))
                rowCount--;
            
            if (rowCount < 1)
                return A;

            int colCount = A.Col;

            for(int i= rowCount-1 ; i>=0;i--)
            {
                if (result.IsZeroRow(i, 0, (float)0.0))
                    continue;

                int pivotindex = 0;
                while (float.Parse(result.Values[i][pivotindex].ToString()) == (float)0.0)
                    pivotindex++;

                result.MulRow(i, (float)1.0/float.Parse(result.Values[i][pivotindex].ToString()), 0);

                for(int e = i-1; e >= 0;e--)
                {
                    if (float.Parse(result.Values[e][pivotindex].ToString()) == (float)0.0)
                        continue;

                    float factor = -float.Parse(result.Values[e][pivotindex].ToString());
                    for (int j = pivotindex; j < colCount; j++)
                        result.Values[e][j] = (dynamic)(float.Parse(result.Values[e][j].ToString()) + float.Parse(result.Values[i][j].ToString()) * factor);
                }
            }

            result.FixMinusZero();

            return result;
        }

        public float Determinant(MatrisBase<T> A)
        {
            if (!A.IsSquare())
                throw new Exception("Determinant hesabı için matris kare olmalı!");

            if (A.Row == 1)
                return float.Parse(A.Values[0][0].ToString());

            if (A.Row == 2)
                return float.Parse(A.Values[0][0].ToString())* float.Parse(A.Values[1][1].ToString())
                       - float.Parse(A.Values[0][1].ToString())* float.Parse(A.Values[1][0].ToString());

            MatrisBase<T> ech = Echelon(A);

            float det = float.Parse(ech.Values[0][0].ToString());
            if (ech.swapCount % 2 == 1)
                det *= -1;

            int dim = A.Row;

            for (int i=1;i<dim;i++)
            {
                det *= float.Parse(ech.Values[i][i].ToString());
            }
            return det;
        }

        public int Rank(MatrisBase<T> A)
        {
            MatrisBase<T> ech = Echelon(A);
            int zeroCount = 0;
            if(A.Row <= A.Col)
            {
                for (int i = ech.Row - 1; i >= 0; i--)
                {
                    if (ech.IsZeroRow(i, 0, (float)0.0))
                        zeroCount++;
                }
                return ech.Row - zeroCount;
            }
            else
            {
                for (int i = ech.Col - 1; i >= 0; i--)
                {
                    if (ech.IsZeroCol(i, 0, (float)0.0))
                        zeroCount++;
                }
                return ech.Col - zeroCount;
            }
        }

        public MatrisBase<T> Inverse(MatrisBase<T> A)
        {
            if (!A.IsSquare())
                throw new Exception("Ters matris hesabı için matris kare olmalı!");

            if (Determinant(A) == (float)(0.0))
                throw new Exception("Determinant 0, ters matris bulunamadı");

            MatrisBase<T> temp = Concatenate(A.Copy(), (dynamic)new SpecialMatricesService().Identity(A.Row),1);
            return new MatrisBase<T>(RREchelon(temp)[new Range(new Index(0), new Index(temp.Row)), new Range(new Index(A.Col), new Index(temp.Col))]);
        }

        public MatrisBase<T> PseudeInverse(MatrisBase<T> A)
        {
            return new MatrisBase<T>();
        }

        public MatrisBase<T> Adjoint(MatrisBase<T> A)
        {
            return new MatrisBase<T>();
        }

        /*
         * Functions with multiple parameters
         */
        public MatrisBase<T> MatrisMul(MatrisBase<T> A, MatrisBase<T> B)
        {
            if (A.Col != B.Row)
            {
                throw new Exception("Satır ve sütun boyutları uyuşmalı");
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

        public MatrisBase<T> Concatenate(MatrisBase<T> A, MatrisBase<T> B, int axis=0)
        {
            if(axis == 0)
            {
                if (A.Col != B.Col)
                {
                    throw new Exception("Column dimensions don't match for concatenation");
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
            else if(axis == 1)
            {
                if (A.Row != B.Row)
                {
                    throw new Exception("Row dimensions don't match for concatenation");
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
                throw new Exception("Axis should be 0 to concat as rows, 1 for cols");
            }
        }

        public float Minor(MatrisBase<T> A, int row, int col)
        {
            return (float)0.0;
        }

        public MatrisBase<T> MinorMatris(MatrisBase<T> A, int row, int col)
        {
            return new MatrisBase<T>();
        }

    }
}
