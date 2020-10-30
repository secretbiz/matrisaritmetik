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
            dynamic a1 = a;dynamic b1 = b;
            return (a1 + b1);
        }

        private T Mul(T a, T b)
        {
            dynamic a1 = a; dynamic b1 = b;
            return (a1 * b1);
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

            for (int j = 0; j < A.col; j++)
            {
                result.Add(A.Col(j,0));
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
            if (A.col != B.row)
            {
                throw new Exception("Satır ve sütun boyutları hatalı");
            }

            List<List<T>> result = new List<List<T>>();

            for (int i = 0; i < A.row; i++)
            {
                result.Add(new List<T>());
                for (int j = 0; j < B.col; j++)
                {
                    result[i].Add(DotProduct(A.values[i], B.Col(j, 0)));
                }
            }

            return new MatrisBase<T>(result);

        }

        public MatrisBase<T> Concat(MatrisBase<T> A, MatrisBase<T> B, string concat_as = "row")
        {
            if(concat_as == "row")
            {
                if (A.col != B.col)
                {
                    throw new Exception("Column dimensions don't match for concatenation");
                }

                List<List<T>> newvals = new List<List<T>>();

                for (int r1 = 0; r1 < A.row; r1++)
                {
                    newvals.Add(A.Row(r1, 0));
                }
                for (int r2 = 0; r2 < B.row; r2++)
                {
                    newvals.Add(B.Row(r2, 0));
                }

                return new MatrisBase<T>() { row = A.row + B.row, col = A.col, values = newvals };
            }
            else if(concat_as == "col")
            {
                if (A.row != B.row)
                {
                    throw new Exception("Row dimensions don't match for concatenation");
                }

                List<List<T>> newvals = new List<List<T>>();

                for (int r = 0; r < A.row; r++)
                {
                    newvals.Add(A.Row(r, 0));
                    newvals[r].AddRange(B.Row(r, 0));
                }

                return new MatrisBase<T>() { row = A.row, col = A.col + B.col, values = newvals };
                
            }
            else
            {
                throw new Exception("concat_as should be 'row' or 'col'");
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
