using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;

namespace MatrisAritmetik.Core
{
    public class MatrisBase<T>
    {
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

        private T Pow(dynamic a, int n)
        {
            dynamic a1 = a;
            for (int i = 0; i < n - 1; i++)
                a1 *= a;
            return a1;
        }

        public MatrisBase<T> Power(int n)
        {
            List<List<T>> newlist = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                newlist.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    newlist[i].Add(Pow((dynamic)(float.Parse(Values[i][j].ToString())), n));
                }
            }
            return new MatrisBase<T>(newlist);
        }

        public MatrisBase<T> Modulo(int n)
        {
            List<List<T>> newlist = new List<List<T>>();
            for (int i = 0; i < Row; i++)
            {
                newlist.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    newlist[i].Add((dynamic)(float.Parse(Values[i][j].ToString())) % n);
                }
            }
            return new MatrisBase<T>(newlist);
        }

        // this % mat
        public MatrisBase<T> Modulo(MatrisBase<T> mat)
        {
            if (Row != mat.Row || Col != mat.Col)
                throw new Exception("Modülo operatörü için boyutlar uyuşmalı!");

            List<List<T>> newlist = new List<List<T>>();
            List<List<T>> mat2 = mat.Values;
            for (int i = 0; i < Row; i++)
            {
                newlist.Add(new List<T>());
                for (int j = 0; j < Col; j++)
                {
                    newlist[i].Add((dynamic)(float.Parse(Values[i][j].ToString())) % (dynamic)(float.Parse(mat2[i][j].ToString())));
                }
            }
            return new MatrisBase<T>(newlist);
        }

        // Matrix features
        public bool IsSquare()
        {
            return Row == Col;
        }

        // Overloads
        public IEnumerator<List<T>> GetEnumerator()
        {
            return Values.GetEnumerator();
        }
        public override string ToString()
        {
            return PrintStrMaxtrisForm().Printstr();
        }
        ////// Parsers
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

        ////// Operator overloads
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
            for(int i=0;i<mat.Row;i++)
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
                    newlis[i].Add( val - (dynamic)(float.Parse(vals[i][j].ToString())));
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
                    newlis[i].Add( val / (dynamic)(float.Parse(vals[i][j].ToString())));
            }
            return new MatrisBase<dynamic>(newlis);
        }

    }
}
