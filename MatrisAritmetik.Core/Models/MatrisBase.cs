using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MatrisAritmetik.Core
{
    public class MatrisBase<T>
    {
        public int row = 3;
        public int col = 3;
        public List<List<T>> values = new List<List<T>>();

        public string delimiter = ",";

        // Emtpy 3x3 constructor
        public MatrisBase() {}

        // Constructor with row, column, fill
        public MatrisBase(int r,int c, T fill) 
        { 
            row = r; col = c;

            values = new List<List<T>>();
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
            row = vals.Count;
            if (row == 0)
            {
                col = 0;
                return;
            }

            col = vals.ElementAt(0).Count;

            values = vals;
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

        // Satırı matris olarak döner. 1-based
        public MatrisBase<T> RowMat(int r,int based=1)
        {
            List<List<T>> listrow = new List<List<T>>() { new List<T>() };
            for(int j=0;j<col;j++)
            {
                listrow[0].Add(values[r - based][j]);
            }
            return new MatrisBase<T>() { row = 1, col = col, values = listrow };
        }

        // Sütunu matris olarak döner. 1-based
        public MatrisBase<T> ColMat(int c, int based = 1)
        {
            List<List<T>> listcol = new List<List<T>>();
            for (int i = 0; i < row; i++)
            {
                listcol.Add(new List<T>() { values[i][c - based] });
            }
            return new MatrisBase<T>() { row = row, col = 1, values = listcol };
        }

        // Sütunu liste olarak döner. 1-based
        public List<T> Col(int c, int based = 1)
        {
            List<T> listrow = new List<T>();
            for (int i = 0; i < row; i++)
            {
                listrow.Add(values[i][c - based]);
            }
            return listrow;
        }

        public IEnumerator<List<T>> GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
