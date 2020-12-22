using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Row label settings to use for <see cref="Dataframe"/>
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class DataframeRowSettings : IDisposable
    {
        #region Settings
        /// <summary>
        /// Seperator for the right-most side of <see cref="Dataframe.RowLabels"/> rows from <see cref="Dataframe.Values"/>
        /// </summary>
        public string LabelSeperatorFromElements = "||";
        /// <summary>
        /// Seperator for the top of the <see cref="Dataframe.RowLabels"/>, only used if <see cref="Dataframe.ColLabels"/> exist
        /// </summary>
        public string LabelSeperatorFromCorner = "+";
        /// <summary>
        /// Seperator for each level of <see cref="Dataframe.RowLabels"/> 
        /// </summary>
        public string LevelSeperator = "|";
        /// <summary>
        /// Seperator for each span in a level of a <see cref="Label"/> in a <see cref="LabelList"/> 
        /// </summary>
        public string SpanSeperator = "-";
        #endregion

        /// <summary>
        /// Empty constructor
        /// </summary>
        public DataframeRowSettings() { }

        /// <summary>
        /// Create a deep copy of the current settings
        /// </summary>
        /// <returns>A new <see cref="DataframeRowSettings"/></returns>
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

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "Cor,Spn,Lvl,Ele: " + LabelSeperatorFromCorner + " " + SpanSeperator + " " + LevelSeperator + " " + LabelSeperatorFromElements;
        }
        #endregion

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

    /// <summary>
    /// Column label settings to use for <see cref="Dataframe"/>
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class DataframeColSettings : IDisposable
    {
        #region Settings
        /// <summary>
        /// Seperator for the bottom-most side of <see cref="Dataframe.ColLabels"/> columns from <see cref="Dataframe.Values"/>
        /// </summary>
        public string LabelSeperatorFromElements = "=";
        /// <summary>
        /// Seperator for the left of the <see cref="Dataframe.ColLabels"/>, only used if <see cref="Dataframe.RowLabels"/> exist
        /// </summary>
        public string LabelSeperatorFromCorner = "+";
        /// <summary>
        /// Seperator for each level of <see cref="Dataframe.ColLabels"/> 
        /// </summary>
        public string LevelSeperator = "-";
        /// <summary>
        /// Seperator for each span in a level of a <see cref="Label"/> in a <see cref="LabelList"/> 
        /// </summary>
        public string SpanSeperator = "|";
        #endregion

        /// <summary>
        /// Empty constructor
        /// </summary>
        public DataframeColSettings() { }

        /// <summary>
        /// Create a deep copy of the current settings
        /// </summary>
        /// <returns>A new <see cref="DataframeColSettings"/></returns>
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

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "Cor,Spn,Lvl,Ele: " + LabelSeperatorFromCorner + " " + SpanSeperator + " " + LevelSeperator + " " + LabelSeperatorFromElements;
        }
        #endregion

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

    /// <summary>
    /// A class to enabling storing differing types of data and accessing data inside with labels
    /// <para>This class is limited with <see cref="DataframeLimits"/></para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Dataframe : MatrisBase<dynamic>, IDisposable
    {
        #region Private Encapsulated Fields
        /// <summary>
        /// Row dimension
        /// </summary>
        private int _row = -1;
        /// <summary>
        /// Column dimension
        /// </summary>
        private int _col = -1;
        /// <summary>
        /// Actual values to store
        /// </summary>
        private List<List<dynamic>> _values;
        /// <summary>
        /// Labels for rows
        /// </summary>
        private List<LabelList> _rowlabels;
        /// <summary>
        /// Labels for columns
        /// </summary>
        private List<LabelList> _collabels;

        private bool disposedValue;
        #endregion

        #region Public Properties
        /// <summary>
        /// Row dimension
        /// </summary>
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

        /// <summary>
        /// Column dimension
        /// </summary>
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

        /// <summary>
        /// Actual values to store
        /// </summary>
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

                            int lastcolsize = Math.Min(value[0].Count, collimit);

                            for (int i = 0; i < Math.Min(value.Count, (int)DataframeLimits.forRows); i++)
                            {
                                int colsize = Math.Min(value[i].Count, collimit);
                                if (lastcolsize != colsize)
                                {
                                    return;
                                }
                                temp.Add(new List<dynamic>());
                                for (int j = 0; j < colsize; j++)
                                {
                                    temp[i].Add(value[i][j]);
                                }
                            }
                            _values = temp;
                        }
                        else
                        {
                            int lastcolsize = value.Count == 0 ? 0 : value[0].Count;
                            for (int i = 0; i < value.Count; i++)
                            {
                                if (value[i].Count != lastcolsize)
                                {
                                    return;
                                }
                            }
                            _values = value;
                        }

                        _row = _values.Count;
                        _col = _row > 0 ? _values[0].Count : 0;
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

                    _row = 0;
                    _col = 0;
                }
            }
        }

        /// <summary>
        /// Labels for rows
        /// </summary>
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
                    if (_rowlabels != null)
                    {
                        foreach (LabelList l in _rowlabels)
                        {
                            l.Dispose();
                        }
                        _rowlabels.Clear();
                        _rowlabels = null;
                    }

                    if (value.Count > (int)DataframeLimits.forRowLabelLevels)
                    {
                        _rowlabels = value.GetRange(0, (int)DataframeLimits.forRowLabelLevels);
                    }
                    else
                    {
                        _rowlabels = value;
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
                        for (int i = 0; i < _rowlabels.Count; i++)
                        {
                            if (_rowlabels[i].TotalSpan != _row)
                            {
                                throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(_row, _rowlabels[i].TotalSpan));
                            }
                        }
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

        /// <summary>
        /// Labels for columns
        /// </summary>
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
                    if (_collabels != null)
                    {
                        foreach (LabelList l in _collabels)
                        {
                            l.Dispose();
                        }
                        _collabels.Clear();
                        _collabels = null;
                    }

                    if (value.Count > (int)DataframeLimits.forColLabelLevels)
                    {
                        _collabels = value.GetRange(0, (int)DataframeLimits.forColLabelLevels);
                    }
                    else
                    {
                        _collabels = value;
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
                        else
                        {
                            for (int i = 0; i < _collabels.Count; i++)
                            {
                                if (_collabels[i].TotalSpan != _col)
                                {
                                    throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(_col, _collabels[i].TotalSpan));
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
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Dataframe() { }

        /// <summary>
        /// Create a new instance with given values <paramref name="vals"/> and labels <paramref name="rowLabels"/>, <paramref name="colLabels"/>
        /// and use settings <paramref name="rowSettings"/>, <paramref name="colSettings"/>
        ///  and use delimiter <paramref name="delim"/> and new-line <paramref name="newline"/> while printing
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="delim"></param>
        /// <param name="newline"></param>
        /// <param name="rowLabels"></param>
        /// <param name="colLabels"></param>
        /// <param name="rowSettings"></param>
        /// <param name="colSettings"></param>
        public Dataframe(List<List<dynamic>> vals,
                         string delim = " ",
                         string newline = "\n",
                         List<LabelList> rowLabels = null,
                         List<LabelList> colLabels = null,
                         DataframeRowSettings rowSettings = null,
                         DataframeColSettings colSettings = null)
        {
            // Values and dimensions
            Values = vals;

            // Seperators
            Delimiter = delim;
            NewLine = newline;

            // Labels
            if (rowLabels == null)
            {
                //_rowlabels = new List<LabelList>() { new LabelList(_row, 1, "row_", 1) };
            }
            else
            {
                RowLabels = rowLabels;
                _rowlabels.Reverse();
            }
            if (colLabels == null)
            {
                _collabels = new List<LabelList>() { new LabelList(_col, 1, "col_", 1) };
            }
            else
            {
                ColLabels = colLabels;
                _collabels.Reverse();
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
        /// <summary>
        /// Create a dynamic <see cref="MatrisBase{T}"/> using a copy of <see cref="Dataframe.Values"/>
        /// </summary>
        /// <returns></returns>
        private MatrisBase<dynamic> Elements()
        {
            return new MatrisBase<dynamic>(_values);
        }

        /// <summary>
        /// Sum all values in an integer array within given range
        /// </summary>
        /// <param name="arr">Array to use</param>
        /// <param name="start">Starting index</param>
        /// <param name="end">Ending index exclusively</param>
        /// <returns>Sum of values within given range</returns>
        private int ArraySum(int[] arr, int start, int end)
        {
            int total = 0;
            for (int i = start; i < end; i++)
            {
                total += arr[i];
            }
            return total;
        }

        /// <summary>
        /// Get width of columns using <see cref="Dataframe.Values"/> and <see cref="Dataframe.ColLabels"/> widths
        /// </summary>
        /// <returns>Array of widths for each column</returns>
        private int[] GetColumnWidths()
        {
            // Element columns' widths
            int[] elementwidths = GetColumnWidths(Elements());
            int spanseplength = ColSettings.SpanSeperator.Length;

            if (_collabels != null)
            {
                int currentlabelindex;
                for (int j = 0; j < _col; j++)
                {
                    for (int k = 0; k < _collabels.Count; k++)
                    {
                        currentlabelindex = _collabels[k].GetLabelIndexAtSpan(j + 1);
                        Label current_label = _collabels[k].Labels[currentlabelindex];
                        int labellen = current_label.Value.Length + spanseplength;
                        // Label span is a single column
                        if (current_label.Span == 1)
                        {
                            // Is label name wider than values ?
                            if (labellen > elementwidths[j])
                            {
                                elementwidths[j] = labellen;
                            }
                        }
                        else // Spans over multiple columns
                        {
                            // If its shorter than current width, continue to next level
                            if (labellen < ArraySum(elementwidths, j, current_label.Span) + (spanseplength * (current_label.Span - 1)))
                            {
                                continue;
                            }
                            else // Label name was longer than the sum of widths it spans over
                            {
                                int newlength = (labellen / current_label.Span) + 1;
                                newlength = newlength == 0 ? 1 : newlength;
                                int carryover = 0;
                                int spanstart = 0;
                                for (int k2 = 0; k2 < currentlabelindex; k2++)
                                {
                                    spanstart += _collabels[k].Labels[k2].Span;
                                }
                                for (int j2 = spanstart; j2 < current_label.Span + spanstart; j2++)
                                {
                                    if (elementwidths[j2] >= newlength)
                                    {
                                        carryover += (elementwidths[j2] - newlength);
                                    }
                                    else
                                    {
                                        elementwidths[j2] = newlength - carryover;
                                        carryover = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return elementwidths;
        }

        /// <summary>
        /// Get widths for <see cref="Dataframe.RowLabels"/> levels
        /// </summary>
        /// <returns>Array of widths for each level of <see cref="Dataframe.RowLabels"/></returns>
        private int[] GetRowLabelColumnWidths()
        {
            // Row label columns' widths
            int lvlcount = (_rowlabels != null) ? _rowlabels.Count : 0;

            int[] allwidths = new int[lvlcount];

            for (int c = 0; c < lvlcount; c++)
            {
                List<Label> lbls = _rowlabels[c].Labels;
                int currentmax = 0;
                for (int l = 0; l < lbls.Count; l++)
                {
                    if (lbls[l].Value.Length > currentmax)
                    {
                        currentmax = lbls[l].Value.Length;
                    }
                }
                allwidths[c] = currentmax;
            }
            return allwidths;
        }

        /// <summary>
        /// Sets given <paramref name="res"/> string builder with widths given, using this insantace's labels
        /// </summary>
        /// <param name="res">String builder to append strings to</param>
        /// <param name="rowlbl_widths">Row label widths</param>
        /// <param name="col_widths">Column widths</param>
        private void SetElementsWithRowLabels(StringBuilder res,
                                              int[] rowlbl_widths,
                                              int[] col_widths)
        {
            string row_level_sep = RowSettings.LevelSeperator;
            string row_element_sep = RowSettings.LabelSeperatorFromElements;

            int colno, index;
            int rowlabellevel = _rowlabels.Count;
            List<dynamic> row = null;
            LabelList currentlist;

            int[] lastindex = new int[rowlabellevel];
            for (int t = 0; t < rowlabellevel; t++)
            {
                lastindex[t] = -1;
            }

            for (int i = 0; i < _row; i++)
            {
                for (int k = rowlabellevel - 1; k >= 0; k--)
                {
                    row = _values[i];
                    currentlist = _rowlabels[k];
                    index = currentlist.GetLabelIndexAtSpan(i + 1);
                    if (index == lastindex[k]) // Don't re-write same label again
                    {   // TO-DO: Create a placeholder for dataframes and use that here
                        res.Append(' ', rowlbl_widths[k] + 1);
                    }
                    else
                    {
                        res.Append(' ', rowlbl_widths[k] - currentlist.Labels[index].Value.Length + 1);
                        res.Append(currentlist.Labels[index].Value);
                        lastindex[k] = index;
                    }

                    if (k != 0)
                    {
                        res.Append(row_level_sep);
                    }
                }

                res.Append(row_element_sep);

                colno = 0;
                foreach (dynamic element in row)
                {
                    res.Append(' ', (col_widths[colno] - element.ToString().Length));
                    res.Append((string)element.ToString());

                    if (colno != Col - 1)
                    {
                        res.Append(Delimiter);
                    }

                    colno++;
                }
                res.Append(NewLine);
            }
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

            return new Dataframe(lis, Delimiter.ToString(), NewLine.ToString(), rowlbls, colbls, RowSettings.Copy(), ColSettings.Copy());
        }
        #endregion

        #region Public Method Overloads
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

            if (_rowlabels == null && _collabels == null)
            {
                return Elements().ToString();
            }

            StringBuilder res = new StringBuilder();

            // Column sizes
            int[] col_widths = GetColumnWidths();
            int[] rowlbl_widths = GetRowLabelColumnWidths();

            // Column labels row(s)
            if (_collabels != null)
            {
                string col_corner_sep = ColSettings.LabelSeperatorFromCorner;
                string col_span_sep = ColSettings.SpanSeperator;
                string col_level_sep = ColSettings.LevelSeperator;
                int colrepeat = ArraySum(col_widths, 0, _col) + ((_col - 1) * col_span_sep.Length);
                col_level_sep = new string(col_level_sep[0], colrepeat);

                string col_element_sep = ColSettings.LabelSeperatorFromElements;

                // No row labels
                if (rowlbl_widths.Length == 0)
                {
                    // Column label rows first
                    int colLabelLength = _collabels.Count;
                    for (int i = colLabelLength - 1; i >= 0; i--)
                    {
                        LabelList labelList = _collabels[i];
                        for (int j = 0; j < labelList.Length; j++)
                        {
                            Label lbl = labelList.Labels[j];

                            int spanstart = 0;
                            for (int k2 = 0; k2 < j; k2++)
                            {
                                spanstart += labelList.Labels[k2].Span;
                            }

                            int space_count = ArraySum(col_widths, spanstart, spanstart + lbl.Span) - lbl.Value.Length + (lbl.Span - 1);

                            res.Append(' ', space_count); // Left align for now
                            res.Append(lbl.Value);

                            if (j != labelList.Length - 1)
                            {
                                res.Append(col_span_sep);
                            }
                        }
                        res.Append(NewLine);

                        if (i != 0)
                        {
                            res.Append(col_level_sep);
                            res.Append(NewLine);
                        }
                    }

                    res.Append(col_element_sep[0], colrepeat);
                    res.Append(NewLine);

                    int colno;
                    foreach (List<dynamic> row in _values)
                    {
                        colno = 0;
                        foreach (dynamic element in row)
                        {
                            res.Append(' ', (col_widths[colno] - element.ToString().Length));
                            res.Append((string)element.ToString());

                            if (colno != Col - 1)
                            {
                                res.Append(Delimiter);
                            }

                            colno++;
                        }
                        res.Append(NewLine);
                    }
                }
                else // Column and row labels
                {
                    string row_corner_sep = RowSettings.LabelSeperatorFromCorner;
                    string row_span_sep = RowSettings.SpanSeperator;
                    string row_element_sep = RowSettings.LabelSeperatorFromElements;
                    int rowlabelextra = ArraySum(rowlbl_widths, 0, rowlbl_widths.Length) + (rowlbl_widths.Length - 1) * (row_span_sep.Length + 1) + row_element_sep.Length + col_corner_sep.Length - 1;

                    // Column label rows first
                    int colLabelLength = _collabels.Count;
                    for (int i = colLabelLength - 1; i >= 0; i--)
                    {
                        res.Append(' ', rowlabelextra);
                        res.Append(col_corner_sep);

                        LabelList labelList = _collabels[i];
                        for (int j = 0; j < labelList.Length; j++)
                        {
                            Label lbl = labelList.Labels[j];

                            int spanstart = 0;
                            for (int k2 = 0; k2 < j; k2++)
                            {
                                spanstart += labelList.Labels[k2].Span;
                            }

                            int space_count = ArraySum(col_widths, spanstart, spanstart + lbl.Span) - lbl.Value.Length + (lbl.Span - 1);

                            res.Append(' ', space_count); // Left align for now
                            res.Append(lbl.Value);

                            if (j != labelList.Length - 1)
                            {
                                res.Append(col_span_sep);
                            }
                        }
                        res.Append(NewLine);

                        if (i != 0)
                        {
                            res.Append(' ', rowlabelextra);
                            res.Append(col_corner_sep);
                            res.Append(col_level_sep);
                            res.Append(NewLine);
                        }
                    }

                    res.Append(row_corner_sep[0], rowlabelextra + 1);
                    res.Append(col_element_sep[0], colrepeat);
                    res.Append(NewLine);

                    SetElementsWithRowLabels(res, rowlbl_widths, col_widths);
                }
            }
            else // No column labels
            {
                if (_rowlabels == null) // No labels at all
                {
                    return Elements().ToString();
                }
                else // Row labels only
                {
                    SetElementsWithRowLabels(res, rowlbl_widths, col_widths);
                }
            }
            return res.ToString();
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "df(" + _row + "," + _col + "):" + ToString();
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
