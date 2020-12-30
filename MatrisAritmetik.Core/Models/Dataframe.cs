using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Row label settings to use for <see cref="Dataframe"/>
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class DataframeRowSettings : IDisposable
    {
        private bool disposedValue;

        #region Settings
        /// <summary>
        /// Seperator between elements and labels
        /// </summary>
        private string labelSeperatorFromElements = "||";

        /// <summary>
        /// Seperator from top-most
        /// </summary>
        private string labelSeperatorFromCorner = "+";

        /// <summary>
        /// Seperator for each level of <see cref="Dataframe.GetRowLabels()"/> 
        /// </summary>
        private string levelSeperator = "|";

        /// <summary>
        /// Seperator for each span
        /// </summary>
        private string spanSeperator = "-";

        #endregion

        #region Getters and Setters

        /// <summary>
        /// Get seperator for the right-most side of <see cref="Dataframe.GetRowLabels()"/> rows from <see cref="Dataframe.GetValues()"/>
        /// </summary>
        public string GetLabelSeperatorFromElements()
        {
            return labelSeperatorFromElements;
        }

        /// <summary>
        /// Set seperator for the right-most side of <see cref="Dataframe.GetRowLabels()"/> rows from <see cref="Dataframe.GetValues()"/>
        /// </summary>
        public void SetLabelSeperatorFromElements(string value)
        {
            labelSeperatorFromElements = value;
        }

        /// <summary>
        /// Set seperator for the top-most of the <see cref="Dataframe.GetRowLabels()"/>, only used if <see cref="Dataframe.GetColLabels()"/> exist
        /// </summary>
        public string GetLabelSeperatorFromCorner()
        {
            return labelSeperatorFromCorner;
        }

        /// <summary>
        /// Set seperator for the top-most of the <see cref="Dataframe.GetRowLabels()"/>, only used if <see cref="Dataframe.GetColLabels()"/> exist
        /// </summary>
        public void SetLabelSeperatorFromCorner(string value)
        {
            labelSeperatorFromCorner = value;
        }

        /// <summary>
        /// Get seperator for between each of <see cref="Dataframe.GetRowLabels()"/> columns
        /// </summary>
        public string GetLevelSeperator()
        {
            return levelSeperator;
        }

        /// <summary>
        /// Set seperator for between each of <see cref="Dataframe.GetRowLabels()"/> columns
        /// </summary>
        public void SetLevelSeperator(string value)
        {
            levelSeperator = value;
        }

        /// <summary>
        /// Get seperator for each span in a level of a <see cref="Label"/> in a <see cref="LabelList"/> 
        /// </summary>
        public string GetSpanSeperator()
        {
            return spanSeperator;
        }

        /// <summary>
        /// Set seperator for each span in a level of a <see cref="Label"/> in a <see cref="LabelList"/> 
        /// </summary>
        public void SetSpanSeperator(string value)
        {
            spanSeperator = value;
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public DataframeRowSettings() { }

        /// <summary>
        /// Settings from list, seperator order: [from_elements, from_corner, level, span] 
        /// </summary>
        public DataframeRowSettings(List<string> lis)
        {
            if (lis == null)
            {
                return;
            }
            else if (lis.Count == 4)
            {
                labelSeperatorFromElements = lis[0] ?? labelSeperatorFromElements;
                labelSeperatorFromCorner = lis[1] ?? labelSeperatorFromCorner;
                levelSeperator = lis[2] ?? levelSeperator;
                spanSeperator = lis[3] ?? spanSeperator;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a deep copy of the current settings
        /// </summary>
        /// <returns>A new <see cref="DataframeRowSettings"/></returns>
        public DataframeRowSettings Copy()
        {
            return new DataframeRowSettings()
            {
                labelSeperatorFromCorner = GetLabelSeperatorFromCorner(),
                labelSeperatorFromElements = GetLabelSeperatorFromElements(),
                levelSeperator = GetLevelSeperator(),
                spanSeperator = GetSpanSeperator()
            };
        }

        /// <summary>
        /// Return seperators in order: [from_elements, from_corner, level, span] 
        /// </summary>
        /// <returns></returns>
        public List<string> GetSeperators()
        {
            return new List<string>() { labelSeperatorFromElements, labelSeperatorFromCorner, levelSeperator, spanSeperator };
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "Cor,Spn,Lvl,Ele: " + GetLabelSeperatorFromCorner() + " " + GetSpanSeperator() + " " + GetLevelSeperator() + " " + GetLabelSeperatorFromElements();
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    labelSeperatorFromElements = string.Empty;
                    labelSeperatorFromCorner = string.Empty;
                    levelSeperator = string.Empty;
                    spanSeperator = string.Empty;
                }

                SetLabelSeperatorFromElements(null);
                SetLabelSeperatorFromCorner(null);
                SetLevelSeperator(null);
                SetSpanSeperator(null);
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
        private bool disposedValue;

        #region Settings
        /// <summary>
        /// Seperator between elements and labels
        /// </summary>
        private string labelSeperatorFromElements = "=";

        /// <summary>
        /// Seperator from left-most
        /// </summary>
        private string labelSeperatorFromCorner = "+";

        /// <summary>
        /// Seperator for each level of <see cref="Dataframe.GetColLabels()"/> 
        /// </summary>
        private string levelSeperator = "-";

        /// <summary>
        /// Seperator for each span
        /// </summary>
        private string spanSeperator = "|";

        #endregion

        #region Getters and Setters

        /// <summary>
        /// Get seperator for the bottom-most side of <see cref="Dataframe.GetColLabels()"/> rows from <see cref="Dataframe.GetValues()"/>
        /// </summary>
        public string GetLabelSeperatorFromElements()
        {
            return labelSeperatorFromElements;
        }

        /// <summary>
        /// Set seperator for the bottom-most side of <see cref="Dataframe.GetColLabels()"/> rows from <see cref="Dataframe.GetValues()"/>
        /// </summary>
        public void SetLabelSeperatorFromElements(string value)
        {
            labelSeperatorFromElements = value;
        }

        /// <summary>
        /// Get seperator for the left-most of the <see cref="Dataframe.GetColLabels()"/>, only used if <see cref="Dataframe.GetRowLabels()"/> exist
        /// </summary>
        public string GetLabelSeperatorFromCorner()
        {
            return labelSeperatorFromCorner;
        }

        /// <summary>
        /// Set seperator for the left-most of the <see cref="Dataframe.GetColLabels()"/>, only used if <see cref="Dataframe.GetRowLabels()"/> exist
        /// </summary>
        public void SetLabelSeperatorFromCorner(string value)
        {
            labelSeperatorFromCorner = value;
        }

        /// <summary>
        /// Get seperator for between each of <see cref="Dataframe.GetColLabels()"/> rows
        /// </summary>
        public string GetLevelSeperator()
        {
            return levelSeperator;
        }

        /// <summary>
        /// Set seperator for between each of <see cref="Dataframe.GetColLabels()"/> rows
        /// </summary>
        public void SetLevelSeperator(string value)
        {
            levelSeperator = value;
        }

        /// <summary>
        /// Get seperator for each span in a level of a <see cref="Label"/> in a <see cref="LabelList"/> 
        /// </summary>
        public string GetSpanSeperator()
        {
            return spanSeperator;
        }

        /// <summary>
        /// Set seperator for each span in a level of a <see cref="Label"/> in a <see cref="LabelList"/> 
        /// </summary>
        public void SetSpanSeperator(string value)
        {
            spanSeperator = value;
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public DataframeColSettings() { }
        /// <summary>
        /// Settings from list, seperator order: [from_elements, from_corner, level, span] 
        /// </summary>
        public DataframeColSettings(List<string> lis)
        {
            if (lis == null)
            {
                return;
            }
            else if (lis.Count == 4)
            {
                labelSeperatorFromElements = lis[0] ?? labelSeperatorFromElements;
                labelSeperatorFromCorner = lis[1] ?? labelSeperatorFromCorner;
                levelSeperator = lis[2] ?? levelSeperator;
                spanSeperator = lis[3] ?? spanSeperator;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a deep copy of the current settings
        /// </summary>
        /// <returns>A new <see cref="DataframeColSettings"/></returns>
        public DataframeColSettings Copy()
        {
            return new DataframeColSettings()
            {
                labelSeperatorFromCorner = GetLabelSeperatorFromCorner(),
                labelSeperatorFromElements = GetLabelSeperatorFromElements(),
                levelSeperator = GetLevelSeperator(),
                spanSeperator = GetSpanSeperator()
            };
        }

        /// <summary>
        /// Return seperators in order: [from_elements, from_corner, level, span] 
        /// </summary>
        /// <returns></returns>
        public List<string> GetSeperators()
        {
            return new List<string>() { labelSeperatorFromElements, labelSeperatorFromCorner, levelSeperator, spanSeperator };
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "ColSettings: " + GetLabelSeperatorFromCorner() + " " + GetSpanSeperator() + " " + GetLevelSeperator() + " " + GetLabelSeperatorFromElements();
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    labelSeperatorFromElements = string.Empty;
                    labelSeperatorFromCorner = string.Empty;
                    levelSeperator = string.Empty;
                    spanSeperator = string.Empty;
                }

                SetLabelSeperatorFromElements(null);
                SetLabelSeperatorFromCorner(null);
                SetLevelSeperator(null);
                SetSpanSeperator(null);
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
    public class Dataframe : MatrisBase<object>
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
        private List<List<object>> _values;
        /// <summary>
        /// Labels for rows
        /// </summary>
        private List<LabelList> _rowlabels;
        /// <summary>
        /// Labels for columns
        /// </summary>
        private List<LabelList> _collabels;
        /// <summary>
        /// Row label settings
        /// </summary>
        private DataframeRowSettings rowSettings;
        /// <summary>
        /// Column label settings
        /// </summary>
        private DataframeColSettings colSettings;

        private bool disposedValue;
        #endregion

        #region Public Properties, Getters and Setters
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
        /// Alternative way to access values directly
        /// </summary>
        public override List<List<object>> Values => _values;

        /// <summary>
        /// Get column label settings
        /// </summary>
        /// <returns>Current column settings</returns>
        public DataframeColSettings GetColSettings()
        {
            return colSettings;
        }

        /// <summary>
        /// Set new settings for column labels
        /// </summary>
        /// <param name="value">New settings</param>
        public void SetColSettings(DataframeColSettings value)
        {
            colSettings = value;
        }

        /// <summary>
        /// Get row label settings
        /// </summary>
        /// <returns>Current row settings</returns>
        public DataframeRowSettings GetRowSettings()
        {
            return rowSettings;
        }

        /// <summary>
        /// Set new settings for row labels
        /// </summary>
        /// <param name="value">New settings</param>
        public void SetRowSettings(DataframeRowSettings value)
        {
            rowSettings = value;
        }

        /// <summary>
        /// Get values to stored
        /// </summary>
        public override List<List<object>> GetValues()
        {
            if (_values != null)
            {
                if (_values.Count != 0 && _values.Count * _values[0].Count > (int)DataframeLimits.forSize)
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

        /// <summary>
        /// Set values to store
        /// </summary>
        public override void SetValues(List<List<object>> value)
        {
            if (value != null)
            {
                if (value.Count != 0 && value.Count * value[0].Count > (int)DataframeLimits.forSize)
                {
                    List<List<object>> temp = new List<List<object>>();
                    int collimit = (int)DataframeLimits.forCols;

                    int lastcolsize = Math.Min(value[0].Count, collimit);

                    for (int i = 0; i < Math.Min(value.Count, (int)DataframeLimits.forRows); i++)
                    {
                        int colsize = Math.Min(value[i].Count, collimit);
                        if (lastcolsize != colsize)
                        {
                            return;
                        }
                        temp.Add(new List<object>());
                        for (int j = 0; j < colsize; j++)
                        {
                            if (float.TryParse(value[i][j].ToString(), out float res))
                            {
                                temp[i].Add(res);
                            }
                            else if (value[i][j] is None || value[i][j] is null)
                            {
                                _values[i].Add(new None());
                            }
                            else
                            {
                                temp[i].Add(value[i][j].ToString());
                            }
                        }
                    }
                    _values = temp;
                }
                else
                {
                    _values = new List<List<object>>() { };
                    int lastcolsize = value.Count == 0 ? 0 : value[0].Count;
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (value[i].Count != lastcolsize)
                        {
                            _values = new List<List<object>>() { };
                            return;
                        }
                        else
                        {
                            _values.Add(new List<object>());
                            for (int j = 0; j < value[i].Count; j++)
                            {
                                if (float.TryParse(value[i][j].ToString(), out float res))
                                {
                                    _values[i].Add(res);
                                }
                                else if (value[i][j] is None || value[i][j] is null)
                                {
                                    _values[i].Add(new None());
                                }
                                else
                                {
                                    _values[i].Add(value[i][j].ToString());
                                }
                            }
                        }
                    }
                }

                Row = _values.Count;
                Col = Row > 0 ? _values[0].Count : 0;
            }
            else
            {
                if (_values != null)
                {
                    foreach (List<object> l in _values)
                    {
                        l.Clear();
                    }
                    _values.Clear();
                    _values = null;
                }

                Row = 0;
                Col = 0;
            }

        }

        /// <summary>
        /// Get labels for rows
        /// </summary>
        public List<LabelList> GetRowLabels()
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
                            if (_rowlabels[i].TotalSpan != Row)
                            {
                                throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(Row, _rowlabels[i].TotalSpan));
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

        /// <summary>
        /// Set labels for rows
        /// </summary>
        public void SetRowLabels(List<LabelList> value)
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
                            if (value[i].TotalSpan != Row)
                            {
                                throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(Row, value[i].TotalSpan));
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
                        if (_rowlabels[i].TotalSpan != Row)
                        {
                            throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(Row, _rowlabels[i].TotalSpan));
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

        /// <summary>
        /// Get labels for columns
        /// </summary>
        public List<LabelList> GetColLabels()
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
                            if (_collabels[i].TotalSpan != Col)
                            {
                                throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(Col, _collabels[i].TotalSpan));
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

        /// <summary>
        /// Set labels for columns
        /// </summary>
        public void SetColLabels(List<LabelList> value)
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
                            if (value[i].TotalSpan != Col)
                            {
                                throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(Col, value[i].TotalSpan));
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
                            if (_collabels[i].TotalSpan != Col)
                            {
                                throw new Exception(CompilerMessage.DF_TOTALSPAN_UNMATCH(Col, _collabels[i].TotalSpan));
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

        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Dataframe() { }

        /// <summary>
        /// Create a dataframe with given <paramref name="vals"/> and <paramref name="options"/>
        /// </summary>
        /// <param name="vals">Values to use</param>
        /// <param name="options">Option strings</param>
        public Dataframe(List<List<object>> vals, List<string> options)
        {
            if (options == null || options.Count == 0)
            {
                // Values and dimensions
                SetValues(vals);
                SetRowSettings(new DataframeRowSettings());
                SetColSettings(new DataframeColSettings());
            }
            else
            {
                ApplyOptions(vals, options);
            }

        }

        /// <summary>
        /// Create a new instance with given values <paramref name="vals"/> and labels <paramref name="rowLabels"/>, <paramref name="colLabels"/>
        /// and use settings <paramref name="rowSettings"/>, <paramref name="colSettings"/>
        ///  and use delimiter <paramref name="delim"/> and new-line <paramref name="newline"/> while printing
        /// </summary>
        /// <param name="vals">Values to store</param>
        /// <param name="delim">Delimiter used while given <paramref name="vals"/> were being read</param>
        /// <param name="newline">New-line character used while <paramref name="vals"/> were being read</param>
        /// <param name="rowLabels">Labels for rows, null for default names with single column span per label</param>
        /// <param name="colLabels">Labels for columns, null for default names with single column span per label</param>
        /// <param name="rowSettings">Settings for row labels, null for default <see cref="DataframeRowSettings"/> instance</param>
        /// <param name="colSettings">Settings for column labels, null for default <see cref="DataframeColSettings"/> instance</param>
        public Dataframe(List<List<object>> vals,
                         string delim = " ",
                         string newline = "\n",
                         List<LabelList> rowLabels = null,
                         List<LabelList> colLabels = null,
                         DataframeRowSettings rowSettings = null,
                         DataframeColSettings colSettings = null,
                         bool forceLabelsWhenNull = false)
        {
            // Values and dimensions
            SetValues(vals);

            // Seperators
            Delimiter = delim;
            NewLine = newline;

            // Labels
            if (rowLabels == null)
            {
                if (forceLabelsWhenNull)
                {
                    SetRowLabels(new List<LabelList>() { new LabelList(Row, 1, "row_", 1) });
                }
            }
            else
            {
                SetRowLabels(rowLabels);
                GetRowLabels().Reverse();
            }
            if (colLabels == null)
            {
                if (forceLabelsWhenNull)
                {
                    SetColLabels(new List<LabelList>() { new LabelList(Col, 1, "col_", 1) });
                }
            }
            else
            {
                SetColLabels(colLabels);
                GetColLabels().Reverse();
            }

            // Settings
            if (rowSettings != null)
            {
                SetRowSettings(rowSettings);
            }
            else
            {
                SetRowSettings(new DataframeRowSettings());
            }
            if (colSettings != null)
            {
                SetColSettings(colSettings);
            }
            else
            {
                SetColSettings(new DataframeColSettings());
            }

        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Get width of columns using <see cref="Dataframe.GetValues()"/> and <see cref="Dataframe.GetColLabels()"/> widths
        /// </summary>
        /// <returns>Array of widths for each column</returns>
        private int[] GetColumnWidths()
        {
            int[] elementwidths = GetColumnWidths(this);
            int spanseplength = GetColSettings().GetSpanSeperator().Length;

            if (GetColLabels() != null)
            {
                int currentlabelindex;
                for (int j = 0; j < Col; j++)
                {
                    for (int k = 0; k < GetColLabels().Count; k++)
                    {
                        currentlabelindex = GetColLabels()[k].GetLabelIndexAtSpan(j + 1);
                        Label current_label = GetColLabels()[k].Labels[currentlabelindex];
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
                            if (labellen < int.Parse(ArrayMethods.ArraySum(new List<int>(elementwidths), j, current_label.Span).ToString()) + (spanseplength * (current_label.Span - 1)))
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
                                    spanstart += GetColLabels()[k].Labels[k2].Span;
                                }
                                for (int j2 = spanstart; j2 < current_label.Span + spanstart; j2++)
                                {
                                    if (elementwidths[j2] >= newlength)
                                    {
                                        carryover += elementwidths[j2] - newlength;
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
        /// Get widths for <see cref="Dataframe.GetRowLabels()"/> levels
        /// </summary>
        /// <returns>Array of widths for each level of <see cref="Dataframe.GetRowLabels()"/></returns>
        private int[] GetRowLabelColumnWidths()
        {
            if (GetRowLabels() == null)
            {
                return Array.Empty<int>();
            }
            // Row label columns' widths
            int lvlcount = GetRowLabels().Count;

            int[] allwidths = new int[lvlcount];

            for (int c = 0; c < lvlcount; c++)
            {
                List<Label> lbls = GetRowLabels()[c].Labels ?? new List<Label>();
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
            string row_level_sep = GetRowSettings().GetLevelSeperator();
            string row_element_sep = GetRowSettings().GetLabelSeperatorFromElements();

            int repeat;
            int colno, index;
            int rowlabellevel = GetRowLabels() == null ? 0 : GetRowLabels().Count;
            List<object> row;
            LabelList currentlist;

            int[] lastindex = new int[rowlabellevel];
            for (int t = 0; t < rowlabellevel; t++)
            {
                lastindex[t] = -1;
            }

            for (int i = 0; i < Row; i++)
            {
                row = GetValues()[i];
                for (int k = rowlabellevel - 1; k >= 0; k--)
                {
                    currentlist = GetRowLabels()[k];
                    index = currentlist.GetLabelIndexAtSpan(i + 1);
                    if (index == lastindex[k]) // Don't re-write same label again
                    {   // TO-DO: Create a placeholder for dataframes and use that here
                        repeat = rowlbl_widths[k] + 1;
                        if (repeat > 0)
                        {
                            res.Append(' ', repeat);
                        }
                    }
                    else
                    {
                        repeat = rowlbl_widths[k] - currentlist.Labels[index].Value.Length + 1;
                        if (repeat > 0)
                        {
                            res.Append(' ', repeat);
                        }
                        res.Append(currentlist.Labels[index].Value);
                        lastindex[k] = index;
                    }

                    if (k != 0)
                    {
                        res.Append(row_level_sep);
                    }
                }

                if (rowlabellevel != 0)
                {
                    res.Append(row_element_sep);
                }

                colno = 0;
                foreach (dynamic element in row)
                {
                    repeat = col_widths[colno] - element.ToString().Length;
                    if (repeat > 0)
                    {
                        res.Append(' ', repeat);
                    }
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
        /// Apply given options with given values to the dataframe, override labels with 'null' first if desired
        /// </summary>
        /// <param name="vals">Values to use</param>
        /// <param name="options">Option strings to use</param>
        /// <param name="overrideExisting">If true, column and row labels will be overriden 'null' no matter the rest of the paramaters</param>
        public void ApplyOptions(List<List<object>> vals, List<string> options, bool overrideExisting = false)
        {
            if (overrideExisting)
            {
                SetColLabels(null);
                SetRowLabels(null);
            }

            if (options == null || options.Count == 0
                || vals == null || vals.Count == 0
                || vals[0] == null || vals[0].Count == 0)
            {
                return;
            }

            bool firstRowAsLabel = options.Contains("use_row_as_lbl");
            bool addColLabels = options.Contains("add_col_labels");
            bool addRowLabels = options.Contains("add_row_labels");

            if (firstRowAsLabel)
            {
                if (vals.Count == 1)
                {
                    SetValues(vals);
                    SetColLabels(new List<LabelList>() { new LabelList(vals[0]) });
                    if (addRowLabels)
                    {
                        SetRowLabels(new List<LabelList>() { new LabelList(Row, 1, "row_", 1) });
                    }
                }
                else
                {
                    SetValues(vals.GetRange(1, vals.Count - 1));
                    SetColLabels(new List<LabelList>() { new LabelList(vals[0]) });
                    if (addRowLabels)
                    {
                        SetRowLabels(new List<LabelList>() { new LabelList(Row, 1, "row_", 1) });
                    }
                }
            }
            else
            {
                SetValues(vals);

                if (addRowLabels)
                {
                    SetRowLabels(new List<LabelList>() { new LabelList(Row, 1, "row_", 1) });
                }
                if (addColLabels)
                {
                    SetColLabels(new List<LabelList>() { new LabelList(Col, 1, "col_", 1) });
                }
            }
            SetRowSettings(new DataframeRowSettings());
            SetColSettings(new DataframeColSettings());
        }

        /// <summary>
        /// Get a deep copy of given list of label-list
        /// </summary>
        /// <param name="lis">Label-list list</param>
        /// <returns>Deep copy of given list or null if <paramref name="lis"/> was null</returns>
        public List<LabelList> GetCopyOfLabels(List<LabelList> lis)
        {
            if (lis == null)
            {
                return lis;
            }

            List<LabelList> lbls = new List<LabelList>();
            foreach (LabelList lbl in lis)
            {
                lbls.Add(lbl.Copy());
            }

            return lbls;
        }

        #endregion

        #region Public Method Overloads
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
                    lis[i].Add((dynamic)_values[i][j]);
                }
            }

            return new Dataframe(lis,
                                 Delimiter.ToString(),
                                 NewLine.ToString(),
                                 GetCopyOfLabels(GetRowLabels()),
                                 GetCopyOfLabels(GetColLabels()),
                                 GetRowSettings().Copy(),
                                 GetColSettings().Copy());
        }

        /// <summary>
        /// Creates a smaller printable dataframe for larger dataframes, using only values in the corners
        /// </summary>
        /// <param name="rowEachCorner">Amount of rows for each corner</param>
        /// <param name="colEachCorner">Amount of columns for each corner</param>
        /// <param name="filler">Value to be used between corners</param>
        /// <returns>A smaller dataframe with <paramref name="filler"/> dividing it to 4 smaller dataframes</returns>
        public override dynamic CornerMatrix(int rowEachCorner = -1,
                                             int colEachCorner = -1,
                                             string filler = "...")
        {
            if (Row == 0 || Col == 0)
            {
                return new Dataframe();
            }

            List<List<object>> smallerList = new List<List<object>>();

            if (rowEachCorner <= 0)
            {
                rowEachCorner = Math.Min((int)((float)Row * 0.33), 4);
                rowEachCorner = rowEachCorner == 0 ? Row : rowEachCorner;
            }
            if (colEachCorner <= 0)
            {
                colEachCorner = Math.Min((int)((float)Col * 0.33), 4);
                colEachCorner = colEachCorner == 0 ? Col : colEachCorner;
            }

            // No reduction
            if (((float)rowEachCorner * 2.0) + 1.0 > (float)Row && ((float)colEachCorner * 2.0) + 1.0 > (float)Col)
            {
                return new Dataframe(_values, rowSettings: GetRowSettings().Copy(), colSettings: GetColSettings().Copy());
            }
            // Only reduce columns
            else if (((float)rowEachCorner * 2.0) + 1.0 > (float)Row)
            {
                // Start reducing columns
                for (int i = 0; i < Row; i++)
                {
                    smallerList.Add(new List<object>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[i].Add(_values[i][left].ToString());
                    }

                    smallerList[i].Add(filler);

                    for (int right = Col - colEachCorner; right < Col; right++)
                    {
                        smallerList[i].Add(_values[i][right].ToString());
                    }
                }

            }
            // Only reduce rows
            else if (((float)colEachCorner * 2.0) + 1.0 > (float)Col)
            {
                // Start reducing rows
                // Upper half
                for (int u = 0; u < rowEachCorner; u++)
                {
                    smallerList.Add(new List<object>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[u].Add(_values[u][left].ToString());
                    }
                }

                smallerList.Add(new List<object>());
                for (int j = 0; j < Col; j++)
                {
                    smallerList[rowEachCorner].Add(filler);
                }

                int rrowind = rowEachCorner + 1;
                // Bottom half
                for (int i = Row - rowEachCorner; i < Row; i++)
                {
                    smallerList.Add(new List<object>());
                    for (int bleft = 0; bleft < colEachCorner; bleft++)
                    {
                        smallerList[rrowind].Add(_values[i][bleft].ToString());
                    }
                    rrowind++;
                }

            }
            else
            {
                // Reduce both rows and columns
                // Upper half
                for (int u = 0; u < rowEachCorner; u++)
                {
                    smallerList.Add(new List<object>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[u].Add(_values[u][left].ToString());
                    }

                    smallerList[u].Add(filler);

                    for (int right = Col - colEachCorner; right < Col; right++)
                    {
                        smallerList[u].Add(_values[u][right].ToString());
                    }
                }

                smallerList.Add(new List<object>());
                for (int j = 0; j < (colEachCorner * 2) + 1; j++)
                {
                    smallerList[rowEachCorner].Add(filler);
                }

                int rowind = rowEachCorner + 1;
                // Bottom half
                for (int i = Row - rowEachCorner; i < Row; i++)
                {
                    smallerList.Add(new List<object>());
                    for (int bleft = 0; bleft < colEachCorner; bleft++)
                    {
                        smallerList[rowind].Add(_values[i][bleft].ToString());
                    }

                    smallerList[rowind].Add(filler);

                    for (int bright = Col - colEachCorner; bright < Col; bright++)
                    {
                        smallerList[rowind].Add(_values[i][bright].ToString());
                    }
                    rowind++;
                }
            }
            return new Dataframe(smallerList, rowSettings: GetRowSettings().Copy(), colSettings: GetColSettings().Copy());
        }
        /// <summary>
        /// Apply exponentiation operation over each value in the matrix
        /// </summary>
        /// <param name="n">Exponential value</param>
        /// <returns>A new matrix raised to the power of <paramref name="n"/></returns>
        public override MatrisBase<object> Power(dynamic n)
        {
            if (n is null)
            {
                return null;
            }

            if (float.TryParse(n.ToString(), out float res))
            {
                n = res;
            }
            else
            {
                return null;
            }

            int nr = Row;
            int nc = Col;
            List<List<object>> newlist = new List<List<object>>();

            for (int i = 0; i < nr; i++)
            {
                newlist.Add(new List<object>());
                for (int j = 0; j < nc; j++)
                {
                    if (float.TryParse(GetValues()[i][j].ToString(), out float r))
                    {
                        newlist[i].Add((float)Math.Pow(r, n));
                    }
                    else
                    {
                        newlist[i].Add(float.NaN);
                    }
                }
            }
            return new Dataframe(newlist,
                                 Delimiter,
                                 NewLine,
                                 null,
                                 GetCopyOfLabels(GetColLabels()),
                                 null,
                                 GetColSettings().Copy());
        }

        /// <summary>
        /// Creates a dataframe detail summary
        /// </summary>
        /// <param name="name">Name given for this dataframe</param>
        /// <returns>A string with dataframe name, seed(if exists), dimensions and values</returns>
        public override string Details(string name = "")
        {
            string seed_str = CreatedFromSeed ? $"Seed: {Seed}\n" : string.Empty;
            return $"Veri Tablosu: {name}\n"
                   + seed_str
                   + $"Boyut: {Row}x{Col}\n"
                   + "Elementler:\n"
                   + ToString();
        }

        /// <summary>
        /// Get a new row-dataframe from given row index <paramref name="r"/> with base <paramref name="based"/>
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="based">Index base</param>
        /// <returns>New row-dataframe</returns>
        public override MatrisBase<object> RowMat(int r,
                                                  int based = 1)
        {
            List<List<object>> listrow = new List<List<object>>() { new List<object>() };
            for (int j = 0; j < Col; j++)
            {
                listrow[0].Add((dynamic)GetValues()[r - based][j]);
            }

            return new Dataframe(listrow);
        }

        /// <summary>
        /// Get a new column-dataframe from given column index <paramref name="c"/> with base <paramref name="based"/>
        /// </summary>
        /// <param name="c">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>New column-dataframe</returns>
        public override MatrisBase<object> ColMat(int c,
                                                  int based = 1)
        {
            List<List<object>> listcol = new List<List<object>>();
            for (int i = 0; i < Row; i++)
            {
                listcol.Add(new List<object>() { (dynamic)GetValues()[i][c - based] });
            }

            return new Dataframe(listcol);
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
            /*
            if (GetRowLabels() == null && GetColLabels() == null)
            {
                using MatrisBase<dynamic> matrisBase = new MatrisBase<dynamic>(_values);
                return matrisBase.ToString();
            }
            */
            StringBuilder res = new StringBuilder();

            // Column sizes
            int[] col_widths = GetColumnWidths();
            int[] rowlbl_widths = GetRowLabelColumnWidths();

            // Column labels row(s)
            if (GetColLabels() != null)
            {
                string col_corner_sep = GetColSettings().GetLabelSeperatorFromCorner();
                string col_span_sep = GetColSettings().GetSpanSeperator();
                string col_level_sep = GetColSettings().GetLevelSeperator();
                int colrepeat = int.Parse(ArrayMethods.ArraySum(new List<int>(col_widths), 0, Col).ToString())
                                + ((Col - 1) * col_span_sep.Length);

                col_level_sep = new string(col_level_sep[0], colrepeat);

                string col_element_sep = GetColSettings().GetLabelSeperatorFromElements();

                // No row labels
                if (rowlbl_widths.Length == 0)
                {
                    // Column label rows first
                    int colLabelLength = GetColLabels().Count;
                    for (int i = colLabelLength - 1; i >= 0; i--)
                    {
                        LabelList labelList = GetColLabels()[i];
                        for (int j = 0; j < labelList.Length; j++)
                        {
                            Label lbl = labelList.Labels[j];

                            int spanstart = 0;
                            for (int k2 = 0; k2 < j; k2++)
                            {
                                spanstart += labelList.Labels[k2].Span;
                            }

                            int space_count = int.Parse(ArrayMethods.ArraySum(new List<int>(col_widths), spanstart, spanstart + lbl.Span).ToString())
                                              - lbl.Value.Length + (lbl.Span - 1);

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
                            res.Append(' ', col_widths[colno] - element.ToString().Length);
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
                    string row_corner_sep = GetRowSettings().GetLabelSeperatorFromCorner();
                    string row_span_sep = GetRowSettings().GetSpanSeperator();
                    string row_element_sep = GetRowSettings().GetLabelSeperatorFromElements();
                    int rowlabelextra = int.Parse(ArrayMethods.ArraySum(new List<int>(rowlbl_widths), 0, rowlbl_widths.Length).ToString())
                                        + ((rowlbl_widths.Length - 1) * (row_span_sep.Length + 1)) + row_element_sep.Length + col_corner_sep.Length - 1;

                    // Column label rows first
                    int colLabelLength = GetColLabels().Count;
                    for (int i = colLabelLength - 1; i >= 0; i--)
                    {
                        res.Append(' ', rowlabelextra);
                        res.Append(col_corner_sep);

                        LabelList labelList = GetColLabels()[i];
                        for (int j = 0; j < labelList.Length; j++)
                        {
                            Label lbl = labelList.Labels[j];

                            int spanstart = 0;
                            for (int k2 = 0; k2 < j; k2++)
                            {
                                spanstart += labelList.Labels[k2].Span;
                            }

                            int space_count = int.Parse(ArrayMethods.ArraySum(new List<int>(col_widths), spanstart, spanstart + lbl.Span).ToString())
                                              - lbl.Value.Length + (lbl.Span - 1);

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
                SetElementsWithRowLabels(res, rowlbl_widths, col_widths);
            }
            return res.ToString();
        }
        #endregion

        #region Dataframe Features
        /// <summary>
        /// Check if given column at index <paramref name="c"/> is all numbers
        /// </summary>
        /// <param name="c">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>True if all values in the column <paramref name="c"/> is parsable as floats</returns>
        public bool IsAllNumberColumn(int c, int based = 1)
        {
            foreach (object val in ColList(c, based))
            {
                if (float.TryParse(val.ToString(), out _))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if all values in the dataframe is parsable as floats
        /// </summary>
        /// <returns>True if all values are parsable, otherwise false</returns>
        public bool IsAllNumbers()
        {
            for (int j = 0; j < Col; j++)
            {
                foreach (object val in ColList(j, 0))
                {
                    if (float.TryParse(val.ToString(), out _))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Operator Overloads

        #region Equals
        public static bool operator ==(Dataframe df, Dataframe df2)
        {
            if (df.Row != df2.Row || df.Col != df2.Col)
            {
                return false;
            }

            bool isEqual = true;
            List<List<object>> vals1 = df.GetValues();
            List<List<object>> vals2 = df2.GetValues();
            for (int i = 0; i < df.Row; i++)
            {
                if (!isEqual)
                {
                    break;
                }

                for (int j = 0; j < df.Col; j++)
                {
                    if (vals1[i][j].ToString() != vals2[i][j].ToString())
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }

        public static bool operator ==(Dataframe df, MatrisBase<dynamic> mat2)
        {
            if (df.Row != mat2.Row || df.Col != mat2.Col)
            {
                return false;
            }

            bool isEqual = true;
            List<List<object>> vals1 = df.GetValues();
            List<List<dynamic>> vals2 = mat2.GetValues();
            for (int i = 0; i < df.Row; i++)
            {
                if (!isEqual)
                {
                    break;
                }

                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(vals1[i][j].ToString(), out float res))
                    {
                        if (res != float.Parse(vals2[i][j].ToString()))
                        {
                            isEqual = false;
                            break;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return isEqual;
        }
        public static bool operator ==(MatrisBase<dynamic> mat, Dataframe df)
        {
            return df == mat;
        }

        public static bool operator ==(dynamic other, Dataframe df)
        {
            if (other == null)
            {
                return false;
            }

            if (df.IsScalar())
            {
                if (other is int @int)
                {
                    return int.Parse(df[0, 0].ToString(), CultureInfo.CurrentCulture) == @int;
                }
                else
                {
                    return other is float @float
                        ? float.Parse(df[0, 0].ToString(), CultureInfo.CurrentCulture) == @float
                        : other is double @double
                                            ? double.Parse(df[0, 0].ToString(), CultureInfo.CurrentCulture) == @double
                                            : (bool)(other.ToString() == df[0, 0].ToString());
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(Dataframe df, dynamic other)
        {
            return other == df;
        }

        #endregion

        #region Not Equals

        public static bool operator !=(Dataframe df, MatrisBase<dynamic> mat)
        {
            return !(mat == df);
        }
        public static bool operator !=(MatrisBase<dynamic> mat, Dataframe df)
        {
            return df != mat;
        }

        public static bool operator !=(Dataframe df, Dataframe df2)
        {
            return !(df == df2);
        }

        public static bool operator !=(dynamic other, Dataframe df)
        {
            return !(other == df);
        }
        public static bool operator !=(Dataframe df, dynamic other)
        {
            return !(df == other);
        }

        #endregion

        #region Addition
        // Unary
        public static Dataframe operator +(Dataframe df)
        {
            return df;
        }

        public static Dataframe operator +(Dataframe df, Dataframe df2)
        {
            if (df.Row != df2.Row || df.Col != df2.Col)
            {
                throw new Exception(CompilerMessage.ADDITION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float r1)
                        && float.TryParse(df2.GetValues()[i][j].ToString(), out float r2))
                    {
                        newlis[i].Add(r1 + r2);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator +(Dataframe df, MatrisBase<dynamic> mat2)
        {
            if (mat2.IsScalar())
            {
                return df + (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (df.Row != mat2.Row || df.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.ADDITION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(res + float.Parse(mat2.GetValues()[i][j].ToString()));
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }
        public static Dataframe operator +(MatrisBase<dynamic> mat, Dataframe df)
        {
            return df + mat;
        }

        public static Dataframe operator +(Dataframe df, dynamic val)
        {
            if (val is null)
            {
                val = 0;
            }

            if (float.TryParse(((object)val).ToString(), out float res))
            {
                List<List<object>> vals = df.GetValues();
                List<List<object>> newlis = new List<List<object>>();
                for (int i = 0; i < df.Row; i++)
                {
                    newlis.Add(new List<object>());
                    for (int j = 0; j < df.Col; j++)
                    {
                        if (float.TryParse(vals[i][j].ToString(), out float res2))
                        {
                            newlis[i].Add(res + res2);
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                        }
                    }
                }
                Dataframe result = df.Copy();
                result.SetValues(newlis);
                return result;
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }

        }
        public static Dataframe operator +(dynamic val, Dataframe df)
        {
            return df + val;
        }

        #endregion

        #region Subtraction
        // Unary
        public static Dataframe operator -(Dataframe df)
        {
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float r1))
                    {
                        newlis[i].Add(r1 * -1);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator -(Dataframe df, Dataframe df2)
        {
            if (df.Row != df2.Row || df.Col != df2.Col)
            {
                throw new Exception(CompilerMessage.SUBTRACTION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float r1)
                        && float.TryParse(df2.GetValues()[i][j].ToString(), out float r2))
                    {
                        newlis[i].Add(r1 - r2);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.SUBTRACTION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator -(Dataframe df, MatrisBase<dynamic> mat2)
        {
            if (mat2.IsScalar())
            {
                return df - (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (df.Row != mat2.Row || df.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.SUBTRACTION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(res - float.Parse(mat2.GetValues()[i][j].ToString()));
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.SUBTRACTION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }
        public static Dataframe operator -(MatrisBase<dynamic> mat, Dataframe df)
        {
            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) - df;
            }

            if (df.Row != mat.Row || df.Col != mat.Col)
            {
                throw new Exception(CompilerMessage.SUBTRACTION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(float.Parse(mat.GetValues()[i][j].ToString()) - res);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.SUBTRACTION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator -(Dataframe df, dynamic val)
        {
            if (val is null)
            {
                val = 0;
            }

            if (float.TryParse(((object)val).ToString(), out float res))
            {
                List<List<object>> vals = df.GetValues();
                List<List<object>> newlis = new List<List<object>>();
                for (int i = 0; i < df.Row; i++)
                {
                    newlis.Add(new List<object>());
                    for (int j = 0; j < df.Col; j++)
                    {
                        if (float.TryParse(vals[i][j].ToString(), out float res2))
                        {
                            newlis[i].Add(res2 - res);
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                        }
                    }
                }
                Dataframe result = df.Copy();
                result.SetValues(newlis);
                return result;
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }

        }
        public static Dataframe operator -(dynamic val, Dataframe df)
        {
            return val + (-df);
        }

        #endregion

        #region Multiplication
        public static Dataframe operator *(Dataframe df, Dataframe df2)
        {
            if (df.Row != df2.Row || df.Col != df2.Col)
            {
                throw new Exception(CompilerMessage.ADDITION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float r1)
                        && float.TryParse(df2.GetValues()[i][j].ToString(), out float r2))
                    {
                        newlis[i].Add(r1 * r2);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator *(Dataframe df, MatrisBase<dynamic> mat2)
        {
            if (mat2.IsScalar())
            {
                return df * (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (df.Row != mat2.Row || df.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.ADDITION_SIZE_INVALID);
            }

            List<List<object>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(df.GetValues()[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(res * float.Parse(mat2.GetValues()[i][j].ToString()));
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }
        public static Dataframe operator *(MatrisBase<dynamic> mat, Dataframe df)
        {
            return df * mat;
        }

        public static Dataframe operator *(Dataframe df, dynamic val)
        {
            if (val is null)
            {
                val = 1;
            }

            if (float.TryParse(((object)val).ToString(), out float res))
            {
                List<List<object>> vals = df.GetValues();
                List<List<object>> newlis = new List<List<object>>();
                for (int i = 0; i < df.Row; i++)
                {
                    newlis.Add(new List<object>());
                    for (int j = 0; j < df.Col; j++)
                    {
                        if (float.TryParse(vals[i][j].ToString(), out float res2))
                        {
                            newlis[i].Add(res2 * res);
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ADDITION_PARSE_FAILED);
                        }
                    }
                }
                Dataframe result = df.Copy();
                result.SetValues(newlis);
                return result;
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }

        }
        public static Dataframe operator *(dynamic val, Dataframe df)
        {
            return df * val;
        }
        #endregion

        #region Division

        public static Dataframe operator /(Dataframe df, Dataframe df2)
        {
            if (df.Row != df2.Row || df.Col != df2.Col)
            {
                throw new Exception(CompilerMessage.DIVISION_SIZE_INVALID);
            }

            List<List<object>> vals1 = df.GetValues();
            List<List<object>> vals2 = df2.GetValues();
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(vals1[i][j].ToString(), out float r1)
                           && float.TryParse(vals2[i][j].ToString(), out float r2))
                    {
                        newlis[i].Add(r1 / r2);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.DIVISION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator /(Dataframe df, MatrisBase<dynamic> mat2)
        {
            if (mat2.IsScalar())
            {
                return df / (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (df.Row != mat2.Row || df.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.DIVISION_SIZE_INVALID);
            }

            List<List<object>> vals1 = df.GetValues();
            List<List<object>> vals2 = mat2.GetValues();
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(vals1[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(res / float.Parse(vals2[i][j].ToString()));
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.DIVISION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }
        public static Dataframe operator /(MatrisBase<dynamic> mat, Dataframe df)
        {

            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) / df;
            }

            if (mat.Row != df.Row || mat.Col != df.Col)
            {
                throw new Exception(CompilerMessage.DIVISION_SIZE_INVALID);
            }

            List<List<object>> vals1 = mat.GetValues();
            List<List<object>> vals2 = df.GetValues();
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < mat.Col; j++)
                {
                    if (float.TryParse(vals2[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(float.Parse(vals1[i][j].ToString()) / res);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.DIVISION_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator /(Dataframe df, dynamic val)
        {
            if (val is null)
            {
                return df.Copy();
            }
            else
            {
                if (float.TryParse(((object)val).ToString(), out float res))
                {
                    List<List<object>> vals = df.GetValues();
                    List<List<object>> newlis = new List<List<object>>();
                    for (int i = 0; i < df.Row; i++)
                    {
                        newlis.Add(new List<object>());
                        for (int j = 0; j < df.Col; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float dval))
                            {
                                newlis[i].Add(dval / res);
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.DIVISION_PARSE_FAILED);
                            }
                        }
                    }
                    Dataframe result = df.Copy();
                    result.SetValues(newlis);
                    return result;
                }
                else
                {
                    throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
                }
            }
        }
        public static Dataframe operator /(dynamic val, Dataframe df)
        {
            if (val is null)
            {
                return df.Copy();
            }
            else
            {
                if (float.TryParse(((object)val).ToString(), out float res))
                {
                    List<List<object>> vals = df.GetValues();
                    List<List<object>> newlis = new List<List<object>>();
                    for (int i = 0; i < df.Row; i++)
                    {
                        newlis.Add(new List<object>());
                        for (int j = 0; j < df.Col; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float dval))
                            {
                                newlis[i].Add(res / dval);
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.DIVISION_PARSE_FAILED);
                            }
                        }
                    }
                    Dataframe result = df.Copy();
                    result.SetValues(newlis);
                    return result;
                }
                else
                {
                    throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
                }

            }

        }

        #endregion

        #region Modulo
        public static Dataframe operator %(Dataframe df, Dataframe df2)
        {
            if (df.Row != df2.Row || df.Col != df2.Col)
            {
                throw new Exception(CompilerMessage.MODULO_SIZE_INVALID);
            }

            List<List<object>> vals1 = df.GetValues();
            List<List<object>> vals2 = df2.GetValues();
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(vals1[i][j].ToString(), out float r1)
                           && float.TryParse(vals2[i][j].ToString(), out float r2))
                    {
                        newlis[i].Add(r1 % r2);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.MODULO_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator %(Dataframe df, MatrisBase<dynamic> mat2)
        {
            if (mat2.IsScalar())
            {
                return df % (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (df.Row != mat2.Row || df.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.MODULO_SIZE_INVALID);
            }

            List<List<object>> vals1 = df.GetValues();
            List<List<object>> vals2 = mat2.GetValues();
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < df.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < df.Col; j++)
                {
                    if (float.TryParse(vals1[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(res % float.Parse(vals2[i][j].ToString()));
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.MODULO_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }
        public static Dataframe operator %(MatrisBase<dynamic> mat, Dataframe df)
        {

            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) % df;
            }

            if (mat.Row != df.Row || mat.Col != df.Col)
            {
                throw new Exception(CompilerMessage.MODULO_SIZE_INVALID);
            }

            List<List<object>> vals1 = mat.GetValues();
            List<List<object>> vals2 = df.GetValues();
            List<List<object>> newlis = new List<List<object>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<object>());
                for (int j = 0; j < mat.Col; j++)
                {
                    if (float.TryParse(vals2[i][j].ToString(), out float res))
                    {
                        newlis[i].Add(float.Parse(vals1[i][j].ToString()) % res);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.MODULO_PARSE_FAILED);
                    }
                }
            }
            Dataframe result = df.Copy();
            result.SetValues(newlis);
            return result;
        }

        public static Dataframe operator %(Dataframe df, dynamic val)
        {
            if (val is null)
            {
                return df.Copy();
            }
            else
            {
                if (float.TryParse(((object)val).ToString(), out float res))
                {
                    List<List<object>> vals = df.GetValues();
                    List<List<object>> newlis = new List<List<object>>();
                    for (int i = 0; i < df.Row; i++)
                    {
                        newlis.Add(new List<object>());
                        for (int j = 0; j < df.Col; j++)
                        {
                            if (float.TryParse(vals[i][j].ToString(), out float dval))
                            {
                                newlis[i].Add(dval % res);
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.MODULO_PARSE_FAILED);
                            }
                        }
                    }
                    Dataframe result = df.Copy();
                    result.SetValues(newlis);
                    return result;
                }
                else
                {
                    throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
                }
            }
        }
        public static Dataframe operator %(dynamic val, Dataframe df)
        {
            if (df.IsScalar())
            {
                if (float.TryParse(((object)val).ToString(), out float res))
                {
                    if (float.TryParse(df.GetValues()[0][0].ToString(), out float dval))
                    {
                        Dataframe result = df.Copy();
                        result.SetValues(new List<List<object>>() { new List<object>() { res % dval } });
                        return result;
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.MODULO_PARSE_FAILED);
                    }
                }
                else
                {
                    throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);

                }
            }
            else
            {
                throw new Exception(CompilerMessage.MODULO_MAT_INVALID);
            }
        }

        #endregion

        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "df(" + Row + "," + Col + ")\n" + ToString();
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
                        foreach (List<object> r in _values)
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
                    if (GetRowSettings() != null)
                    {
                        GetRowSettings().Dispose();
                        SetRowSettings(null);
                    }

                    if (GetColSettings() != null)
                    {
                        GetColSettings().Dispose();
                        SetColSettings(null);
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

        #region Hash Override

        public override bool Equals(object obj)
        {
            return obj is Dataframe dataframe &&
                   base.Equals(obj) &&
                   Row == dataframe.Row &&
                   Col == dataframe.Col &&
                   Seed == dataframe.Seed &&
                   Delimiter == dataframe.Delimiter &&
                   NewLine == dataframe.NewLine &&
                   Digits == dataframe.Digits &&
                   SwapCount == dataframe.SwapCount &&
                   CreatedFromSeed == dataframe.CreatedFromSeed &&
                   ElementCount == dataframe.ElementCount &&
                   _row == dataframe._row &&
                   _col == dataframe._col &&
                   EqualityComparer<List<List<object>>>.Default.Equals(_values, dataframe._values) &&
                   EqualityComparer<List<LabelList>>.Default.Equals(_rowlabels, dataframe._rowlabels) &&
                   EqualityComparer<List<LabelList>>.Default.Equals(_collabels, dataframe._collabels) &&
                   EqualityComparer<DataframeRowSettings>.Default.Equals(rowSettings, dataframe.rowSettings) &&
                   EqualityComparer<DataframeColSettings>.Default.Equals(colSettings, dataframe.colSettings) &&
                   disposedValue == dataframe.disposedValue &&
                   Row == dataframe.Row &&
                   Col == dataframe.Col;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(Row);
            hash.Add(Col);
            hash.Add(Seed);
            hash.Add(Delimiter);
            hash.Add(NewLine);
            hash.Add(Digits);
            hash.Add(SwapCount);
            hash.Add(CreatedFromSeed);
            hash.Add(ElementCount);
            hash.Add(_row);
            hash.Add(_col);
            hash.Add(_values);
            hash.Add(_rowlabels);
            hash.Add(_collabels);
            hash.Add(rowSettings);
            hash.Add(colSettings);
            hash.Add(disposedValue);
            hash.Add(Row);
            hash.Add(Col);
            return hash.ToHashCode();
        }
        #endregion
    }
}
