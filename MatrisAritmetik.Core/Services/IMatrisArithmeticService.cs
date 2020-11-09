using System;
using System.Collections.Generic;
using System.Text;

namespace MatrisAritmetik.Core.Services
{
    public interface IMatrisArithmeticService<T>
    {
        MatrisBase<T> Transpose(MatrisBase<T> A);

        MatrisBase<T> Conjugate(MatrisBase<T> A);

        MatrisBase<T> Echelon(MatrisBase<T> A);

        MatrisBase<T> RREchelon(MatrisBase<T> A);

        float Determinant(MatrisBase<T> A);

        int Rank(MatrisBase<T> A);

        MatrisBase<T> Adjoint(MatrisBase<T> A);

        MatrisBase<T> Inverse(MatrisBase<T> A);

        MatrisBase<T> PseudeInverse(MatrisBase<T> A);

        MatrisBase<T> MatrisMul(MatrisBase<T> A, MatrisBase<T> B);

        MatrisBase<T> Concatenate(MatrisBase<T> A, MatrisBase<T> B, int axis = 0);

        float Minor(MatrisBase<T> A,int row, int col);

        MatrisBase<T> MinorMatris(MatrisBase<T> A, int row, int col);


    }
}
