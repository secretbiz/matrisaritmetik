using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    public class DataframeRowSettings : IDisposable
    {
        #region Settings
        public string LabelSeperatorFromElements = "||";
        public string LabelSeperatorFromCorner = "+";
        public string LevelSeperator = "|";
        public string SpanSeperator = "-";
        #endregion

        public DataframeRowSettings() { }

        public DataframeRowSettings Copy()
        {
            return new DataframeRowSettings()
            {
                LabelSeperatorFromCorner = LabelSeperatorFromCorner,
                LabelSeperatorFromElements = LabelSeperatorFromElements,
                LevelSeperator = LevelSeperator,
                SpanSeperator = SpanSeperator
            };
        }

        #region Dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                LabelSeperatorFromElements = null;
                LabelSeperatorFromCorner = null;
                LevelSeperator = null;
                SpanSeperator = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~DataframeRowSettings()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class DataframeColSettings : IDisposable
    {
        #region Settings
        public string LabelSeperatorFromElements = "=";
        public string LabelSeperatorFromCorner = "+";
        public string LevelSeperator = "-";
        public string SpanSeperator = "|";
        #endregion

        public DataframeColSettings() { }

        public DataframeColSettings Copy()
        {
            return new DataframeColSettings()
            {
                LabelSeperatorFromCorner = LabelSeperatorFromCorner,
                LabelSeperatorFromElements = LabelSeperatorFromElements,
                LevelSeperator = LevelSeperator,
                SpanSeperator = SpanSeperator
            };
        }
        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                LabelSeperatorFromElements = null;
                LabelSeperatorFromCorner = null;
                LevelSeperator = null;
                SpanSeperator = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~DataframeColSettings()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Dataframe : MatrisBase<dynamic>, IDisposable
    {
        #region Private Encapsulated Fields
        private int _row = -1;
        private int _col = -1;
        private List<List<dynamic>> _values;
        private List<LabelList> _rowlabels;
        private List<LabelList> _collabels;

        private bool disposedValue;
        #endregion

        #region Public Properties
        public override int Row
        {
            get
            {
                if (_row > (int)DataframeLimits.forRows)
                {
                    _row = (int)DataframeLimits.forRows;
                }
                return _row;
            }
            set
            {
                if (_row == -1 && value >= 0)
                {
                    _row = value;
                }
            }
        }

        public override int Col
        {
            get
            {
                if (_col > (int)DataframeLimits.forCols)
                {
                    _col = (int)DataframeLimits.forCols;
                }
                return _col;
            }
            set
            {
                if (_col == -1 && value >= 0)
                {
                    _col = value;
                }
            }
        }

        public override List<List<dynamic>> Values
        {
            get
            {
                if (_values != null)
                {
                    if (_values.Count * _values[0].Count > (int)DataframeLimits.forSize)
                    {
                        if (_values.Count > (int)DataframeLimits.forRows)
                        {
                            int delindex = (int)DataframeLimits.forRows;
                            for (int i = 0; i < _values.Count - delindex; i++)
                            {
                                _values.RemoveAt(delindex);
                            }
                        }
                        if (_values.Count >= 1)
                        {
                            if (_values[0].Count > (int)DataframeLimits.forCols)
                            {
                                int delindex = (int)DataframeLimits.forCols;
                                for (int i = 0; i < _values.Count; i++)
                                {
                                    if (_values[i].Count > delindex)
                                    {
                                        for (int j = 0; j < delindex; j++)
                                        {
                                            _values[i].RemoveAt(delindex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return _values;
            }
            set
            {
                if (value != null)
                {
                    if (_values == null) // Only set if first time
                    {
                        if (value.Count * value[0].Count > (int)DataframeLimits.forSize)
                        {
                            List<List<dynamic>> temp = new List<List<dynamic>>();
                            int collimit = (int)DataframeLimits.forCols;

                            for (int i = 0; i < Math.Min(value.Count, (int)DataframeLimits.forRows); i++)
                            {
                                temp.Add(new List<dynamic>());
                                for (int j = 0; j < Math.Min(value[i].Count, collimit); j++)
                                {
                                    temp[i].Add(value[i][j]);
                                }
                            }
                            _values = temp;
                        }
                        else
                        {
                            _values = value;
                        }
                    }
                }
                else
                {
                    if (_values != null)
                    {
                        foreach (List<dynamic> l in _values)
                        {
                            l.Clear();
                        }
                        _values.Clear();
                        _values = null;
                    }
                }
            }
        }

        public List<LabelList> RowLabels
        {
            get
            {
                if (_rowlabels != null)
                {
                    if (_rowlabels.Count > (int)DataframeLimits.forRowLabelLevels)
                    {
                        int delindex = (int)DataframeLimits.forRowLabelLevels;
                        for (int i = 0; i < _rowlabels.Count - delindex; i++)
                        {
                            _rowlabels.RemoveAt(delindex);
                        }
                    }
                    if (_rowlabels.Count >= 1)
                    {
                        if (_rowlabels[0].Length > (int)DataframeLimits.forRows)
                        {
                            int delindex = (int)DataframeLimits.forRows;
                            for (int i = 0; i < _rowlabels.Count; i++)
                            {
                                if (_rowlabels[i].TotalSpan != _row)
                                {
                                    throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(_row, _rowlabels[i].TotalSpan));
                                }

                                if (_rowlabels[i].Length > delindex)
                                {
                                    for (int j = 0; j < delindex; j++)
                                    {
                                        _rowlabels[i].Labels.RemoveAt(delindex);
                                    }
                                }
                            }
                        }
                    }
                }

                return _rowlabels;
            }
            set
            {
                if (value != null)
                {
                    _rowlabels = null;
                    if (value.Count > (int)DataframeLimits.forRowLabelLevels)
                    {
                        _rowlabels = value.GetRange(0, (int)DataframeLimits.forRowLabelLevels);
                    }

                    if (value.Count >= 1)
                    {
                        if (value[0].Length > (int)DataframeLimits.forRows)
                        {
                            int len = (int)DataframeLimits.forRows;
                            for (int i = 0; i < value.Count; i++)
                            {
                                if (value[i].TotalSpan != _row)
                                {
                                    throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(_row, value[i].TotalSpan));
                                }
                                if (value[i].Length > len)
                                {
                                    _rowlabels[i] = value[i].GetRange(0, len);
                                }
                            }
                        }
                    }
                    else
                    {
                        _rowlabels = value;
                    }
                }
                else
                {
                    if (_rowlabels != null)
                    {
                        foreach (LabelList l in _rowlabels)
                        {
                            l.Dispose();
                        }
                        _rowlabels.Clear();
                        _rowlabels = null;
                    }
                }
            }
        }

        public List<LabelList> ColLabels
        {
            get
            {
                if (_collabels != null)
                {
                    if (_collabels.Count > (int)DataframeLimits.forColLabelLevels)
                    {
                        int delindex = (int)DataframeLimits.forColLabelLevels;
                        for (int i = 0; i < _collabels.Count - delindex; i++)
                        {
                            _collabels.RemoveAt(delindex);
                        }
                    }
                    if (_collabels.Count >= 1)
                    {
                        if (_collabels[0].Length > (int)DataframeLimits.forCols)
                        {
                            int delindex = (int)DataframeLimits.forCols;
                            for (int i = 0; i < _collabels.Count; i++)
                            {
                                if (_collabels[i].TotalSpan != _col)
                                {
                                    throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(_col, _collabels[i].TotalSpan));
                                }
                                if (_collabels[i].Length > delindex)
                                {
                                    for (int j = 0; j < delindex; j++)
                                    {
                                        _collabels[i].Labels.RemoveAt(delindex);
                                    }
                                }
                            }
                        }
                    }
                }

                return _collabels;
            }
            set
            {
                if (value != null)
                {
                    _collabels = null;
                    if (value.Count > (int)DataframeLimits.forColLabelLevels)
                    {
                        _collabels = value.GetRange(0, (int)DataframeLimits.forColLabelLevels);
                    }

                    if (value.Count >= 1)
                    {
                        if (value[0].Length > (int)DataframeLimits.forCols)
                        {
                            int len = (int)DataframeLimits.forCols;
                            for (int i = 0; i < value.Count; i++)
                            {
                                if (value[i].TotalSpan != _col)
                                {
                                    throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(_col, value[i].TotalSpan));
                                }
                                if (value[i].Length > len)
                                {
                                    _collabels[i] = value[i].GetRange(0, len);
                                }
                            }
                        }
                    }
                    else
                    {
                        _collabels = value;
                    }
                }
                else
                {
                    if (_collabels != null)
                    {
                        foreach (LabelList l in _collabels)
                        {
                            l.Dispose();
                        }
                        _collabels.Clear();
                        _collabels = null;
                    }
                }
            }
        }

        public DataframeRowSettings RowSettings = new DataframeRowSettings();

        public DataframeColSettings ColSettings = new DataframeColSettings();

        #endregion

        #region Constructors
        public Dataframe() { }

        public Dataframe(List<List<dynamic>> vals,
                         List<LabelList> rowLabels = null,
                         List<LabelList> colLabels = null,
                         DataframeRowSettings rowSettings = null,
                         DataframeColSettings colSettings = null)
        {
            // Values
            Values = vals;

            // Labels
            if (rowLabels == null)
            {
                _rowlabels = new List<LabelList>() { new LabelList(_row, 1, "row_", 1) };
            }
            else
            {
                RowLabels = rowLabels;
            }
            if (colLabels == null)
            {
                _collabels = new List<LabelList>() { new LabelList(_col, 1, "col_", 1) };
            }
            else
            {
                RowLabels = rowLabels;
            }

            // Settings
            if (rowSettings != null)
            {
                RowSettings = rowSettings;
            }
            if (colSettings != null)
            {
                ColSettings = colSettings;
            }

        }
        #endregion

        #region Private Methods
        private int[] GetColumnWidths(Dataframe df)
        {
            int[] colwidths = new int[_col + RowLabels.Count];

            return colwidths;
        }
        private string GetDebuggerDisplay()
        {
            return "df(" + _row + "," + _col + "):" + ToString();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a deep copy of the dataframe
        /// </summary>
        /// <returns>A new dataframe with the same values, labels and settings</returns>
        public new Dataframe Copy()
        {
            int r = Row;
            int c = Col;
            List<List<dynamic>> lis = new List<List<dynamic>>();
            for (int i = 0; i < r; i++)
            {
                lis.Add(new List<dynamic>());
                for (int j = 0; j < c; j++)
                {
                    lis[i].Add(Values[i][j]);
                }
            }

            List<LabelList> rowlbls = new List<LabelList>();
            foreach (LabelList lbl in RowLabels)
            {
                rowlbls.Add(lbl.Copy());
            }
            List<LabelList> colbls = new List<LabelList>();
            foreach (LabelList lbl in ColLabels)
            {
                colbls.Add(lbl.Copy());
            }

            return new Dataframe(lis, rowlbls, colbls, RowSettings.Copy(), ColSettings.Copy());
        }

        /// <summary>
        /// Create a printable string with dataframe's labels and elements aligned   
        /// </summary>
        /// <returns>String created from dataframe's labels, elements and settings</returns>
        public override string ToString()
        {
            if (!IsValid())
            {
                return "";
            }

            StringBuilder res = new StringBuilder();
            // Column sizes
            int[] longest_in_col = GetColumnWidths(this);

            return res.ToString();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Values
                    if (_values != null)
                    {
                        foreach (List<dynamic> r in _values)
                        {
                            r.Clear();
                        }
                        _values.Clear();
                        _values = null;
                    }

                    // Labels
                    if (_rowlabels != null)
                    {
                        foreach (LabelList lvl in _rowlabels)
                        {
                            lvl.Dispose();
                        }
                        _rowlabels.Clear();
                        _rowlabels = null;
                    }
                    if (_collabels != null)
                    {
                        foreach (LabelList lvl in _collabels)
                        {
                            lvl.Dispose();
                        }
                        _collabels.Clear();
                        _collabels = null;
                    }

                    // Settings
                    if (RowSettings != null)
                    {
                        RowSettings.Dispose();
                        RowSettings = null;
                    }

                    if (ColSettings != null)
                    {
                        ColSettings.Dispose();
                        ColSettings = null;
                    }
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Dataframe()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        #endregion
    }
}
