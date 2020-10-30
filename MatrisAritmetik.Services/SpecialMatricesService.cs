using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class SpecialMatricesService : ISpecialMatricesService
    {
        public MatrisBase<dynamic> Identity(int dimension)
        {
            if (dimension <= 0)
                throw new Exception("Dimensions of identity matrix can't be lower than 1");

            dimension = Math.Min(dimension, (int)MatrisLimits.forCols);

            List<List<dynamic>> empty = new List<List<dynamic>>(dimension);
            List<dynamic> temprow;
            for (int i = 0; i < dimension; i++)
            {
                temprow = new List<dynamic>();
                for(int j = 0; j < dimension; j++)
                {
                    if (i == j)
                        temprow.Add((float)1.0);
                    else
                        temprow.Add((float)0.0);
                }
                empty.Add(temprow);
            }

            return new MatrisBase<dynamic>(empty);
        }

        public MatrisBase<dynamic> RandIntMat(int row, int col, int min, int max, dynamic seed = null)
        {
            if (row <= 0 || col <= 0)
                throw new Exception("Bad dimensions for a matrix");

            row = Math.Min(row, (int)MatrisLimits.forRows);
            col = Math.Min(col, (int)MatrisLimits.forCols);

            var random = new Random();
            if (seed != null)
                random = new Random(seed);

            List<List<dynamic>> vals = new List<List<dynamic>>();
            int maxoffset = max - min + 1;

            for(int i = 0; i < row; i++)
            {
                vals.Add(new List<dynamic>());
                for(int j = 0; j < col; j++)
                {
                    vals[i].Add(min + random.Next()%maxoffset);
                }
            }

            return new MatrisBase<dynamic>() { row = row, col = col, values = vals };
        }

        public MatrisBase<dynamic> RandFloatMat(int row, int col, float min, float max, dynamic seed = null)
        {
            if (row <= 0 || col <= 0)
                throw new Exception("Bad dimensions for a matrix");

            row = Math.Min(row, (int)MatrisLimits.forRows);
            col = Math.Min(col, (int)MatrisLimits.forCols);

            var random = new Random();
            if (seed != null)
                random = new Random(seed);

            List<List<dynamic>> vals = new List<List<dynamic>>();
            float maxoffset = max - min;

            for (int i = 0; i < row; i++)
            {
                vals.Add(new List<dynamic>());
                for (int j = 0; j < col; j++)
                {
                    vals[i].Add(min + (float)random.NextDouble() % maxoffset);
                }
            }

            return new MatrisBase<dynamic>() { row = row, col = col, values = vals };
        }
    }
}
