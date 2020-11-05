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

        public MatrisBase<T> Echelon(MatrisBase<T> A, bool row_reduce = false)
        {
            return new MatrisBase<T>();
        }

        public float Determinant(MatrisBase<T> A)
        {
            return (float)0.0;
        }

        public float Rank(MatrisBase<T> A)
        {
            return (float)0.0;
        }

        public MatrisBase<T> Inverse(MatrisBase<T> A)
        {
            return new MatrisBase<T>();
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
