using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// A class to represent one or many rows or column with a given name
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Label : IDisposable
    {
        #region Private Encapsulated Fields
        /// <summary>
        /// Span of this label
        /// </summary>
        private int _span = 1;
        /// <summary>
        /// Value/name of this label
        /// </summary>
        private string _value = "empty_label";
        private bool disposedValue;
        #endregion

        #region Public Properties
        /// <summary>
        /// Span of this label, can't be negative
        /// </summary>
        public int Span
        {
            get => _span;
            set => _span = (value > 0) ? value : _span;
        }

        /// <summary>
        /// Value/name of this label, can't be null or length 0
        /// </summary>
        public string Value
        {
            get => _value;
            set => _value = (value == null)
                                ? _value
                                : (value.Length > 0)
                                    ? value
                                    : _value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Label() { }

        /// <summary>
        /// Create a label which spans over given <paramref name="span"/> and has value of <paramref name="val"/>
        /// </summary>
        /// <param name="val">Value/name for this label, non-null and non-0-length</param>
        /// <param name="span">Span of the label, non-negative and non-zero</param>
        public Label(string val, int span = 1)
        {
            Value = val;
            Span = span;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a deep copy of this <see cref="Label"/>
        /// </summary>
        /// <returns>A new <see cref="Label"/></returns>
        public Label Copy()
        {
            return new Label(_value.ToString(), _span);
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return Value + ", span(" + Span + ")";
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _value = string.Empty;
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

    /// <summary>
    /// A class to keep <see cref="Label"/>s together to represent a row or column dimension
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class LabelList : IDisposable
    {
        #region Private Encapsulated Fields
        /// <summary>
        /// List of all <see cref="Label"/>s
        /// </summary>
        private List<Label> _labels = new List<Label>();

        private bool disposedValue;
        #endregion

        #region Public Properties
        /// <summary>
        /// Amount of <see cref="Label"/>s this list has
        /// </summary>
        public int Length => Labels == null ? 0 : Labels.Count;
        /// <summary>
        /// Total span of this list's <see cref="Label"/>s
        /// </summary>
        public int TotalSpan
        {
            get
            {
                if (Labels == null)
                {
                    return 0;
                }

                int t = 0;
                foreach (Label l in Labels)
                {
                    t += l.Span;
                }
                return t;
            }
        }
        /// <summary>
        /// Get new list of all <see cref="Label"/>s
        /// </summary>
        /// <summary>
        /// Set the list of all <see cref="Label"/>s
        /// </summary>
        public List<Label> Labels
        {
            get => _labels;
            set => _labels = value ?? new List<Label>();
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public LabelList() { }

        /// <summary>
        /// Create a <see cref="LabelList"/> with given <paramref name="length"/> where each label has a span of <paramref name="spaneach"/>
        /// <para>Each label's value starts with <paramref name="prefix"/> and ends with it's index(base as <paramref name="based"/>)</para>
        /// </summary>
        /// <param name="length">Amount of labels for this list to have</param>
        /// <param name="spaneach">Span for each label</param>
        /// <param name="prefix">Prefix string to add to each <see cref="Label"/></param>
        /// <param name="based">Base for indices to append to each <see cref="Label"/>'s value</param>
        public LabelList(int length, int spaneach = 1, string prefix = "col_", int based = 1)
        {
            _labels = new List<Label>();

            if (length > 0)
            {
                for (int i = based; i < length + based; i++)
                {
                    _labels.Add(new Label(prefix + i.ToString(), spaneach));
                }
            }

        }

        /// <summary>
        /// Create a <see cref="LabelList"/> from given list of <see cref="Label"/>s
        /// </summary>
        /// <param name="labels">List of labels</param>
        public LabelList(List<Label> labels) { Labels = labels; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get labels starting at index <paramref name="start"/> ending at index <paramref name="end"/> exclusively
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="end">Ending index exclusively</param>
        /// <returns>A new <see cref="LabelList"/> containing deep copy of the <see cref="Label"/>s within given range</returns>
        public LabelList GetRange(int start, int end)
        {
            Label[] temp = new Label[end - start];
            for (int i = start; i < end; i++)
            {
                temp[i - start] = _labels[i].Copy();
            }
            return new LabelList(new List<Label>(temp));
        }

        /// <summary>
        /// Get the index of which label is spanning over the given index <paramref name="ind"/>
        /// </summary>
        /// <param name="ind">Index to use</param>
        /// <returns>Index of <see cref="Label"/> in <see cref="LabelList.GetLabels()"/>, which spans over given index <paramref name="ind"/></returns>
        public int GetLabelIndexAtSpan(int ind)
        {
            if (TotalSpan < ind)
            {
                return -1;
            }
            if (Length == 1)
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

        /// <summary>
        /// Get a deep copy of the <see cref="Label"/>s in this instance in a new <see cref="LabelList"/>
        /// </summary>
        /// <returns>A new <see cref="LabelList"/></returns>
        public LabelList Copy()
        {
            Label[] temp = new Label[Length];
            for (int i = 0; i < Length; i++)
            {
                temp[i] = _labels[i].Copy();
            }
            return new LabelList(new List<Label>(temp));
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return Length + ", total_span(" + TotalSpan + ")";
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
