using System;
using System.Collections.Generic;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class SpecialMatricesService : ISpecialMatricesService
    {
        public MatrisBase<dynamic> Identity(int dimension)
        {
            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, (int)MatrisLimits.forCols);

            List<List<dynamic>> empty = new List<List<dynamic>>(dimension);
            List<dynamic> temprow;
            for (int i = 0; i < dimension; i++)
            {
                temprow = new List<dynamic>();
                for (int j = 0; j < dimension; j++)
                {
                    if (i == j)
                    {
                        temprow.Add((float)1.0);
                    }
                    else
                    {
                        temprow.Add((float)0.0);
                    }
                }
                empty.Add(temprow);
            }

            return new MatrisBase<dynamic>(empty);
        }

        public MatrisBase<dynamic> RandIntMat(int row,
                                              int col,
                                              int min = 0,
                                              int max = 1,
                                              dynamic seed = null)
        {
            if (row <= 0 || col <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            row = Math.Min(row, (int)MatrisLimits.forRows);
            col = Math.Min(col, (int)MatrisLimits.forCols);

            Random random;
            int s;

            if (seed != null)
            {
                random = new Random(seed);
                s = seed;
            }
            else
            {
                s = Environment.TickCount & int.MaxValue;
                random = new Random(s);
            }

            if (max < min)
            {
                throw new Exception(CompilerMessage.MAT_MINMAX_ORDER);
            }

            List<List<dynamic>> vals = new List<List<dynamic>>();
            int maxoffset = max - min + 1;

            for (int i = 0; i < row; i++)
            {
                vals.Add(new List<dynamic>());
                for (int j = 0; j < col; j++)
                {
                    vals[i].Add(min + (random.Next() % maxoffset));
                }
            }

            return new MatrisBase<dynamic>() { Row = row, Col = col, Values = vals, Seed = s, CreatedFromSeed = true };
        }

        public MatrisBase<dynamic> RandFloatMat(int row,
                                                int col,
                                                float min = (float)0.0,
                                                float max = (float)1.0,
                                                dynamic seed = null)
        {
            if (row <= 0 || col <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            row = Math.Min(row, (int)MatrisLimits.forRows);
            col = Math.Min(col, (int)MatrisLimits.forCols);

            Random random;
            int s;

            if (seed != null)
            {
                random = new Random(seed);
                s = seed;
            }
            else
            {
                s = Environment.TickCount & int.MaxValue;
                random = new Random(s);
            }

            if (max < min)
            {
                throw new Exception(CompilerMessage.MAT_MINMAX_ORDER);
            }

            List<List<dynamic>> vals = new List<List<dynamic>>();
            float realmax = max - min;

            for (int i = 0; i < row; i++)
            {
                vals.Add(new List<dynamic>());
                for (int j = 0; j < col; j++)
                {
                    vals[i].Add((dynamic)(min + (((float)random.NextDouble()) * realmax)));
                }
            }

            return new MatrisBase<dynamic>() { Row = row, Col = col, Values = vals, Seed = s, CreatedFromSeed = true };
        }
    }
}
