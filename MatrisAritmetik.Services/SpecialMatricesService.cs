using System;
using System.Collections.Generic;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class SpecialMatricesService : ISpecialMatricesService
    {
        #region SpecialMatricesService Methods
        public MatrisBase<dynamic> Identity(int dimension)
        {
            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, (int)MatrisLimits.forCols);

            List<List<dynamic>> values = new List<List<dynamic>>(dimension);
            List<dynamic> temprow;
            for (int i = 0; i < dimension; i++)
            {
                temprow = new List<dynamic>();
                for (int j = 0; j < dimension; j++)
                {
                    temprow.Add((float)0.0);
                }
                values.Add(temprow);
            }
            for (int n = 0; n < dimension; n++)
            {
                values[n][n] += 1;
            }
            return new MatrisBase<dynamic>(values);
        }

        public MatrisBase<dynamic> Range(float start,
                                         float end,
                                         float interval = 1,
                                         int axis = 0,
                                         int digits = 6)
        {
            if (start > end)
            {
                throw new Exception(CompilerMessage.MAT_START_END_ORDER);
            }
            if (interval > end - start)
            {
                throw new Exception(CompilerMessage.MAT_INTERVAL_INVALID);
            }

            if (axis != 0 && axis != 1)
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("axis", "Satır vektör için 0, sütun vekör için 1 olmalı."));
            }

            int amount = Math.Min((int)((end - start) / interval), axis == 0 ? (int)MatrisLimits.forRows : (int)MatrisLimits.forCols);

            if (Math.Abs(end - (start + amount * interval)) > 1e-8)
            {
                throw new Exception(CompilerMessage.MAT_INTERVAL_EXCESS);
            }

            List<List<dynamic>> vals = new List<List<dynamic>>();

            if (axis == 0)
            {
                vals.Add(new List<dynamic>());

                for (int i = 0; i <= amount; i++)
                {
                    vals[0].Add(Math.Round(start + interval * i, digits));
                }
            }
            else
            {
                for (int i = 0; i <= amount; i++)
                {
                    vals.Add(new List<dynamic>() { Math.Round(start + interval * i, digits) });
                }
            }

            return new MatrisBase<dynamic>(vals);
        }

        public MatrisBase<dynamic> Fill(int row,
                                        int col,
                                        float fill)
        {
            if (row <= 0 || col <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            row = Math.Min(row, (int)MatrisLimits.forRows);
            col = Math.Min(col, (int)MatrisLimits.forCols);

            List<List<dynamic>> vals = new List<List<dynamic>>();
            for (int i = 0; i < row; i++)
            {
                vals.Add(new List<dynamic>());
                for (int j = 0; j < col; j++)
                {
                    vals[i].Add(fill);
                }
            }

            return new MatrisBase<dynamic>() { Row = row, Col = col, Values = vals };
        }

        public MatrisBase<dynamic> RandInt(int row,
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

        public MatrisBase<dynamic> RandFloat(int row,
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

        public MatrisBase<dynamic> SymInt(int dimension,
                                          int min = 0,
                                          int max = 1,
                                          dynamic seed = null)
        {
            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, Math.Min((int)MatrisLimits.forRows, (int)MatrisLimits.forCols));

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
            for (int i = 0; i < dimension; i++)
            {
                vals.Add(new List<dynamic>());
            }

            int maxoffset = max - min + 1;
            dynamic val;
            for (int i = 0; i < dimension; i++)
            {
                for (int j = i; j < dimension; j++)
                {
                    val = min + (random.Next() % maxoffset);
                    vals[i].Add(val);
                    if (i != j)
                    {
                        vals[j].Add(val);
                    }
                }
            }

            return new MatrisBase<dynamic>() { Row = dimension, Col = dimension, Values = vals, Seed = s, CreatedFromSeed = true };
        }

        public MatrisBase<dynamic> SymFloat(int dimension,
                                            int min = 0,
                                            int max = 1,
                                            dynamic seed = null)
        {

            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, Math.Min((int)MatrisLimits.forRows, (int)MatrisLimits.forCols));

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
            for (int i = 0; i < dimension; i++)
            {
                vals.Add(new List<dynamic>());
            }

            float realmax = max - min;
            dynamic val;
            for (int i = 0; i < dimension; i++)
            {
                for (int j = i; j < dimension; j++)
                {
                    val = (dynamic)(min + (((float)random.NextDouble()) * realmax));
                    vals[i].Add(val);
                    if (i != j)
                    {
                        vals[j].Add(val);
                    }
                }
            }

            return new MatrisBase<dynamic>() { Row = dimension, Col = dimension, Values = vals, Seed = s, CreatedFromSeed = true };
        }
        #endregion
    }
}
