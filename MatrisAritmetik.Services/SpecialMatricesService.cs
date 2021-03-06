﻿using System;
using System.Collections.Generic;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Services
{
    public class SpecialMatricesService : ISpecialMatricesService
    {
        #region SpecialMatricesService Methods

        #region Matrix Methods
        public MatrisBase<object> ToMat(Dataframe dataframe)
        {
            if (dataframe.IsValid() && dataframe.IsAllNumbers())
            {
                return new MatrisBase<object>(dataframe.Copy().GetValues());
            }
            else
            {
                throw new Exception(CompilerMessage.INVALID_CONVERSION_TO_MAT);
            }
        }

        public MatrisBase<dynamic> Identity(int dimension)
        {
            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, (int)MatrisLimits.forCols);

            List<List<dynamic>> values = new List<List<dynamic>>();
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

            if (Math.Abs(end - (start + (amount * interval))) > 1e-7)
            {
                throw new Exception(CompilerMessage.MAT_INTERVAL_EXCESS);
            }

            List<List<dynamic>> vals = new List<List<dynamic>>();

            if (axis == 0)
            {
                vals.Add(new List<dynamic>());

                for (int i = 0; i <= amount; i++)
                {
                    vals[0].Add(Math.Round(start + (interval * i), digits));
                }
            }
            else
            {
                for (int i = 0; i <= amount; i++)
                {
                    vals.Add(new List<dynamic>() { Math.Round(start + (interval * i), digits) });
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

            MatrisBase<dynamic> res = new MatrisBase<dynamic>() { Row = row, Col = col };
            res.SetValues(vals);
            return res;
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

            MatrisBase<dynamic> res = new MatrisBase<dynamic>() { Row = row, Col = col, Seed = s, CreatedFromSeed = true };
            res.SetValues(vals);
            return res;
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

            MatrisBase<dynamic> res = new MatrisBase<dynamic>() { Row = row, Col = col, Seed = s, CreatedFromSeed = true };
            res.SetValues(vals);
            return res;
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

            MatrisBase<dynamic> res = new MatrisBase<dynamic>() { Row = dimension, Col = dimension, Seed = s, CreatedFromSeed = true };
            res.SetValues(vals);
            return res;
        }

        public MatrisBase<dynamic> SymFloat(int dimension,
                                            float min = 0,
                                            float max = 1,
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

            MatrisBase<dynamic> res = new MatrisBase<dynamic>() { Row = dimension, Col = dimension, Seed = s, CreatedFromSeed = true };
            res.SetValues(vals);
            return res;
        }
        #endregion

        #region Dataframe Methods

        public Dataframe ToDf(MatrisBase<object> matrix)
        {
            if (matrix.IsValid())
            {
                return new Dataframe(matrix.Copy().GetValues());
            }
            else
            {
                throw new Exception(CompilerMessage.INVALID_CONVERSION_TO_DF);
            }
        }
        public Dataframe IdentityDf(int dimension)
        {
            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, (int)DataframeLimits.forCols);

            List<List<object>> values = new List<List<object>>();
            List<object> temprow;
            for (int i = 0; i < dimension; i++)
            {
                temprow = new List<object>();
                for (int j = 0; j < dimension; j++)
                {
                    temprow.Add((float)0.0);
                }
                values.Add(temprow);
            }
            for (int n = 0; n < dimension; n++)
            {
                values[n][n] = 1;
            }
            return new Dataframe(values);
        }

        public Dataframe RangeDf(float start,
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

            int amount = Math.Min((int)((end - start) / interval), axis == 0 ? (int)DataframeLimits.forRows : (int)DataframeLimits.forCols);

            if (Math.Abs(end - (start + (amount * interval))) > 1e-7)
            {
                throw new Exception(CompilerMessage.MAT_INTERVAL_EXCESS);
            }

            List<List<object>> vals = new List<List<object>>();

            if (axis == 0)
            {
                vals.Add(new List<object>());

                for (int i = 0; i <= amount; i++)
                {
                    vals[0].Add(Math.Round(start + (interval * i), digits));
                }
            }
            else
            {
                for (int i = 0; i <= amount; i++)
                {
                    vals.Add(new List<object>() { Math.Round(start + (interval * i), digits) });
                }
            }

            return new Dataframe(vals);
        }

        public Dataframe FillDf(int row,
                                int col,
                                dynamic fill)
        {
            if (row <= 0 || col <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            row = Math.Min(row, (int)DataframeLimits.forRows);
            col = Math.Min(col, (int)DataframeLimits.forCols);

            List<List<object>> vals = new List<List<object>>();
            if (fill != null)
            {
                if (float.TryParse(((object)fill).ToString(), out float res))
                {
                    fill = res;
                }
                else
                {
                    fill = ((object)fill).ToString();
                }

                for (int i = 0; i < row; i++)
                {
                    vals.Add(new List<object>());
                    for (int j = 0; j < col; j++)
                    {
                        vals[i].Add(fill);
                    }
                }
            }
            else
            {
                for (int i = 0; i < row; i++)
                {
                    vals.Add(new List<object>());
                    for (int j = 0; j < col; j++)
                    {
                        vals[i].Add(new None());
                    }
                }
            }

            return new Dataframe(vals);
        }

        public Dataframe SymIntDf(int dimension,
                                  int min = 0,
                                  int max = 1,
                                  dynamic seed = null)
        {
            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, Math.Min((int)DataframeLimits.forRows, (int)DataframeLimits.forCols));

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

            List<List<object>> vals = new List<List<object>>();
            for (int i = 0; i < dimension; i++)
            {
                vals.Add(new List<object>());
            }

            int maxoffset = max - min + 1;
            int val;
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

            return new Dataframe(vals) { Seed = s, CreatedFromSeed = true };
        }

        public Dataframe SymFloatDf(int dimension,
                                    float min = 0,
                                    float max = 1,
                                    dynamic seed = null)
        {

            if (dimension <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            dimension = Math.Min(dimension, Math.Min((int)DataframeLimits.forRows, (int)DataframeLimits.forCols));

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

            List<List<object>> vals = new List<List<object>>();
            for (int i = 0; i < dimension; i++)
            {
                vals.Add(new List<object>());
            }

            float realmax = max - min;
            float val;
            for (int i = 0; i < dimension; i++)
            {
                for (int j = i; j < dimension; j++)
                {
                    val = (float)(min + (((float)random.NextDouble()) * realmax));
                    vals[i].Add(val);
                    if (i != j)
                    {
                        vals[j].Add(val);
                    }
                }
            }

            return new Dataframe(vals) { Seed = s, CreatedFromSeed = true };
        }

        public Dataframe RandIntDf(int row,
                                   int col,
                                   int min = 0,
                                   int max = 1,
                                   dynamic seed = null)
        {
            if (row <= 0 || col <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            row = Math.Min(row, (int)DataframeLimits.forRows);
            col = Math.Min(col, (int)DataframeLimits.forCols);

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

            List<List<object>> vals = new List<List<object>>();
            int maxoffset = max - min + 1;

            for (int i = 0; i < row; i++)
            {
                vals.Add(new List<object>());
                for (int j = 0; j < col; j++)
                {
                    vals[i].Add(min + (random.Next() % maxoffset));
                }
            }

            return new Dataframe(vals) { Seed = s, CreatedFromSeed = true };
        }

        public Dataframe RandFloatDf(int row,
                                     int col,
                                     float min = 0,
                                     float max = 1,
                                     dynamic seed = null)
        {
            if (row <= 0 || col <= 0)
            {
                throw new Exception(CompilerMessage.MAT_INVALID_SIZE);
            }

            row = Math.Min(row, (int)DataframeLimits.forRows);
            col = Math.Min(col, (int)DataframeLimits.forCols);

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

            List<List<object>> vals = new List<List<object>>();
            float realmax = max - min;

            for (int i = 0; i < row; i++)
            {
                vals.Add(new List<object>());
                for (int j = 0; j < col; j++)
                {
                    vals[i].Add((float)(min + (((float)random.NextDouble()) * realmax)));
                }
            }

            return new Dataframe(vals) { Seed = s, CreatedFromSeed = true };
        }
        #endregion

        #endregion
    }
}
