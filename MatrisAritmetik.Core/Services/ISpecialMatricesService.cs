using System;
using System.Collections.Generic;
using System.Text;

namespace MatrisAritmetik.Core.Services
{
    public interface ISpecialMatricesService
    {
        MatrisBase<dynamic> Identity(int dimension);

        MatrisBase<dynamic> RandIntMat(int row, int col, int min, int max, dynamic seed = null);

        MatrisBase<dynamic> RandFloatMat(int row, int col, float min, float max, dynamic seed = null);

    }
}
