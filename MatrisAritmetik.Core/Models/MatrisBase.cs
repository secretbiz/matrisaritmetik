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
        public int row
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

        public int col
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

        public List<List<T>> values
        { 
            get
            {
                if(_values != null)
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
                    if(_values == null) // Only set if first time
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
                    if(_values != null)
                        _values.Clear();
                }
            }
        }

        public string delimiter = ",";

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

            _values = new List<List<T>>();
            List<T> temp;

            for (int i = 0; i < r; i++)
            {
                temp = new List<T>();
                for (int j = 0; j < c; j++)
                {
                    temp.Add(fill);
                }
                values.Add(temp);

            }
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
            for(int i=0;i<row;i++)
            {
                lis.Add(new List<T>());
                for(int j=0;j<col;j++)
                {
                    lis[i].Add(values[i][j]);
                }
            }
            return new MatrisBase<T>(lis);
        }

        // Sütun genişliklerini bütün elemanlara bakarak int[] olarak döner 
        private int[] GetColumnWidths(MatrisBase<T> mat)
        {
            int[] longest_in_col = new int[mat.col];
            int currentmax;
            for (int j = 0; j < mat.col; j++)
            {
                currentmax = 0;
                for (int i = 0; i < mat.row; i++)
                {
                    if (mat.values[i][j].ToString().Length > currentmax)
                    {
                        currentmax = mat.values[i][j].ToString().Length;
                    }
                }
                longest_in_col[j] = currentmax;
            }

            return longest_in_col;

        }

        // Matrisi string olarak döner
        public string Printstr()
        {
            if (row == 0 || col == 0)
                return "";

            string str = "";

            // Column sizes
            int[] longest_in_col = GetColumnWidths(this);

            int colno;
            foreach(var row in values)
            {
                colno = 0;
                foreach(var element in row)
                {
                    str += new string(' ', (longest_in_col[colno] - element.ToString().Length)) + element.ToString();
                    
                    if (colno != col - 1)
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
            if (row == 0 || col == 0)
                return new MatrisBase<string>(1,1,"");

            MatrisBase<string> strmat = new MatrisBase<string>(row, col, "");

            // Column sizes
            int[] longest_in_col = GetColumnWidths(this);

            for (int j = 0; j < col; j++)
            {
                for (int i = 0; i < row; i++)
                {
                    strmat.values[i][j] = new string(' ', longest_in_col[j] - values[i][j].ToString().Length) + values[i][j].ToString();
                }
            }

            return strmat;
        }

        // Sütunu yeni bir liste olarak döner. 1-based
        public List<T> Row(int r, int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int j = 0; j < col; j++)
            {
                listrow.Add(values[r - based][j]);
            }
            return listrow;
        }

        // Satırı yeni bir matris olarak döner. 1-based
        public MatrisBase<T> RowMat(int r,int based=1)
        {
            List<List<T>> listrow = new List<List<T>>() { new List<T>() };
            for(int j=0;j<col;j++)
            {
                listrow[0].Add(values[r - based][j]);
            }
            return new MatrisBase<T>() { row = 1, col = col, values = listrow };
        }

        // Sütunu yeni bir matris olarak döner. 1-based
        public MatrisBase<T> ColMat(int c, int based = 1)
        {
            List<List<T>> listcol = new List<List<T>>();
            for (int i = 0; i < row; i++)
            {
                listcol.Add(new List<T>() { values[i][c - based] });
            }
            return new MatrisBase<T>() { row = row, col = 1, values = listcol };
        }

        // Sütunu yeni bir liste olarak döner. 1-based
        public List<T> Col(int c, int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int i = 0; i < row; i++)
            {
                listrow.Add(values[i][c - based]);
            }
            return listrow;
        }
        
        // Matrisin köşelerindeki değerleri içeren string matris döner
        // Matrisin boyutu yüksekse kullanılabilir
        public dynamic CornerMatrix(int rowEachCorner = -1, int colEachCorner = -1, string filler = "...")
        {
            if (row == 0 || col == 0)
                return new MatrisBase<T>();

            List<List<string>> smallerList = new List<List<string>>();

            if (rowEachCorner <= 0)
            {
                rowEachCorner = Math.Min((int)((float)row * 0.33), 4);
                rowEachCorner = (rowEachCorner == 0) ? row : rowEachCorner;
            }
            if (colEachCorner <= 0)
            {
                colEachCorner = Math.Min((int)((float)col * 0.33), 4);
                colEachCorner = (colEachCorner == 0) ? col : colEachCorner;
            }

            // No reduction
            if ( (((float)rowEachCorner) * 2.0 + 1.0 > (float)row) && (((float)colEachCorner) * 2.0 + 1.0 > (float)col))
            {
                return new MatrisBase<T>() { row = row, col = col, values = values };
            }
            // Only reduce columns
            else if (((float)rowEachCorner) * 2.0 + 1.0 > (float)row)
            {
                // Start reducing columns
                for (int i = 0; i < row; i++)
                {
                    smallerList.Add(new List<string>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[i].Add(values[i][left].ToString());
                    }

                    smallerList[i].Add(filler);

                    for (int right = col - colEachCorner; right < col; right++)
                    {
                        smallerList[i].Add(values[i][right].ToString());
                    }
                }

                return new MatrisBase<string>() { row = rowEachCorner * 2 + 1, col = colEachCorner * 2 + 1, values = smallerList };
            }
            // Only reduce rows
            else if (((float)colEachCorner) * 2.0 + 1.0 > (float)col)
            {
                // Start reducing rows
                // Upper half
                for (int u = 0; u < rowEachCorner; u++)
                {
                    smallerList.Add(new List<string>());
                    for (int left = 0; left < colEachCorner; left++)
                    {
                        smallerList[u].Add(values[u][left].ToString());
                    }
                }

                smallerList.Add(new List<string>());
                for (int j = 0; j < col; j++)
                {
                    smallerList[rowEachCorner].Add(filler);
                }

                int rrowind = rowEachCorner + 1;
                // Bottom half
                for (int i = row - rowEachCorner; i < row; i++)
                {
                    smallerList.Add(new List<string>());
                    for (int bleft = 0; bleft < colEachCorner; bleft++)
                    {
                        smallerList[rrowind].Add(values[i][bleft].ToString());
                    }
                    rrowind++;
                }

                return new MatrisBase<string>() { row = rowEachCorner * 2 + 1, col = colEachCorner * 2 + 1, values = smallerList };
            }

            // Reduce both rows and columns
            // Upper half
            for (int u=0; u<rowEachCorner; u++)
            {
                smallerList.Add(new List<string>());
                for(int left = 0; left < colEachCorner; left++)
                {
                    smallerList[u].Add(values[u][left].ToString());
                }

                smallerList[u].Add(filler);

                for (int right = col-colEachCorner; right < col; right++)
                {
                    smallerList[u].Add(values[u][right].ToString());
                }
            }

            smallerList.Add(new List<string>());
            for (int j = 0; j < colEachCorner * 2 + 1; j++)
            {
                smallerList[rowEachCorner].Add(filler);
            }

            int rowind = rowEachCorner + 1;
            // Bottom half
            for (int i = row-rowEachCorner; i < row; i++)
            {
                smallerList.Add(new List<string>());
                for (int bleft = 0; bleft < colEachCorner; bleft++)
                {
                    smallerList[rowind].Add(values[i][bleft].ToString());
                }

                smallerList[rowind].Add(filler);

                for (int bright = col - colEachCorner; bright < col; bright++)
                {
                    smallerList[rowind].Add(values[i][bright].ToString());
                }
                rowind++;
            }

            return new MatrisBase<string>() { row = rowEachCorner * 2 + 1, col = colEachCorner * 2 + 1, values = smallerList };
        }

        public int ElementCount()
        {
            return col * row;
        }

        public IEnumerator<List<T>> GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
