namespace MatrisAritmetik.Core.Services
{
    public interface ISpecialMatricesService
    {
        MatrisBase<dynamic> Identity(int dimension);

        MatrisBase<dynamic> RandIntMat(int row, int col, int min = 0, int max = 1, dynamic seed = null);

        MatrisBase<dynamic> RandFloatMat(int row, int col, float min = (float)0.0, float max = (float)1.0, dynamic seed = null);

    }
}
