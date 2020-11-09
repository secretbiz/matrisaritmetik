using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrisAritmetik.Core
{
    public class MatrisBase<T>
    {
        public int swapCount = 0;
        private int _row = -1;
        private int _col = -1;
        private List<List<T>> _values = null;
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                if (_row == -1) // Only set if first time
                {
                    if (value > (int)MatrisLimits.forRows)
                        _row = (int)MatrisLimits.forRows;
                    else
                        _row = value;
                }
            }
        }

        public int Col
        {
            get
            {
                return _col;
            }
            set
            {
                if (_col == -1) // Only set if first time
                {
                    if (value > (int)MatrisLimits.forCols)
                        _col = (int)MatrisLimits.forCols;
                    else
                        _col = value;
                }
            }
        }
        public void SetCol(int c)
        { _col = c; }

        public List<List<T>> Values
        {
            get
            {
                if (_values != null)
                {
                    if (_values.Count * _values[0].Count > (int)MatrisLimits.forSize)
                    {
                        if (_values.Count > (int)MatrisLimits.forRows)
                        {
                            int delindex = (int)MatrisLimits.forRows;
                            for (int i = 0; i < _values.Count - delindex; i++)
                                _values.RemoveAt(delindex);
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
                                            _values[i].RemoveAt(delindex);
                                    }
                                }
                            }
                        }
                    }
                }

                return _values;

            }
            set // Just clones it
            {
                if (value != null)
                {
                    if (_values == null) // Only set if first time
                    {
                        if (value.Count * value[0].Count > (int)MatrisLimits.forSize)
                        {
                            List<List<T>> temp = new List<List<T>>();
                            int collimit = (int)MatrisLimits.forCols;

                            for (int i = 0; i < Math.Min(value.Count, (int)MatrisLimits.forRows); i++)
                            {
                                temp.Add(new List<T>());
                                for (int j = 0; j < Math.Min(value[i].Count, collimit); j++)
                                    temp[i].Add(value[i][j]);

                            }
                            _values = temp;
                        }
                        else
                            _values = value;
                    }
                }
                else
                {
                    if (_values != null)
                        _values.Clear();
                }
            }
        }

        public string delimiter = " ";

        // Emtpy 3x3 constructor
        public MatrisBase() { }

        // Constructor with row, column, fill
        public MatrisBase(int r, int c, T fill)
        {
            if (r > (int)MatrisLimits.forRows)
                r = (int)MatrisLimits.forRows;

            if (c > (int)MatrisLimits.forCols)
                c = (int)MatrisLimits.forCols;

            _row = r; _col = c;

            List<List<T>> vals = new List<List<T>>();
            for (int i = 0; i < r; i++)
            {
                vals.Add(new List<T>());
                for (int j = 0; j < c; j++)
                {
                    vals[i].Add(fill);
                }
            }
            _values = vals;
        }

        // Constructor with given list of list of values
        public MatrisBase(List<List<T>> vals)
        {
            _row = vals.Count;
            if (_row == 0)
            {
                _col = 0;
                return;
            }

            _col = vals.ElementAt(0).Count;

            _values = vals;
        }

        // Matris kopyası
        public MatrisBase<T> Copy()
        {
            List<List<T>> lis = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                lis.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    lis[i].Add(Values[i][j]);
                }
            }
            return new MatrisBase<T>(lis);
        }

        // Sütun genişliklerini bütün elemanlara bakarak int[] olarak döner 
        private int[] GetColumnWidths(MatrisBase<T> mat)
        {
            int[] longest_in_col = new int[mat.Col];
            int currentmax;
            for (int j = 0; j < mat.Col; j++)
            {
                currentmax = 0;
                for (int i = 0; i < mat.Row; i++)
                {
                    if (mat.Values[i][j].ToString().Length > currentmax)
                    {
                        currentmax = mat.Values[i][j].ToString().Length;
                    }
                }
                longest_in_col[j] = currentmax;
            }

            return longest_in_col;

        }

        // Matrisi string olarak döner
        public string Printstr()
        {
            bool stringmat = typeof(T) == typeof(String);

            if (Row == 0 || Col == 0)
                return "";

            string str = "";

            // Column sizes
            int[] longest_in_col = GetColumnWidths(this);

            int colno;
            foreach (var row in Values)
            {
                colno = 0;
                foreach (var element in row)
                {
                    if (stringmat)
                        str += element.ToString();
                    else
                        str += new string(' ', (longest_in_col[colno] - element.ToString().Length)) + element.ToString();

                    if (colno != Col - 1)
                        str += delimiter;

                    colno++;
                }
                str += "\n";
            }

            return str;
        }

        // Matris değerlerinin sütün genişliklerine göre padlenmiş hallerini matris olarak döner
        public MatrisBase<string> PrintStrMaxtrisForm()
        {
            if (Row == 0 || Col == 0)
                return new MatrisBase<string>(1, 1, "");

            MatrisBase<string> strmat = new MatrisBase<string>(Row, Col, "");

            // Column sizes
            int[] longest_in_col = GetColumnWidths(this);

            for (int j = 0; j < Col; j++)
            {
                for (int i = 0; i < Row; i++)
                {
                    strmat.Values[i][j] = new string(' ', longest_in_col[j] - Values[i][j].ToString().Length) + Values[i][j].ToString();
                }
            }

            return strmat;
        }

        //// Indexers, base 0
        // Rows
        public List<List<T>> this[Range r]
        {
            get
            {
                if (r.Start.Equals(r.End) || r.End.Value > _row || r.Start.Value >= _row)
                    return null;


                return _values.GetRange(r.Start.Value, r.End.Value - r.Start.Value);
            }
            set
            {
                if (r.Start.Equals(r.End) || r.End.Value > _row || r.Start.Value >= _row)
                    return;

                List<List<T>> newVals = new List<List<T>>();
                List<T>[] temp = new List<T>[_row];

                if (r.Start.Value > 0)
                    newVals.AddRange(_values.GetRange(0, r.Start.Value));

                newVals.AddRange(value);

                if (_row - r.End.Value > 0)
                    newVals.AddRange(_values.GetRange(r.End.Value, _row - r.End.Value));

                newVals.CopyTo(temp);
                newVals.Clear();

                _values.Clear();
                _values = new List<List<T>>(temp);
            }
        }

        // Row
        public List<T> this[int index]
        {

            get
            {
                if (index >= _row)
                    return null;
                return _values[index];
            }
            set
            {
                if (index >= _row)
                    return;
                _values[index] = value;
            }
        }

        // Single value
        public T this[int r, int c]
        {
            get
            {
                if (r >= _row)
                    return default;

                if (c >= _col)
                    return default;

                return _values[r][c];
            }
            set
            {
                if (r >= _row)
                    return;

                if (c >= _col)
                    return;

                _values[r][c] = value;
            }
        }

        // Cols and rows
        public List<List<T>> this[Range r, Range c]
        {
            get
            {
                if (r.Start.Equals(r.End) || r.End.Value > _row || r.Start.Value >= _row)
                    return null;

                if (c.Start.Equals(c.End) || c.End.Value > _col || c.Start.Value >= _col)
                    return null;

                List<List<T>> newVals = new List<List<T>>();
                for (int i = r.Start.Value; i < r.End.Value; i++)
                {
                    List<T> temp = new List<T>();
                    for (int j = c.Start.Value; j < c.End.Value; j++)
                        temp.Add(_values[i][j]);

                    newVals.Add(temp);
                }
                return newVals;
            }
            set
            {

                if (r.Start.Equals(r.End) || r.End.Value > _row || r.Start.Value >= _row)
                    return;

                if (c.Start.Equals(c.End) || c.End.Value > _col || c.Start.Value >= _col)
                    return;

                for (int i = r.Start.Value; i < r.End.Value; i++)
                {
                    List<T> newVals = new List<T>();
                    T[] temp = new T[_col];

                    if (c.Start.Value > 0)
                        newVals.AddRange(_values[i].GetRange(0, c.Start.Value));

                    newVals.AddRange(value[i]);

                    if (_col - c.End.Value > 0)
                        newVals.AddRange(_values[i].GetRange(c.End.Value, _col - c.End.Value));

                    newVals.CopyTo(temp);
                    newVals.Clear();

                    _values[i].Clear();
                    _values[i] = new List<T>(temp);
                }

            }
        }
        ////

        // Sütunu yeni bir liste olarak döner. 1-based
        public List<T> RowList(int r, int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int j = 0; j < Col; j++)
            {
                listrow.Add(Values[r - based][j]);
            }
            return listrow;
        }

        // Satırı yeni bir matris olarak döner. 1-based
        public MatrisBase<T> RowMat(int r, int based = 1)
        {
            List<List<T>> listrow = new List<List<T>>() { new List<T>() };
            for (int j = 0; j < Col; j++)
            {
                listrow[0].Add(Values[r - based][j]);
            }
            return new MatrisBase<T>() { Row = 1, Col = Col, Values = listrow };
        }

        // Sütunu yeni bir matris olarak döner. 1-based
        public MatrisBase<T> ColMat(int c, int based = 1)
        {
            List<List<T>> listcol = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                listcol.Add(new List<T>() { Values[i][c - based] });
            }
            return new MatrisBase<T>() { Row = Row, Col = 1, Values = listcol };
        }

        // Sütunu yeni bir liste olarak döner. 1-based
        public List<T> ColList(int c, int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int i = 0; i < Row; i++)
            {
                listrow.Add(Values[i][c - based]);
            }
            return listrow;
        }

        // Matrisin köşelerindeki değerleri içeren string matris döner
        // Matrisin boyutu yüksekse kullanılabilir
        public dynamic CornerMatrix(int rowEachCorner = -1, int colEachCorner = -1, string filler = "...")
        {
            if (Row == 0 || Col == 0)
                return new MatrisBase<T>();

            List<List<string>> smallerList = new List<List<string>>();

            if (rowEachCorner <= 0)
            {
                rowEachCorner = Math.Min((int)((float)Row * 0.33), 4);
                rowEachCorner = (rowEachCorner == 0) ? Row : rowEachCorner;
            }
            if (colEachCorner <= 0)
            {
                colEachCorner = Math.Min((int)((float)Col * 0.33), 4);
                colEachCorner = (colEachCorner == 0) ? Col : colEachCorner;
            }

            // No reduction
            if ((((float)rowEachCorner) * 2.0 + 1.0 > (float)Row) && (((float)colEachCorner) * 2.0 + 1.0 > (float)Col))
            {
                return new MatrisBase<T>() { Row = Row, Col = Col, Values = Values };
            }
            // Only reduce columns
            else if (((float)rowEachCorner) * 2.0 + 1.0 > (float)Row)
            {
                // Start reducing columns
                for (int i = 0; i < Row; i++)
                {
                    smallerList.Add(new List<string>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[i].Add(Values[i][left].ToString());
                    }

                    smallerList[i].Add(filler);

                    for (int right = Col - colEachCorner; right < Col; right++)
                    {
                        smallerList[i].Add(Values[i][right].ToString());
                    }
                }

                return new MatrisBase<string>() { Row = rowEachCorner * 2 + 1, Col = colEachCorner * 2 + 1, Values = smallerList };
            }
            // Only reduce rows
            else if (((float)colEachCorner) * 2.0 + 1.0 > (float)Col)
            {
                // Start reducing rows
                // Upper half
                for (int u = 0; u < rowEachCorner; u++)
                {
                    smallerList.Add(new List<string>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[u].Add(Values[u][left].ToString());
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
                        smallerList[rrowind].Add(Values[i][bleft].ToString());
                    }
                    rrowind++;
                }

                return new MatrisBase<string>() { Row = rowEachCorner * 2 + 1, Col = colEachCorner * 2 + 1, Values = smallerList };
            }

            // Reduce both rows and columns
            // Upper half
            for (int u = 0; u < rowEachCorner; u++)
            {
                smallerList.Add(new List<string>());
                for (int left = 0; left < colEachCorner; left++)
                {
                    smallerList[u].Add(Values[u][left].ToString());
                }

                smallerList[u].Add(filler);

                for (int right = Col - colEachCorner; right < Col; right++)
                {
                    smallerList[u].Add(Values[u][right].ToString());
                }
            }

            smallerList.Add(new List<string>());
            for (int j = 0; j < colEachCorner * 2 + 1; j++)
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
                    smallerList[rowind].Add(Values[i][bleft].ToString());
                }

                smallerList[rowind].Add(filler);

                for (int bright = Col - colEachCorner; bright < Col; bright++)
                {
                    smallerList[rowind].Add(Values[i][bright].ToString());
                }
                rowind++;
            }

            return new MatrisBase<string>() { Row = rowEachCorner * 2 + 1, Col = colEachCorner * 2 + 1, Values = smallerList };
        }

        public int ElementCount()
        {
            return Col * Row;
        }

        public MatrisBase<T> Power(int n)
        {
            List<List<T>> newlist = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                newlist.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    newlist[i].Add(PowerMethod((dynamic)(float.Parse(Values[i][j].ToString())), n));
                }
            }
            return new MatrisBase<T>(newlist);
        }

        public void MulRow(int row, float factor, int based = 1)
        {
            for (int j = 0; j < Col; j++)
            {
                _values[row - based][j] = (dynamic)(float.Parse(_values[row - based][j].ToString()) * factor);
            }
        }

        public void SwapToEnd(int a, int based = 1)
        {
            a -= based;
            // Row swap
            T[] temp = new T[Col];
            Values[a].CopyTo(temp);
            _values.Remove(Values[a]);
            _values.Add(new List<T>(temp));
        }

        public void Swap(int a, int b, int axis = 0, int based = 1)
        {
            if (a == b)
                return;
            a -= based;
            b -= based;
            // Row swap
            if (axis == 0)
            {
                T[] temp = new T[Col];
                Values[a].CopyTo(temp);
                Values[a] = new List<T>(Values[b]);
                Values[b] = new List<T>(temp);
            }
            // Column swap
            else
            {
                T temp;
                for (int i = 0; i < Row; i++)
                {
                    temp = Values[i][a];
                    Values[i][a] = Values[i][b];
                    Values[i][b] = temp;
                }
            }
        }

        public void FixMinusZero()
        {
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    if (_values[i][j].ToString() == "-0" || Math.Abs(float.Parse(_values[i][j].ToString())) < 1e-6)
                        _values[i][j] = (dynamic)(float)0.0;
                }
            }
        }

        public MatrisBase<T> Round(int decimals = 5)
        {
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                    _values[i][j] = (dynamic)(float)Math.Round(float.Parse(_values[i][j].ToString()), decimals);
            }
            return this;
        }

        //// Matrix features
        // Valid ?
        public bool IsValid()
        {
            return (Row > 0) && (Col > 0);
        }

        // Square ?
        public bool IsSquare()
        {
            return Row == Col;
        }

        // Zero matrix ?
        public bool IsZero(float tolerance = (float)0.00001)
        {
            static bool inRange(float num, float tol) => (num <= tol) && (num >= -tol);

            bool isZero = true;
            List<List<T>> vals1 = Values;
            for (int i = 0; i < Row; i++)
            {
                if (!isZero)
                    break;
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
        public bool IsZeroCol(int col_index, int based = 1, float tolerance = (float)0.00001)
        {
            return ColMat(col_index, based).IsZero(tolerance);
        }
        public bool IsZeroRow(int row_index, int based = 1, float tolerance = (float)0.00001)
        {
            return RowMat(row_index, based).IsZero(tolerance);
        }

        public bool IsSameSize(MatrisBase<T> other)
        {
            return (Row == other.Row) && (Col == other.Col);
        }

        // Overloads
        public IEnumerator<List<T>> GetEnumerator()
        {
            return Values.GetEnumerator();
        }
        public override string ToString()
        {
            try
            {
                return Round(4).PrintStrMaxtrisForm().Printstr();
            }
            catch (Exception)
            {
                return PrintStrMaxtrisForm().Printstr();
            }
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        ////// Static stuff
        //// Parse as float matrix
        public static List<List<float>> FloatListParse(MatrisBase<T> mat)
        {
            List<List<float>> floatlis = new List<List<float>>();
            for (int i = 0; i < mat.Row; i++)
            {
                floatlis.Add(new List<float>());
                for (int j = 0; j < mat.Col; j++)
                    floatlis[i].Add((float.Parse(mat.Values[i][j].ToString())));
            }
            return floatlis;
        }
        //// a^b handling
        public static dynamic PowerMethod(dynamic a, dynamic b)
        {
            dynamic result;
            if (b == 0)
            {
                if (a == 0) // 0^0
                    result = double.NaN;
                else
                    result = 1;    // x^0
            }
            else if (b < 0)
            {
                if (a == 0)       // 0^(-x)
                    result = double.PositiveInfinity;
                else                 // y^(-x)
                {
                    dynamic val = 1;
                    for (int i = 0; i < (b * -1); i++)
                        val *= a;

                    result = 1.0 / val;
                }
            }
            else
            {
                if (a == 0)   // 0^x
                    result = 0;
                else                          // y^x
                {
                    dynamic val = 1;
                    for (int i = 0; i < b; i++)
                        val *= a;

                    result = val;
                }
            }
            return result;
        }

        ////// Operator overloads
        //// Equals
        // Mat == Mat
        public static bool operator ==(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                return false;
            bool isEqual = true;
            List<List<T>> vals1 = mat.Values;
            List<List<T>> vals2 = mat2.Values;
            for (int i = 0; i < mat.Row; i++)
            {
                if (!isEqual)
                    break;
                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)(float.Parse(vals1[i][j].ToString())) != (dynamic)(float.Parse(vals2[i][j].ToString())))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }
        // Mat == List<List<dynamic>>
        public static bool operator ==(MatrisBase<T> mat, List<List<dynamic>> lis)
        {
            if (mat.Row != lis.Count || lis.Count == 0)
                return false;
            if (lis[0].Count != mat.Col)
                return false;

            bool isEqual = true;
            List<List<T>> vals1 = mat.Values;
            for (int i = 0; i < mat.Row; i++)
            {
                if (!isEqual)
                    break;
                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)(float.Parse(vals1[i][j].ToString())) != (dynamic)(float.Parse(lis[i][j].ToString())))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }
        // List<List<dynamic>> == Mat
        public static bool operator ==(List<List<dynamic>> lis, MatrisBase<T> mat)
        {
            if (mat.Row != lis.Count || lis.Count == 0)
                return false;
            if (lis[0].Count != mat.Col)
                return false;

            bool isEqual = true;
            List<List<T>> vals1 = mat.Values;
            for (int i = 0; i < mat.Row; i++)
            {
                if (!isEqual)
                    break;
                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)(float.Parse(vals1[i][j].ToString())) != (dynamic)(float.Parse(lis[i][j].ToString())))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }
        //// Not Equals
        // Mat != Mat
        public static bool operator !=(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                return true;
            bool isNotEqual = false;
            List<List<T>> vals1 = mat.Values;
            List<List<T>> vals2 = mat2.Values;
            for (int i = 0; i < mat.Row; i++)
            {
                if (isNotEqual)
                    break;
                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)(float.Parse(vals1[i][j].ToString())) != (dynamic)(float.Parse(vals2[i][j].ToString())))
                    {
                        isNotEqual = true;
                        break;
                    }
                }
            }
            return isNotEqual;
        }
        // Mat != List<List<dynamic>>
        public static bool operator !=(MatrisBase<T> mat, List<List<dynamic>> lis)
        {
            if (mat.Row != lis.Count || lis.Count == 0)
                return true;
            if (lis[0].Count != mat.Col)
                return true;

            bool isNotEqual = false;
            List<List<T>> vals1 = mat.Values;
            for (int i = 0; i < mat.Row; i++)
            {
                if (isNotEqual)
                    break;
                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)(float.Parse(vals1[i][j].ToString())) != (dynamic)(float.Parse(lis[i][j].ToString())))
                    {
                        isNotEqual = true;
                        break;
                    }
                }
            }
            return isNotEqual;
        }
        // List<List<dynamic>> != Mat
        public static bool operator !=(List<List<dynamic>> lis, MatrisBase<T> mat)
        {
            if (mat.Row != lis.Count || lis.Count == 0)
                return true;
            if (lis[0].Count != mat.Col)
                return true;

            bool isNotEqual = false;
            List<List<T>> vals1 = mat.Values;
            for (int i = 0; i < mat.Row; i++)
            {
                if (isNotEqual)
                    break;
                for (int j = 0; j < mat.Col; j++)
                {
                    if ((dynamic)(float.Parse(vals1[i][j].ToString())) != (dynamic)(float.Parse(lis[i][j].ToString())))
                    {
                        isNotEqual = true;
                        break;
                    }
                }
            }
            return isNotEqual;
        }
        //// Addition
        // Unary
        public static MatrisBase<T> operator +(MatrisBase<T> mat) => mat;
        // Mat + Mat
        public static MatrisBase<dynamic> operator +(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                throw new Exception("Matris boyutları toplama işlemi için aynı olmalı");

            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(mat.Values[i][j].ToString())) + (dynamic)(float.Parse(mat2.Values[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // Mat + val
        public static MatrisBase<dynamic> operator +(MatrisBase<T> mat, dynamic val)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) + val);
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // val + Mat
        public static MatrisBase<dynamic> operator +(dynamic val, MatrisBase<T> mat)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) + val);
            }
            return new MatrisBase<dynamic>(newlis);
        }

        //// Subtraction
        // Unary
        public static MatrisBase<dynamic> operator -(MatrisBase<T> mat)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) * -1);
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // Mat - Mat
        public static MatrisBase<dynamic> operator -(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                throw new Exception("Matris boyutları çıkarma işlemi için aynı olmalı");

            List<List<T>> vals1 = mat.Values;
            List<List<T>> vals2 = mat2.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals1[i][j].ToString())) - (dynamic)(float.Parse(vals2[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // Mat - val
        public static MatrisBase<dynamic> operator -(MatrisBase<T> mat, dynamic val)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) - val);
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // val - Mat
        public static MatrisBase<dynamic> operator -(dynamic val, MatrisBase<T> mat)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add(val - (dynamic)(float.Parse(vals[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }

        //// Multiplication
        // Mat * Mat
        public static MatrisBase<dynamic> operator *(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                throw new Exception("Matris boyutları çarpma işlemi için aynı olmalı");

            List<List<T>> vals1 = mat.Values;
            List<List<T>> vals2 = mat2.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals1[i][j].ToString())) * (dynamic)(float.Parse(vals2[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // Mat * val
        public static MatrisBase<dynamic> operator *(MatrisBase<T> mat, dynamic val)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) * val);
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // val * Mat
        public static MatrisBase<dynamic> operator *(dynamic val, MatrisBase<T> mat)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) * val);
            }
            return new MatrisBase<dynamic>(newlis);
        }

        //// Division
        // Mat / Mat
        public static MatrisBase<dynamic> operator /(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                throw new Exception("Matris boyutları bölme işlemi için aynı olmalı");

            List<List<T>> vals1 = mat.Values;
            List<List<T>> vals2 = mat2.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals1[i][j].ToString())) / (dynamic)(float.Parse(vals2[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // Mat / val
        public static MatrisBase<dynamic> operator /(MatrisBase<T> mat, dynamic val)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) / val);
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // val / Mat
        public static MatrisBase<dynamic> operator /(dynamic val, MatrisBase<T> mat)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add(val / (dynamic)(float.Parse(vals[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }

        //// Modulo
        // Mat % Mat
        public static MatrisBase<dynamic> operator %(MatrisBase<T> mat, MatrisBase<T> mat2)
        {
            if (mat.Row != mat2.Row || mat.Col != mat2.Col)
                throw new Exception("Matris boyutları bölme işlemi için aynı olmalı");

            List<List<T>> vals1 = mat.Values;
            List<List<T>> vals2 = mat2.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals1[i][j].ToString())) % (dynamic)(float.Parse(vals2[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // Mat % val
        public static MatrisBase<dynamic> operator %(MatrisBase<T> mat, dynamic val)
        {
            List<List<T>> vals = mat.Values;
            List<List<dynamic>> newlis = new List<List<dynamic>>();
            for (int i = 0; i < mat.Row; i++)
            {
                newlis.Add(new List<dynamic>());
                for (int j = 0; j < mat.Col; j++)
                    newlis[i].Add((dynamic)(float.Parse(vals[i][j].ToString())) % val);
            }
            return new MatrisBase<dynamic>(newlis);
        }
        // val % Mat
        public static MatrisBase<dynamic> operator %(dynamic val, MatrisBase<T> mat)
        {
            throw new Exception("Matris mod olarak kullanılamaz!");
        }
    }
}
