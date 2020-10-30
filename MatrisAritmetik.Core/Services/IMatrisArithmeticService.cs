using System;
using System.Collections.Generic;
using System.Text;

namespace MatrisAritmetik.Core.Services
{
    public interface IMatrisArithmeticService<T>
    {
        MatrisBase<T> Transpose(MatrisBase<T> A);

        MatrisBase<T> Conjugate(MatrisBase<T> A);

        MatrisBase<T> Echelon(MatrisBase<T> A, bool row_reduce=false);

        float Determinant(MatrisBase<T> A);

        float Rank(MatrisBase<T> A);

        MatrisBase<T> Adjoint(MatrisBase<T> A);

        MatrisBase<T> Inverse(MatrisBase<T> A);

        MatrisBase<T> PseudeInverse(MatrisBase<T> A);

        MatrisBase<T> MatrisMul(MatrisBase<T> A, MatrisBase<T> B);

        MatrisBase<T> Concat(MatrisBase<T> A, MatrisBase<T> B, string concat_as = "row");

        float Minor(MatrisBase<T> A,int row, int col);

        MatrisBase<T> MinorMatris(MatrisBase<T> A, int row, int col);


    }
}
