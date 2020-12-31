using System;
using System.Collections.Generic;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Services
{
    public class StatisticsService : IStatisticsService
    {
        public MatrisBase<object> Describe(MatrisBase<object> df,
                                           int usePopulation = 0,
                                           int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            List<object> mins = ArrayMethods.CopyList<float>(Min(df, numberOnly)[0]);
            List<object> meds = ArrayMethods.CopyList<float>(Median(df, numberOnly)[0]);
            List<object> maxs = ArrayMethods.CopyList<float>(Max(df, numberOnly)[0]);
            List<object> mods = ArrayMethods.CopyList<float>(Mode(df, numberOnly)[0]);
            List<object> meas = ArrayMethods.CopyList<float>(Mean(df, numberOnly)[0]);
            List<object> sdev = ArrayMethods.CopyList<float>(SDev(df, usePopulation, numberOnly)[0]);
            List<object> vars = ArrayMethods.CopyList<float>(Var(df, usePopulation, numberOnly)[0]);

            int nc = df.Col;
            List<List<object>> desc = new List<List<object>>();
            for (int j = 0; j < nc; j++)
            {
                desc.Add(new List<object>()
                            {
                                mins[j],
                                meds[j],
                                maxs[j],
                                mods[j],
                                meas[j],
                                sdev[j],
                                vars[j]
                            }
                        );
            }
            mins.Clear();
            meds.Clear();
            maxs.Clear();
            mods.Clear();
            meas.Clear();
            sdev.Clear();
            vars.Clear();

            List<LabelList> newcollabels = CompilerUtils.Create1DLabelListFromList("Min", "Median", "Max", "Mode", "Mean", "Sdev", "Var");

            if (df is Dataframe dataframe)
            {
                return new Dataframe(desc,
                                     dataframe.Delimiter,
                                     dataframe.NewLine,
                                     dataframe.GetCopyOfLabels(dataframe.GetColLabels()) ?? new List<LabelList>() { new LabelList(df.Col, 1, "col_", 1) },
                                     newcollabels,
                                     dataframe.GetRowSettings().Copy(),
                                     dataframe.GetColSettings().Copy(),
                                     true
                                     );
            }
            else
            {
                return new Dataframe(desc,
                                     rowLabels: new List<LabelList>() { new LabelList(df.Col, 1, "col_", 1) },
                                     colLabels: newcollabels
                                     );
            }

        }

        public MatrisBase<object> Head(MatrisBase<object> df,
                                       int n = 5)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            n = Math.Min(n, df.Row);
            if (n <= 0 || df.Row < n)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", 1, df.Row));
            }

            if (df.Row == n)
            {
                return df is Dataframe ? ((Dataframe)df.Copy()) : df.Copy();
            }
            else
            {
                return df is Dataframe dataframe
                    ? new Dataframe(dataframe[new Range(0, n)],
                                         dataframe.Delimiter,
                                         dataframe.NewLine,
                                         null,
                                         dataframe.GetCopyOfLabels(dataframe.GetColLabels()),
                                         dataframe.GetRowSettings().Copy(),
                                         dataframe.GetColSettings().Copy()
                                         )
                    : new MatrisBase<object>(df[new Range(0, n)]);
            }
        }

        public MatrisBase<object> Tail(MatrisBase<object> df,
                                       int n = 5)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            n = Math.Min(n, df.Row);
            if (n <= 0 || df.Row < n)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", 1, df.Row));
            }

            if (df.Row == n)
            {
                return df is Dataframe ? ((Dataframe)df.Copy()) : df.Copy();
            }
            else
            {
                return df is Dataframe dataframe
                    ? new Dataframe(dataframe[new Range(df.Row - n, df.Row)],
                                         dataframe.Delimiter,
                                         dataframe.NewLine,
                                         null,
                                         dataframe.GetCopyOfLabels(dataframe.GetColLabels()),
                                         dataframe.GetRowSettings().Copy(),
                                         dataframe.GetColSettings().Copy()
                                         )
                    : new MatrisBase<object>(df[new Range(df.Row - n, df.Row)]);
            }
        }

        public MatrisBase<object> Sample(MatrisBase<object> df,
                                         int n = 5)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nr = df.Row;
            n = Math.Min(n, nr);
            if (n <= 0 || nr < n)
            {
                throw new Exception(CompilerMessage.MAT_OUT_OF_RANGE_INDEX("satır", 1, nr));
            }

            if (nr == n)
            {
                return df is Dataframe ? ((Dataframe)df.Copy()) : df.Copy();
            }
            else
            {
                List<List<object>> newList = new List<List<object>>();

                List<List<object>> shuffled = new MatrisArithmeticService().Shuffle(df, 0).GetValues();

                for (int i = 0; i < n; i++)
                {
                    newList.Add(shuffled[i]);
                }

                return df is Dataframe dataframe
                    ? new Dataframe(newList,
                                    dataframe.Delimiter,
                                    dataframe.NewLine,
                                    null,
                                    dataframe.GetCopyOfLabels(dataframe.GetColLabels()),
                                    dataframe.GetRowSettings().Copy(),
                                    dataframe.GetColSettings().Copy()
                                    )
                    : new MatrisBase<object>(newList);
            }
        }

        public MatrisBase<object> Min(MatrisBase<object> df,
                                      int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nc = df.Col;
            List<object> mins = new List<object>();
            for (int c = 0; c < nc; c++)
            {
                mins.Add(ArrayMethods.ArrayMin(df.ColList(c, 0), numberOnly) ?? float.NaN);
            }

            return df is Dataframe data
                ? new Dataframe(new List<List<object>>() { mins },
                                data.Delimiter,
                                data.NewLine,
                                null,
                                data.GetCopyOfLabels(data.GetColLabels()),
                                null,
                                data.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { mins });
        }

        public MatrisBase<object> Max(MatrisBase<object> df,
                                      int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nc = df.Col;
            List<object> maxs = new List<object>();
            for (int c = 0; c < nc; c++)
            {
                maxs.Add(ArrayMethods.ArrayMax(df.ColList(c, 0), numberOnly) ?? float.NaN);
            }

            return df is Dataframe data
                ? new Dataframe(new List<List<object>>() { maxs },
                                data.Delimiter,
                                data.NewLine,
                                null,
                                data.GetCopyOfLabels(data.GetColLabels()),
                                null,
                                data.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { maxs });
        }

        public MatrisBase<object> Sum(MatrisBase<object> df,
                                      int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nc = df.Col;
            int nr = df.Row;

            List<object> sums = new List<object>();
            for (int c = 0; c < nc; c++)
            {
                sums.Add(ArrayMethods.ArraySum(df.ColList(c, 0), 0, nr, numberOnly) ?? float.NaN);
            }

            return df is Dataframe data
                ? new Dataframe(new List<List<object>>() { sums },
                                data.Delimiter,
                                data.NewLine,
                                null,
                                data.GetCopyOfLabels(data.GetColLabels()),
                                null,
                                data.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { sums });
        }

        public MatrisBase<object> Mul(MatrisBase<object> df,
                                      int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nc = df.Col;
            int nr = df.Row;

            List<object> muls = new List<object>();
            for (int c = 0; c < nc; c++)
            {
                muls.Add(ArrayMethods.ArrayMul(df.ColList(c, 0), 0, nr, numberOnly) ?? float.NaN);
            }

            return df is Dataframe data
                ? new Dataframe(new List<List<object>>() { muls },
                                data.Delimiter,
                                data.NewLine,
                                null,
                                data.GetCopyOfLabels(data.GetColLabels()),
                                null,
                                data.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { muls });
        }

        public MatrisBase<object> Mean(MatrisBase<object> df,
                                       int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nc = df.Col;
            int nr = df.Row;

            List<object> means = new List<object>();
            int pop;
            for (int c = 0; c < nc; c++)
            {
                pop = nr - (numberOnly == 1 ? df.AmountOfNanInColumn(c) : 0);
                object res = ArrayMethods.ArraySum(df.ColList(c, 0), 0, nr, numberOnly) ?? float.NaN;
                if (pop == 0)
                {
                    means.Add(float.NaN);
                }
                else
                {
                    means.Add(float.IsNaN((float)res) ? res : (float)res / pop);
                }

            }

            return df is Dataframe data
                ? new Dataframe(new List<List<object>>() { means },
                                data.Delimiter,
                                data.NewLine,
                                null,
                                data.GetCopyOfLabels(data.GetColLabels()),
                                null,
                                data.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { means });
        }

        public MatrisBase<object> SDev(MatrisBase<object> df,
                                       int usePopulation = 0,
                                       int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            if (usePopulation != 0 && usePopulation != 1)
            {
                throw new Exception(CompilerMessage.ARG_INVALID_VALUE("usePopulation", "Örneklem için 0, popülasyon için 1 olmalı!"));
            }

            int nr = df.Row;
            int nc = df.Col;

            if (nr == 1 && usePopulation == 1)
            {
                usePopulation = 0;
            }

            List<object> means = Mean(df).RowList(1);
            List<object> sdevs = new List<object>();
            List<List<object>> vals = df.GetValues();
            int pop;
            for (int j = 0; j < nc; j++)
            {
                float sdev = 0;
                float mean = (float)means[j];
                if (float.IsNaN(mean))
                {
                    if (numberOnly == 1)
                    {
                        sdevs.Add(0);
                    }
                    else
                    {
                        sdevs.Add(float.NaN);
                    }
                    continue;
                }
                for (int i = 0; i < nr; i++)
                {
                    if (float.TryParse(vals[i][j].ToString(), out float res))
                    {
                        if (numberOnly == 1 && float.IsNaN(res))
                        {
                            continue;
                        }
                        sdev += (float)Math.Pow(res - mean, 2);
                    }
                    else
                    {
                        if (numberOnly == 1)
                        {
                            continue;
                        }
                        else
                        {
                            sdev = float.NaN;
                            break;
                        }
                    }
                }
                pop = nr - usePopulation - (numberOnly == 1 ? df.AmountOfNanInColumn(j) : 0);
                if (pop == 0)
                {
                    sdevs.Add(0);
                }
                else
                {
                    sdevs.Add((float)Math.Pow(sdev * (1.0 / pop), 0.5));
                }
            }

            return df is Dataframe data
                ? new Dataframe(new List<List<object>>() { sdevs },
                                data.Delimiter,
                                data.NewLine,
                                null,
                                data.GetCopyOfLabels(data.GetColLabels()),
                                null,
                                data.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { sdevs });
        }

        public MatrisBase<object> Var(MatrisBase<object> df,
                                      int usePopulation = 0,
                                      int numberOnly = 1)
        {
            return SDev(df, usePopulation, numberOnly).Power(2);
        }

        public MatrisBase<object> Median(MatrisBase<object> df,
                                         int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int pop;
            int nr = df.Row;
            int nc = df.Col;
            List<object> meds = new List<object>();

            if (df is Dataframe dataframe)
            {
                for (int j = 0; j < nc; j++)
                {
                    if (!dataframe.IsAllNumberColumn(j, 0))
                    {
                        if (numberOnly == 1)
                        {
                            List<object> col = new List<object>();
                            foreach (object o in dataframe.ColList(j, 0))
                            {
                                if (o is None
                                    || o is null
                                    || ((!float.TryParse(o.ToString(), out float res)
                                        || float.IsNaN(res))))
                                {
                                    continue;
                                }
                                col.Add(o);
                            }

                            if (col.Count == 0)
                            {
                                meds.Add(float.NaN);
                                continue;
                            }

                            pop = nr - df.AmountOfNanInColumn(j);
                            col.Sort();
                            meds.Add((pop % 2 == 1)
                                ? (float)col[((pop + 1) / 2) - 1]
                                : ((float)col[(pop / 2) - 1] + (float)col[(int)Math.Round((decimal)(pop + 1) / 2, 0) - 1]) / 2);
                        }
                        else
                        {
                            meds.Add(float.NaN);
                        }
                    }
                    else
                    {
                        List<object> col = dataframe.ColList(j, 0);
                        col.Sort();
                        meds.Add((nr % 2 == 1)
                            ? (float)col[((nr + 1) / 2) - 1]
                            : ((float)col[(nr / 2) - 1] + (float)col[(int)Math.Round((decimal)(nr + 1) / 2, 0) - 1]) / 2);
                    }
                }

                return new Dataframe(new List<List<object>>() { meds },
                                     dataframe.Delimiter,
                                     dataframe.NewLine,
                                     null,
                                     dataframe.GetCopyOfLabels(dataframe.GetColLabels()),
                                     null,
                                     dataframe.GetColSettings().Copy());
            }
            else
            {

                for (int j = 0; j < nc; j++)
                {
                    if (numberOnly == 1)
                    {
                        List<object> col = new List<object>();
                        foreach (object o in df.ColList(j, 0))
                        {
                            if (o is None
                                || o is null
                                || ((!float.TryParse(o.ToString(), out float res)
                                    || float.IsNaN(res))))
                            {
                                continue;
                            }
                            col.Add(o);
                        }

                        if (col.Count == 0)
                        {
                            meds.Add(float.NaN);
                            continue;
                        }

                        pop = nr - df.AmountOfNanInColumn(j);
                        col.Sort();
                        meds.Add((pop % 2 == 1)
                            ? (float)col[((pop + 1) / 2) - 1]
                            : ((float)col[(pop / 2) - 1] + (float)col[(int)Math.Round((decimal)(pop + 1) / 2, 0) - 1]) / 2);
                    }
                    else
                    {
                        List<object> col = df.ColList(j, 0);
                        col.Sort();
                        meds.Add((nr % 2 == 1)
                            ? (float)col[((nr + 1) / 2) - 1]
                            : (float)col[(nr / 2) - 1] + (float)col[(int)Math.Round((decimal)(nr + 1) / 2, 0) - 1]);
                    }

                }

                return new MatrisBase<object>(new List<List<object>>() { meds });
            }
        }

        public MatrisBase<object> Mode(MatrisBase<object> df,
                                       int numberOnly = 1)
        {
            if (!df.IsValid())
            {
                throw new Exception(CompilerMessage.DF_INVALID_SIZE);
            }

            int nc = df.Col;
            List<object> modes = new List<object>();

            for (int j = 0; j < nc; j++)
            {
                List<object> col = df.ColList(j, 0);
                Dictionary<object, int> tracker = new Dictionary<object, int>();
                foreach (object val in col)
                {
                    if (tracker.ContainsKey(val))
                    {
                        tracker[val]++;
                    }
                    else
                    {
                        tracker.Add(val, 1);
                    }
                }

                object mode = null;
                object placeholder = new None();
                placeholder = (df is Dataframe ? placeholder : float.NaN);

                int currentmax = 0;
                foreach (KeyValuePair<object, int> pair in tracker)
                {
                    if (numberOnly == 1 && (pair.Key is None
                            || pair.Key is null
                            || ((!float.TryParse(pair.Value.ToString(), out float res)
                            || float.IsNaN(res)))))
                    {
                        continue;
                    }

                    if (pair.Value > currentmax)
                    {
                        currentmax = pair.Value;
                        mode = pair.Key;
                    }
                }
                modes.Add(mode ?? placeholder);
            }

            return df is Dataframe dataframe
                ? new Dataframe(new List<List<object>>() { modes },
                                dataframe.Delimiter,
                                dataframe.NewLine,
                                null,
                                dataframe.GetCopyOfLabels(dataframe.GetColLabels()),
                                null,
                                dataframe.GetColSettings().Copy())
                : new MatrisBase<object>(new List<List<object>>() { modes });
        }

    }
}
