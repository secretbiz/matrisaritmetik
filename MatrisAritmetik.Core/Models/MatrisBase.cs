using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Base class for matrices
    /// </summary>
    /// <typeparam name="T">Type of the values matrix will store, use "object" or "dynamic" if unknown</typeparam>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class MatrisBase<T> : IDisposable
    {
        #region Private Encapsulated Fields
        private int _seed;
        private int _row = -1;
        private int _col = -1;
        private List<List<T>> _values;
        private string _delimiter = " ";
        private string _newline = "\n";
        private int _digits = 6;
        private int swapCount;
        private bool createdFromSeed;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the row dimension, limit at <see cref="MatrisLimits.forRows"/>
        /// </summary>
        public virtual int Row
        {
            get => _row;
            set
            {
                if (_row == -1) // Only set if first time
                {
                    _row = value > (int)MatrisLimits.forRows ? (int)MatrisLimits.forRows : value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the column dimension, limit at <see cref="MatrisLimits.forCols"/>
        /// </summary>
        public virtual int Col
        {
            get => _col;
            set
            {
                if (_col == -1) // Only set if first time
                {
                    _col = value > (int)MatrisLimits.forCols ? (int)MatrisLimits.forCols : value;
                }
            }
        }

        /// <summary>
        /// Alternative way to access values directly
        /// </summary>
        public virtual List<List<T>> Values => _values;

        /// <summary>
        /// Manually sets the column dimension, only used during echelon form process
        /// </summary>
        /// <param name="c">New column dimension</param>
        public void SetCol(int c)
        { Col = c; }

        /// <summary>
        /// List of lists to hold matrix values, limits and removes if list count's are higher than <see cref="MatrisLimits"/>
        /// </summary>
        public virtual List<List<T>> GetValues()
        {
            if (_values != null)
            {
                if (_values.Count != 0 && _values.Count * _values[0].Count > (int)MatrisLimits.forSize)
                {
                    if (_values.Count > (int)MatrisLimits.forRows)
                    {
                        int delindex = (int)MatrisLimits.forRows;
                        for (int i = 0; i < _values.Count - delindex; i++)
                        {
                            _values.RemoveAt(delindex);
                        }
                    }
                    if (_values.Count >= 1)
                    {
                        if (_values[0].Count > (int)MatrisLimits.forCols)
                        {
                            int delindex = (int)MatrisLimits.forCols;
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
        /// List of lists to hold matrix values, limits and removes if list count's are higher than <see cref="MatrisLimits"/>
        /// </summary>
        public virtual void SetValues(List<List<T>> value)
        {
            if (value != null)
            {
                if (value.Count != 0 && value.Count * value[0].Count > (int)MatrisLimits.forSize)
                {
                    List<List<T>> temp = new List<List<T>>();
                    int collimit = (int)MatrisLimits.forCols;

                    for (int i = 0; i < Math.Min(value.Count, (int)MatrisLimits.forRows); i++)
                    {
                        temp.Add(new List<T>());
                        for (int j = 0; j < Math.Min(value[i].Count, collimit); j++)
                        {
                            if (float.TryParse(value[i][j].ToString(), out float res))
                            {
                                temp[i].Add((dynamic)res);
                            }
                            else
                            {
                                temp[i].Add((dynamic)new None());
                            }
                        }
                    }
                    _values = temp;
                }
                else
                {
                    _values = new List<List<T>>() { };
                    int lastcolsize = value.Count == 0 ? 0 : value[0].Count;
                    if (typeof(T) == typeof(string))
                    {
                        for (int i = 0; i < value.Count; i++)
                        {
                            if (value[i].Count != lastcolsize)
                            {
                                _values = new List<List<T>>() { };
                                return;
                            }
                            else
                            {
                                _values.Add(new List<T>());
                                for (int j = 0; j < value[i].Count; j++)
                                {
                                    _values[i].Add((dynamic)value[i][j].ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < value.Count; i++)
                        {
                            if (value[i].Count != lastcolsize)
                            {
                                _values = new List<List<T>>() { };
                                return;
                            }
                            else
                            {
                                _values.Add(new List<T>());
                                for (int j = 0; j < value[i].Count; j++)
                                {
                                    if (float.TryParse(value[i][j].ToString(), out float res))
                                    {
                                        _values[i].Add((dynamic)res);
                                    }
                                    else
                                    {
                                        _values[i].Add((dynamic)new None());
                                    }
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
                    foreach (List<T> l in _values)
                    {
                        l.Clear();
                    }
                    _values.Clear();
                    _values = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the seed used with <see cref="Random"/> while filling <see cref="MatrisBase{T}.GetValues()"/>
        /// </summary>
        public int Seed
        {
            get => _seed;
            set => _seed = value >= 0 ? value : int.MaxValue + value;
        }
        /// <summary>
        /// Delimiter to use while printing the matrix values
        /// </summary>
        public string Delimiter
        {
            get => _delimiter;
            set => _delimiter = value != null && value.Length > 0 ? value : " ";
        }
        /// <summary>
        /// New-line character to use while printing the matrix
        /// </summary>
        public string NewLine
        {
            get => _newline;
            set => _newline = value != null && value.Length > 0 ? value : "\n";
        }
        /// <summary>
        /// Amount of digits to round to while printing
        /// </summary>
        public int Digits
        {
            get => _digits;
            set => _digits = value >= 0 ? value : 0;
        }
        /// <summary>
        /// Disposal information
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// For storing how many times row's were swapped during echelon form process
        /// </summary>
        public int SwapCount { get => swapCount; set => swapCount = value; }

        /// <summary>
        /// Wheter the matrix was filled with random values
        /// </summary>
        public bool CreatedFromSeed { get => createdFromSeed; set => createdFromSeed = value; }

        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public MatrisBase() { }

        /// <summary>
        /// Creates a matrix with given dimensions, filled with given value 
        /// </summary>
        /// <param name="rowDim">Row dimension</param>
        /// <param name="colDim">Column dimension</param>
        /// <param name="fill">Value to fill the matrix with</param>
        public MatrisBase(int rowDim, int colDim, T fill)
        {
            if (rowDim > (int)MatrisLimits.forRows)
            {
                rowDim = (int)MatrisLimits.forRows;
            }

            if (colDim > (int)MatrisLimits.forCols)
            {
                colDim = (int)MatrisLimits.forCols;
            }

            Row = rowDim; Col = colDim;

            List<List<T>> vals = new List<List<T>>();
            for (int i = 0; i < rowDim; i++)
            {
                vals.Add(new List<T>());
                for (int j = 0; j < colDim; j++)
                {
                    vals[i].Add(fill);
                }
            }
            _values = vals;
        }

        /// <summary>
        /// Creates a matrix from the given 2D List
        /// </summary>
        /// <param name="vals">A list with same length lists</param>
        /// <param name="delim">Delimiter to seperate values</param>
        /// <param name="newline">Newline character to create a new row</param>
        public MatrisBase(List<List<T>> vals, string delim = " ", string newline = "\n")
        {
            if (vals == null)
            {
                Row = 0;
                Col = 0;
                return;
            }
            Row = vals.Count;
            if (Row == 0)
            {
                Col = 0;
                return;
            }

            SetValues(vals);

            Delimiter = delim;
            NewLine = newline;
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Finds the longest values in columns and returns their widths in an array
        /// </summary>
        /// <param name="mat">Matrix to get column widths of</param>
        /// <returns>Array of column widths</returns>
        internal static int[] GetColumnWidths(MatrisBase<T> mat)
        {
            int[] longest_in_col = new int[mat.Col];
            int currentmax;
            int row = mat.Row;
            int col = mat.Col;

            List<List<T>> m = mat.GetValues();
            for (int j = 0; j < col; j++)
            {
                currentmax = 0;
                for (int i = 0; i < row; i++)
                {
                    if (m[i][j].ToString().Length > currentmax)
                    {
                        currentmax = m[i][j].ToString().Length;
                    }
                }
                longest_in_col[j] = currentmax;
            }

            return longest_in_col;

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a deep copy of the matrix
        /// </summary>
        /// <returns>A new matrix with the same values</returns>
        public MatrisBase<T> Copy()
        {
            int r = Row;
            int c = Col;
            List<List<T>> v = GetValues();
            List<List<T>> lis = new List<List<T>>();
            for (int i = 0; i < r; i++)
            {
                lis.Add(new List<T>());
                for (int j = 0; j < c; j++)
                {
                    lis[i].Add((dynamic)float.Parse(v[i][j].ToString()));
                }
            }
            return new MatrisBase<T>(lis);
        }

        /// <summary>
        /// Creates a printable string using <see cref="MatrisBase{T}.Delimiter"/> and <see cref="MatrisBase{T}.NewLine"/>
        /// </summary>
        /// <returns>A string with values in columns left-aligned</returns>
        public string Printstr()
        {
            if (!IsValid())
            {
                return "";
            }

            bool stringmat = typeof(T) == typeof(string);

            StringBuilder res = new StringBuilder();
            // Column sizes
            int[] longest_in_col = GetColumnWidths(this);

            int colno;
            foreach (List<T> row in GetValues())
            {
                colno = 0;
                foreach (T element in row)
                {
                    if (stringmat)
                    {
                        res.Append(element.ToString());
                    }
                    else
                    {
                        res.Append(' ', longest_in_col[colno] - element.ToString().Length);
                        res.Append(element.ToString());
                    }

                    if (colno != Col - 1)
                    {
                        res.Append(Delimiter);
                    }

                    colno++;
                }
                res.Append(NewLine);
            }

            return res.ToString();
        }

        #endregion

        #region Indexers, 0-based
        /// <summary>
        /// Get or set the rows in the given range
        /// </summary>
        /// <param name="r">Range for rows</param>
        /// <returns>A shallow copy of the values in range</returns>
        public List<List<T>> this[Range r]
        {
            get
            {
                if (r.Start.Equals(r.End) || r.End.Value > Row || r.Start.Value >= Row)
                {
                    return null;
                }

                return GetValues().GetRange(r.Start.Value, r.End.Value - r.Start.Value);
            }
            set
            {
                if (r.Start.Equals(r.End) || r.End.Value > Row || r.Start.Value >= Row)
                {
                    return;
                }

                List<List<T>> newVals = new List<List<T>>();
                List<T>[] temp = new List<T>[Row];

                if (r.Start.Value > 0)
                {
                    newVals.AddRange(GetValues().GetRange(0, r.Start.Value));
                }

                newVals.AddRange(value);

                if (Row - r.End.Value > 0)
                {
                    newVals.AddRange(GetValues().GetRange(r.End.Value, Row - r.End.Value));
                }

                newVals.CopyTo(temp);
                newVals.Clear();

                GetValues().Clear();
                SetValues(new List<List<T>>(temp));
            }
        }

        /// <summary>
        /// Get or set a row of the matrix, 0-based index
        /// </summary>
        /// <param name="index">Index of the row</param>
        /// <returns>Stored instance of the values in the given row</returns>
        public List<T> this[int index]
        {

            get
            {
                if (index >= Row)
                {
                    return null;
                }

                return GetValues()[index];
            }
            set
            {
                if (index >= Row)
                {
                    return;
                }

                GetValues()[index] = value;
            }
        }

        /// <summary>
        /// Get or set a value in the matrix, 0-based indices
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="c">Column index</param>
        /// <returns>The instance stored in the given index</returns>
        public T this[int r, int c]
        {
            get
            {
                if (r >= Row)
                {
                    return default;
                }

                if (c >= Col)
                {
                    return default;
                }

                return GetValues()[r][c];
            }
            set
            {
                if (r >= Row)
                {
                    return;
                }

                if (c >= Col)
                {
                    return;
                }

                GetValues()[r][c] = value;
            }
        }

        /// <summary>
        /// Get or set the given range columns in the given range of rows
        /// </summary>
        /// <param name="r">Row range</param>
        /// <param name="c">Column range</param>
        /// <returns>A deep copy of the values in range</returns>
        public List<List<T>> this[Range r, Range c]
        {
            get
            {
                if (r.Start.Equals(r.End) || r.End.Value > Row || r.Start.Value >= Row)
                {
                    return null;
                }

                if (c.Start.Equals(c.End) || c.End.Value > Col || c.Start.Value >= Col)
                {
                    return null;
                }

                List<List<T>> newVals = new List<List<T>>();
                for (int i = r.Start.Value; i < r.End.Value; i++)
                {
                    List<T> temp = new List<T>();
                    for (int j = c.Start.Value; j < c.End.Value; j++)
                    {
                        temp.Add(GetValues()[i][j]);
                    }

                    newVals.Add(temp);
                }
                return newVals;
            }
            set
            {

                if (r.Start.Equals(r.End) || r.End.Value > Row || r.Start.Value >= Row)
                {
                    return;
                }

                if (c.Start.Equals(c.End) || c.End.Value > Col || c.Start.Value >= Col)
                {
                    return;
                }

                for (int i = r.Start.Value; i < r.End.Value; i++)
                {
                    List<T> newVals = new List<T>();
                    T[] temp = new T[Col];

                    if (c.Start.Value > 0)
                    {
                        newVals.AddRange(GetValues()[i].GetRange(0, c.Start.Value));
                    }

                    newVals.AddRange(value[i]);

                    if (Col - c.End.Value > 0)
                    {
                        newVals.AddRange(GetValues()[i].GetRange(c.End.Value, Col - c.End.Value));
                    }

                    newVals.CopyTo(temp);
                    newVals.Clear();

                    GetValues()[i].Clear();
                    GetValues()[i] = new List<T>(temp);
                }

            }
        }
        #endregion

        #region Public Indexing Methods
        /// <summary>
        /// Get a deep copy of the row with given index <paramref name="r"/> with base <paramref name="based"/>
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="based">Index base</param>
        /// <returns>Deep copy of values in a row</returns>
        public List<T> RowList(int r,
                               int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int j = 0; j < Col; j++)
            {
                listrow.Add((dynamic)GetValues()[r - based][j]);
            }
            return listrow;
        }

        /// <summary>
        /// Get a new row-Matrix from given row index <paramref name="r"/> with base <paramref name="based"/>
        /// </summary>
        /// <param name="r">Row index</param>
        /// <param name="based">Index base</param>
        /// <returns>New row-Matrix</returns>
        public MatrisBase<T> RowMat(int r,
                                    int based = 1)
        {
            List<List<T>> listrow = new List<List<T>>() { new List<T>() };
            for (int j = 0; j < Col; j++)
            {
                listrow[0].Add((dynamic)GetValues()[r - based][j]);
            }

            MatrisBase<T> res = new MatrisBase<T>() { Row = 1, Col = Col };
            res.SetValues(listrow);
            return res;
        }

        /// <summary>
        /// Get a deep copy of the column with given index <paramref name="c"/> with base <paramref name="based"/>
        /// </summary>
        /// <param name="c">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>Deep copy of values in a column as 1D list</returns>
        public List<T> ColList(int c,
                               int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int i = 0; i < Row; i++)
            {
                listrow.Add((dynamic)GetValues()[i][c - based]);
            }
            return listrow;
        }

        /// <summary>
        /// Get a new column-Matrix from given column index <paramref name="c"/> with base <paramref name="based"/>
        /// </summary>
        /// <param name="c">Column index</param>
        /// <param name="based">Index base</param>
        /// <returns>New column-Matrix</returns>
        public MatrisBase<T> ColMat(int c,
                                    int based = 1)
        {
            List<List<T>> listcol = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                listcol.Add(new List<T>() { (dynamic)GetValues()[i][c - based] });
            }

            return new MatrisBase<T>(listcol);
        }
        #endregion

        #region Other Public Methods
        /// <summary>
        /// Returns the size of the matrix
        /// </summary>
        public int ElementCount => Col * Row;

        /// <summary>
        /// Creates a matrix detail summary
        /// </summary>
        /// <param name="name">Name given for this matrix</param>
        /// <returns>A string with matrix name, seed(if exists), dimensions and values</returns>
        public virtual string Details(string name = "")
        {
            string seed_str = CreatedFromSeed ? $"Seed: {Seed}\n" : string.Empty;
            return $"Matris: {name}\n"
                   + seed_str
                   + $"Boyut: {Row}x{Col}\n"
                   + "Elementler:\n"
                   + ToString();
        }

        /// <summary>
        /// Creates a smaller printable matrix for larger matrices, using only values in the corners
        /// </summary>
        /// <param name="rowEachCorner">Amount of rows for each corner</param>
        /// <param name="colEachCorner">Amount of columns for each corner</param>
        /// <param name="filler">Value to be used between corners</param>
        /// <returns>A smaller matrix with <paramref name="filler"/> dividing it to 4 smaller matrices</returns>
        public virtual dynamic CornerMatrix(int rowEachCorner = -1,
                                            int colEachCorner = -1,
                                            string filler = "...")
        {
            if (Row == 0 || Col == 0)
            {
                return new MatrisBase<T>();
            }

            List<List<string>> smallerList = new List<List<string>>();

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
                MatrisBase<T> res = new MatrisBase<T>() { Row = Row, Col = Col };
                res.SetValues(GetValues());
                return res;
            }
            // Only reduce columns
            else if (((float)rowEachCorner * 2.0) + 1.0 > (float)Row)
            {
                // Start reducing columns
                for (int i = 0; i < Row; i++)
                {
                    smallerList.Add(new List<string>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[i].Add(GetValues()[i][left].ToString());
                    }

                    smallerList[i].Add(filler);

                    for (int right = Col - colEachCorner; right < Col; right++)
                    {
                        smallerList[i].Add(GetValues()[i][right].ToString());
                    }
                }

                return new MatrisBase<string>() { Row = (rowEachCorner * 2) + 1, Col = (colEachCorner * 2) + 1, _values = smallerList };
            }
            // Only reduce rows
            else if (((float)colEachCorner * 2.0) + 1.0 > (float)Col)
            {
                // Start reducing rows
                // Upper half
                for (int u = 0; u < rowEachCorner; u++)
                {
                    smallerList.Add(new List<string>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[u].Add(GetValues()[u][left].ToString());
                    }
                }

                smallerList.Add(new List<string>());
                for (int j = 0; j < Col; j++)
                {
                    smallerList[rowEachCorner].Add(filler);
                }

                int rrowind = rowEachCorner + 1;
                // Bottom half
                for (int i = Row - rowEachCorner; i < Row; i++)
                {
                    smallerList.Add(new List<string>());
                    for (int bleft = 0; bleft < colEachCorner; bleft++)
                    {
                        smallerList[rrowind].Add(GetValues()[i][bleft].ToString());
                    }
                    rrowind++;
                }

                MatrisBase<string> res = new MatrisBase<string>() { Row = (rowEachCorner * 2) + 1, Col = (colEachCorner * 2) + 1 };
                res.SetValues(smallerList);
                return res;
            }

            // Reduce both rows and columns
            // Upper half
            for (int u = 0; u < rowEachCorner; u++)
            {
                smallerList.Add(new List<string>());
                for (int left = 0; left < colEachCorner; left++)
                {
                    smallerList[u].Add(GetValues()[u][left].ToString());
                }

                smallerList[u].Add(filler);

                for (int right = Col - colEachCorner; right < Col; right++)
                {
                    smallerList[u].Add(GetValues()[u][right].ToString());
                }
            }

            smallerList.Add(new List<string>());
            for (int j = 0; j < (colEachCorner * 2) + 1; j++)
            {
                smallerList[rowEachCorner].Add(filler);
            }

            int rowind = rowEachCorner + 1;
            // Bottom half
            for (int i = Row - rowEachCorner; i < Row; i++)
            {
                smallerList.Add(new List<string>());
                for (int bleft = 0; bleft < colEachCorner; bleft++)
                {
                    smallerList[rowind].Add(GetValues()[i][bleft].ToString());
                }

                smallerList[rowind].Add(filler);

                for (int bright = Col - colEachCorner; bright < Col; bright++)
                {
                    smallerList[rowind].Add(GetValues()[i][bright].ToString());
                }
                rowind++;
            }

            return new MatrisBase<string>() { Row = (rowEachCorner * 2) + 1, Col = (colEachCorner * 2) + 1, _values = smallerList };
        }

        /// <summary>
        /// Apply exponentiation operation over each value in the matrix
        /// </summary>
        /// <param name="n">Exponential value</param>
        /// <returns>A new matrix raised to the power of <paramref name="n"/></returns>
        public MatrisBase<T> Power(dynamic n)
        {
            n = double.Parse(n.ToString());
            List<List<T>> newlist = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                newlist.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    newlist[i].Add(Math.Pow(double.Parse(GetValues()[i][j].ToString()), n));
                }
            }
            return new MatrisBase<T>(newlist);
        }

        /// <summary>
        /// Multiply a row with given <paramref name="factor"/>
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="factor">Factor to multiply with</param>
        /// <param name="based">Index base</param>
        public void MulRow(int row,
                           float factor,
                           int based = 1)
        {
            for (int j = 0; j < Col; j++)
            {
                GetValues()[row - based][j] = (dynamic)(float.Parse(GetValues()[row - based][j].ToString()) * factor);
            }
        }
        public void MulThenSubFromOtherRow(int row,
                                           float factor,
                                           int other,
                                           int based = 1)
        {
            for (int j = 0; j < Col; j++)
            {
                GetValues()[other - based][j] = (dynamic)(float.Parse(GetValues()[other - based][j].ToString()) - (float.Parse(GetValues()[row - based][j].ToString()) * factor));
            }
        }
        /// <summary>
        /// Moves a row to the end of the rows
        /// </summary>
        /// <param name="a">Row index</param>
        /// <param name="based">Index base</param>
        public void SwapToEnd(int a,
                              int based = 1)
        {
            a -= based;

            T[] temp = new T[Col];
            GetValues()[a].CopyTo(temp);
            GetValues()[a] = new List<T>(GetValues()[Row - 1]);
            GetValues()[Row - 1] = new List<T>(temp);
        }

        /// <summary>
        /// Swap rows or columns
        /// </summary>
        /// <param name="a">Index 1</param>
        /// <param name="b">Index 2</param>
        /// <param name="axis">Swap rows = 0, swap columns = 1</param>
        /// <param name="based">Index base</param>
        public void Swap(int a,
                         int b,
                         int axis = 0,
                         int based = 1)
        {
            if (a == b)
            {
                return;
            }

            a -= based;
            b -= based;
            // Row swap
            if (axis == 0)
            {
                T[] temp = new T[Col];
                GetValues()[a].CopyTo(temp);
                GetValues()[a] = new List<T>(GetValues()[b]);
                GetValues()[b] = new List<T>(temp);
            }
            // Column swap
            else
            {
                T temp;
                for (int i = 0; i < Row; i++)
                {
                    temp = GetValues()[i][a];
                    GetValues()[i][a] = GetValues()[i][b];
                    GetValues()[i][b] = temp;
                }
            }
        }

        /// <summary>
        /// Change values that are too small, if value smaller than <paramref name="tolerance"/> then value = 0.0
        /// </summary>
        /// <param name="tolerance">Upper limit for a value to be consider zero</param>
        public MatrisBase<T> FixMinusZero(double tolerance = 1e-6)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    if (GetValues()[i][j].ToString() == "-0" || Math.Abs(float.Parse(GetValues()[i][j].ToString())) < tolerance)
                    {
                        GetValues()[i][j] = (dynamic)0;
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Round values in the matrix to given amount of <paramref name="decimals"/>
        /// </summary>
        /// <param name="decimals">Amount of decimal places to round to</param>
        /// <returns>A new matrix with rounded values</returns>
        public MatrisBase<T> Round(int decimals = 5, MidpointRounding midpointRounding = MidpointRounding.ToEven)
        {
            List<List<T>> newvals = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                newvals.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    newvals[i].Add((dynamic)(float)Math.Round(float.Parse(GetValues()[i][j].ToString()), decimals, midpointRounding));
                }
            }
            return new MatrisBase<T>(newvals).FixMinusZero(Math.Pow(10, -decimals));
        }

        #endregion

        #region Matrix Features
        /// <summary>
        /// Check dimensions to validate the matrix
        /// </summary>
        /// <returns>True if matrix is valid (<see cref="MatrisBase{T}.Row"/> > 0 and <see cref="MatrisBase{T}.Col"/> > 0)</returns>
        public bool IsValid()
        {
            return Row > 0 && Col > 0;
        }

        /// <summary>
        /// Check if matrix dimensions are equal to 1, meaning matrix can be consider as a scalar value
        /// </summary>
        /// <returns>True if <see cref="MatrisBase{T}.Row"/> == <see cref="MatrisBase{T}.Col"/> == 1</returns>
        public bool IsScalar()
        {
            return Row == 1 && Col == 1;
        }

        /// <summary>
        /// Check if matrix is square
        /// </summary>
        /// <returns>True if matrix is square (<see cref="MatrisBase{T}.Row"/> == <see cref="MatrisBase{T}.Col"/>)</returns>
        public bool IsSquare()
        {
            return Row == Col;
        }

        /// <summary>
        /// Check if matrix is filled with zeros (absolute values are less than <paramref name="tolerance"/>)
        /// </summary>
        /// <param name="tolerance">Upper limit for a value to be consider zero</param>
        /// <returns>True if all values are in the range of <paramref name="tolerance"/></returns>
        public bool IsZero(float tolerance = (float)0.00001)
        {
            static bool inRange(float num, float tol)
            {
                return num <= tol && num >= -tol;
            }

            bool isZero = true;
            List<List<T>> vals1 = GetValues();
            for (int i = 0; i < Row; i++)
            {
                if (!isZero)
                {
                    break;
                }

                for (int j = 0; j < Col; j++)
                {
                    if (!inRange(float.Parse(vals1[i][j].ToString()), tolerance))
                    {
                        isZero = false;
                        break;
                    }
                }
            }
            return isZero;
        }

        /// <summary>
        /// Check if a column is filled with zeros (absolute values are less than <paramref name="tolerance"/>) 
        /// </summary>
        /// <param name="col_index">Column index</param>
        /// <param name="based">Index base</param>
        /// <param name="tolerance">Upper limit for a value to be consider zero</param>
        /// <returns>True if all values in the column is in the range of <paramref name="tolerance"/></returns>
        public bool IsZeroCol(int col_index,
                              int based = 1,
                              float tolerance = (float)0.00001)
        {
            using MatrisBase<T> matrisBase = ColMat(col_index, based);
            return matrisBase.IsZero(tolerance);
        }

        /// <summary>
        /// Check if a row is filled with zeros (absolute values are less than <paramref name="tolerance"/>) 
        /// </summary>
        /// <param name="row_index">Row index</param>
        /// <param name="based">Index base</param>
        /// <param name="tolerance">Upper limit for a value to be consider zero</param>
        /// <returns>True if all values in the row is in the range of <paramref name="tolerance"/></returns>
        public bool IsZeroRow(int row_index,
                              int based = 1,
                              float tolerance = (float)0.00001)
        {
            using MatrisBase<T> matrisBase = RowMat(row_index, based);
            return matrisBase.IsZero(tolerance);
        }

        /// <summary>
        /// Compare the size of the matrix with another matrix
        /// </summary>
        /// <param name="other">Other matrix to compare sizes to</param>
        /// <returns>True if row and column dimensions match</returns>
        public bool IsSameSize(MatrisBase<T> other)
        {
            if (other == (dynamic)null)
            {
                return false;
            }
            return Row == other.Row && Col == other.Col;
        }
        #endregion

        #region Overriding Object Methods
        /// <summary>
        /// Creates an <see cref="IEnumerable{List{T}}"/> over the values
        /// </summary>
        /// <returns>An <see cref="IEnumerable{List{T}}"/> over the values</returns>
        public IEnumerator<List<T>> GetEnumerator()
        {
            return GetValues().GetEnumerator();
        }


        /// <summary>
        /// Create a printable, aligned string form of the matrix
        /// </summary>
        /// <returns> A string with left-aligned values</returns>
        public override string ToString()
        {
            try
            {
                using MatrisBase<T> matrisBase = Round(Digits);
                return matrisBase.Printstr();
            }
            catch (Exception)
            {
                return Printstr();
            }
        }

        /// <summary>
        /// Overrides <see cref="object.Equals(object)"/> method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if <paramref name="obj"/> is the same as the base</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return "mat(" + Row + "," + Col + ")\n" + ToString();
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_values != null)
                    {
                        for (int i = 0; i < _values.Count(); i++)
                        {
                            _values[i].Clear();
                            _values[i] = null;
                        }
                        _values.Clear();
                        _values = null;
                    }
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MatrisBase()
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

        #region Operator Overloads

        #region Equals
        public static bool operator ==(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
            {
                return false;
            }

            bool isEqual = true;
            List<List<T>> vals1 = mat.GetValues();
            List<List<T>> vals2 = mat2.GetValues();
            for (int i = 0; i < mat.Row; i++)
            {
                if (!isEqual)
                {
                    break;
                }

                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)float.Parse(vals1[i][j].ToString()) != (dynamic)float.Parse(vals2[i][j].ToString()))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }

        public static bool operator ==(MatrisBase<T> mat, List<List<dynamic>> lis)
        {
            if (mat.Row != lis.Count || lis.Count == 0)
            {
                return false;
            }

            if (lis[0].Count != mat.Col)
            {
                return false;
            }

            bool isEqual = true;
            List<List<T>> vals1 = mat.GetValues();
            for (int i = 0; i < mat.Row; i++)
            {
                if (!isEqual)
                {
                    break;
                }

                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)float.Parse(vals1[i][j].ToString()) != (dynamic)float.Parse(lis[i][j].ToString()))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }
        public static bool operator ==(List<List<dynamic>> lis, MatrisBase<T> mat)
        {
            return mat == lis;
        }

        public static bool operator ==(dynamic other, MatrisBase<T> mat)
        {
            if (other == null)
            {
                return false;
            }

            if (mat.IsScalar())
            {
                if (other is int @int)
                {
                    return int.Parse(mat[0, 0].ToString(), CultureInfo.CurrentCulture) == @int;
                }
                else if (other is float @float)
                {
                    return float.Parse(mat[0, 0].ToString(), CultureInfo.CurrentCulture) == @float;
                }
                else if (other is double @double)
                {
                    return double.Parse(mat[0, 0].ToString(), CultureInfo.CurrentCulture) == @double;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(MatrisBase<T> mat, dynamic other)
        {
            return other == mat;
        }

        #endregion

        #region Not Equals

        public static bool operator !=(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            return !(mat == mat2);
        }

        public static bool operator !=(MatrisBase<T> mat, List<List<dynamic>> lis)
        {
            return !(mat == lis);
        }
        public static bool operator !=(List<List<dynamic>> lis, MatrisBase<T> mat)
        {
            return mat != lis;
        }

        public static bool operator !=(dynamic other, MatrisBase<T> mat)
        {
            return !(other == mat);
        }
        public static bool operator !=(MatrisBase<T> mat, dynamic other)
        {
            return !(mat == other);
        }

        #endregion

        #region Addition
        // Unary
        public static MatrisBase<T> operator +(MatrisBase<T> mat)
        {
            return mat;
        }

        public static MatrisBase<dynamic> operator +(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) + mat2;
            }

            if (mat2.IsScalar())
            {
                return mat + (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.ADDITION_SIZE_INVALID);
            }

            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                {
                    newlis[i].Add((dynamic)float.Parse(mat.GetValues()[i][j].ToString()) + (dynamic)float.Parse(mat2.GetValues()[i][j].ToString()));
                }
            }
            return new MatrisBase<dynamic>(newlis);
        }

        public static MatrisBase<dynamic> operator +(MatrisBase<T> mat, dynamic val)
        {
            if (val == null)
            {
                val = 0;
            }

            if (float.TryParse(val.ToString(), out float res))
            {
                List<List<T>> vals = mat.GetValues();
                List<List<dynamic>> newlis = new List<List<dynamic>>();
                for (int i = 0; i < mat.Row; i++)
                {
                    newlis.Add(new List<dynamic>());
                    for (int j = 0; j < mat.Col; j++)
                    {
                        newlis[i].Add(float.Parse(vals[i][j].ToString()) + res);
                    }
                }
                return new MatrisBase<dynamic>(newlis);
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }

        }
        public static MatrisBase<dynamic> operator +(dynamic val, MatrisBase<T> mat)
        {
            return mat + val;
        }

        #endregion

        #region Subtraction
        // Unary
        public static MatrisBase<dynamic> operator -(MatrisBase<T> mat)
        {
            List<List<T>> vals = mat.GetValues();
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                {
                    newlis[i].Add((dynamic)float.Parse(vals[i][j].ToString()) * -1);
                }
            }
            return new MatrisBase<dynamic>(newlis);
        }

        public static MatrisBase<dynamic> operator -(MatrisBase<T> mat, MatrisBase<T> mat2)
        {

            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) - mat2;
            }

            if (mat2.IsScalar())
            {
                return mat - (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.SUBTRACTION_SIZE_INVALID);
            }

            List<List<T>> vals1 = mat.GetValues();
            List<List<T>> vals2 = mat2.GetValues();
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                {
                    newlis[i].Add((dynamic)float.Parse(vals1[i][j].ToString()) - (dynamic)float.Parse(vals2[i][j].ToString()));
                }
            }
            return new MatrisBase<dynamic>(newlis);
        }

        public static MatrisBase<dynamic> operator -(MatrisBase<T> mat, dynamic val)
        {
            if (val == null)
            {
                val = 0;
            }

            if (float.TryParse(val.ToString(), out float res))
            {
                List<List<T>> vals = mat.GetValues();
                List<List<dynamic>> newlis = new List<List<dynamic>>();
                for (int i = 0; i < mat.Row; i++)
                {
                    newlis.Add(new List<dynamic>());
                    for (int j = 0; j < mat.Col; j++)
                    {
                        newlis[i].Add(float.Parse(vals[i][j].ToString()) - res);
                    }
                }
                return new MatrisBase<dynamic>(newlis);
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }

        }
        public static MatrisBase<dynamic> operator -(dynamic val, MatrisBase<T> mat)
        {
            if (val == null)
            {
                return -mat;
            }
            else
            {
                if (float.TryParse(val.ToString(), out float res))
                {
                    List<List<T>> vals = mat.GetValues();
                    List<List<dynamic>> newlis = new List<List<dynamic>>();
                    for (int i = 0; i < mat.Row; i++)
                    {
                        newlis.Add(new List<dynamic>());
                        for (int j = 0; j < mat.Col; j++)
                        {
                            newlis[i].Add(res - float.Parse(vals[i][j].ToString()));
                        }
                    }
                    return new MatrisBase<dynamic>(newlis);
                }
                else
                {
                    throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
                }
            }
        }

        #endregion

        #region Multiplication
        public static MatrisBase<dynamic> operator *(MatrisBase<T> mat, MatrisBase<T> mat2)
        {

            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) * mat2;
            }

            if (mat2.IsScalar())
            {
                return mat * (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.MULTIPLICATION_SIZE_INVALID);
            }

            List<List<T>> vals1 = mat.GetValues();
            List<List<T>> vals2 = mat2.GetValues();
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                {
                    newlis[i].Add((dynamic)float.Parse(vals1[i][j].ToString()) * (dynamic)float.Parse(vals2[i][j].ToString()));
                }
            }
            return new MatrisBase<dynamic>(newlis);
        }

        public static MatrisBase<dynamic> operator *(MatrisBase<T> mat, dynamic val)
        {
            if (val == null)
            {
                val = 1;
            }

            if (float.TryParse(val.ToString(), out float res))
            {
                List<List<T>> vals = mat.GetValues();
                List<List<dynamic>> newlis = new List<List<dynamic>>();
                for (int i = 0; i < mat.Row; i++)
                {
                    newlis.Add(new List<dynamic>());
                    for (int j = 0; j < mat.Col; j++)
                    {
                        newlis[i].Add((dynamic)float.Parse(vals[i][j].ToString()) * res);
                    }
                }
                return new MatrisBase<dynamic>(newlis);
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }
        }
        public static MatrisBase<dynamic> operator *(dynamic val, MatrisBase<T> mat)
        {
            return mat * val;
        }

        #endregion

        #region Division

        public static MatrisBase<dynamic> operator /(MatrisBase<T> mat, MatrisBase<T> mat2)
        {

            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) / mat2;
            }

            if (mat2.IsScalar())
            {
                return mat / (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.DIVISION_SIZE_INVALID);
            }

            List<List<T>> vals1 = mat.GetValues();
            List<List<T>> vals2 = mat2.GetValues();
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                {
                    newlis[i].Add((dynamic)float.Parse(vals1[i][j].ToString()) / (dynamic)float.Parse(vals2[i][j].ToString()));
                }
            }
            return new MatrisBase<dynamic>(newlis);
        }

        public static MatrisBase<dynamic> operator /(MatrisBase<T> mat, dynamic val)
        {
            if (val == null)
            {
                val = 1;
            }

            if (float.TryParse(val.ToString(), out float res))
            {
                List<List<T>> vals = mat.GetValues();
                List<List<dynamic>> newlis = new List<List<dynamic>>();
                for (int i = 0; i < mat.Row; i++)
                {
                    newlis.Add(new List<dynamic>());
                    for (int j = 0; j < mat.Col; j++)
                    {
                        newlis[i].Add(float.Parse(vals[i][j].ToString()) / res);
                    }
                }
                return new MatrisBase<dynamic>(newlis);
            }
            else
            {
                throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
            }
        }
        public static MatrisBase<dynamic> operator /(dynamic val, MatrisBase<T> mat)
        {
            if (val == null)
            {
                List<List<T>> vals = mat.GetValues();
                List<List<dynamic>> newlis = new List<List<dynamic>>();
                for (int i = 0; i < mat.Row; i++)
                {
                    newlis.Add(new List<dynamic>());
                    for (int j = 0; j < mat.Col; j++)
                    {
                        newlis[i].Add(float.Parse(vals[i][j].ToString()));
                    }
                }
                return new MatrisBase<dynamic>(newlis);
            }
            else
            {
                if (float.TryParse(val.ToString(), out float res))
                {
                    List<List<T>> vals = mat.GetValues();
                    List<List<dynamic>> newlis = new List<List<dynamic>>();
                    for (int i = 0; i < mat.Row; i++)
                    {
                        newlis.Add(new List<dynamic>());
                        for (int j = 0; j < mat.Col; j++)
                        {
                            newlis[i].Add(res / float.Parse(vals[i][j].ToString()));
                        }
                    }
                    return new MatrisBase<dynamic>(newlis);
                }
                else
                {
                    throw new Exception(CompilerMessage.DYNAMIC_VAL_PARSE_FAILED);
                }

            }

        }

        #endregion

        #region Modulo
        public static MatrisBase<dynamic> operator %(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.IsScalar())
            {
                return (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) % mat2;
            }

            if (mat2.IsScalar())
            {
                return mat % (dynamic)float.Parse(mat2.GetValues()[0][0].ToString());
            }

            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
            {
                throw new Exception(CompilerMessage.MODULO_SIZE_INVALID);
            }

            List<List<T>> vals1 = mat.GetValues();
            List<List<T>> vals2 = mat2.GetValues();
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                {
                    newlis[i].Add((dynamic)float.Parse(vals1[i][j].ToString()) % (dynamic)float.Parse(vals2[i][j].ToString()));
                }
            }
            return new MatrisBase<dynamic>(newlis);
        }

        public static MatrisBase<dynamic> operator %(MatrisBase<T> mat, dynamic val)
        {
            if (val == null)
            {
                List<List<T>> vals = mat.GetValues();
                List<List<dynamic>> newlis = new List<List<dynamic>>();
                for (int i = 0; i < mat.Row; i++)
                {
                    newlis.Add(new List<dynamic>());
                    for (int j = 0; j < mat.Col; j++)
                    {
                        newlis[i].Add(float.Parse(vals[i][j].ToString()));
                    }
                }
                return new MatrisBase<dynamic>(newlis);
            }
            else
            {
                if (float.TryParse(val.ToString(), out float res))
                {
                    List<List<T>> vals = mat.GetValues();
                    List<List<dynamic>> newlis = new List<List<dynamic>>();
                    for (int i = 0; i < mat.Row; i++)
                    {
                        newlis.Add(new List<dynamic>());
                        for (int j = 0; j < mat.Col; j++)
                        {
                            newlis[i].Add(float.Parse(vals[i][j].ToString()) % res);
                        }
                    }
                    return new MatrisBase<dynamic>(newlis);
                }
                else
                {
                    throw new Exception(CompilerMessage.MODULO_PARSE_FAILED);
                }
            }
        }

        public static MatrisBase<dynamic> operator %(dynamic val, MatrisBase<T> mat)
        {
            if (mat.IsScalar())
            {
                if (float.TryParse(val.ToString(), out float res))
                {
                    return new MatrisBase<dynamic>()
                    {
                        Row = 1,
                        Col = 1,
                        _values = new List<List<dynamic>>()
                        { new List<dynamic>() { res % (dynamic)float.Parse(mat.GetValues()[0][0].ToString()) } }
                    };
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
    }
}
