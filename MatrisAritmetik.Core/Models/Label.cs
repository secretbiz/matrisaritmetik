using System;
using System.Collections.Generic;

namespace MatrisAritmetik.Core.Models
{
    public class Label : IDisposable
    {
        #region Private Encapsulated Fields
        private int _span = 1;
        private string _value = "empty_label";
        private bool disposedValue;
        #endregion

        #region Public Properties
        public int Span
        {
            get => _span;
            set => _span = (value > 0) ? value : _span;
        }

        public string Value
        {
            get => _value;
            set => _value = (value.Length > 0) ? value : _value;
        }
        #endregion

        #region Constructors
        public Label() { }

        public Label(string val, int span = 1)
        {
            Value = val;
            Span = span;
        }
        #endregion

        #region Public Methods
        public Label Copy()
        {
            return new Label(_value.ToString(), _span);
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                _value = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Label()
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

    public class LabelList : IDisposable
    {
        #region Private Encapsulated Fields
        private int _length = 0;
        private int _totalspan = 0;
        private List<Label> _labels;
        private bool disposedValue;
        #endregion

        #region Public Properties
        public int Length => _length;
        public int TotalSpan => _totalspan;

        public List<Label> Labels
        {
            get => _labels;
            set
            {
                _length = value.Count;
                _labels = value;
                _totalspan = 0;
                foreach (Label lbl in value)
                {
                    _totalspan += lbl.Span;
                }
            }
        }
        #endregion

        #region Constructors
        public LabelList() { }

        public LabelList(int length, int spaneach = 1, string prefix = "col_", int based = 1)
        {
            _labels = new List<Label>();
            _length = length;
            _totalspan = length * spaneach;

            if (length > 0)
            {
                for (int i = based; i < length + based; i++)
                {
                    _labels.Add(new Label(prefix + i.ToString(), spaneach));
                }
            }

        }

        public LabelList(List<Label> labels) { Labels = labels; }
        #endregion

        #region Public Methods
        public LabelList GetRange(int start, int end)
        {
            Label[] temp = new Label[end - start];
            for (int i = start; i < end; i++)
            {
                temp[i - start] = _labels[i].Copy();
            }
            return new LabelList(new List<Label>(temp));
        }

        public int GetLabelIndexAtSpan(int ind)
        {
            if (_totalspan < ind)
            {
                return -1;
            }
            if (_length == 1)
            {
                return 0;
            }

            int index = 0;
            int total = _labels[0].Span;
            while (total < ind)
            {
                index++;
                total += _labels[index].Span;
            }
            return index;
        }

        public LabelList Copy()
        {
            Label[] temp = new Label[_length];
            for (int i = 0; i < _length; i++)
            {
                temp[i] = _labels[i].Copy();
            }
            return new LabelList(new List<Label>(temp));
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_labels != null)
                    {
                        _labels.Clear();
                        _labels = null;
                    }
                }

                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~LabelList()
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
}
