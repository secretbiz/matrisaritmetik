using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Core.Models
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class ConstantsEnum : IEnumerable
    {
        #region Fields
        private readonly List<string> _keys = new List<string>();

        private readonly List<dynamic> _vals = new List<dynamic>();

        private readonly List<string> _desc = new List<string>();

        private bool disposedValue;

        #endregion

        public ConstantsEnum()
        {
        }

        #region Public Properties
        public ICollection<string> Keys => _keys;

        public ICollection<dynamic> Values => _vals;

        public ICollection<string> Descriptions => _desc;

        public int Count => _keys.Count;

        public bool IsReadOnly => false;

        #endregion

        #region Key Indexing
        public dynamic this[string key]
        {
            get
            {
                if (_keys.Contains(key))
                {
                    return _vals[_keys.IndexOf(key)];
                }
                else
                {
                    throw new Exception();
                }
            }
            set
            {
                if (_keys.Contains(key))
                {
                    _vals[_keys.IndexOf(key)] = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        #endregion

        #region Dictionary Methods
        public void Add(string key, dynamic value)
        {
            if (_keys.Contains(key))
            {
                throw new Exception();
            }
            else
            {
                _keys.Add(key);
                _vals.Add(value);
                _desc.Add("");
            }
        }

        public void Add(KeyValuePair<string, dynamic> item)
        {
            if (_keys.Contains(item.Key))
            {
                throw new Exception();
            }
            else
            {
                _keys.Add(item.Key);
                _vals.Add(item.Value);
                _desc.Add("");
            }
        }

        public void Clear()
        {
            _keys.Clear();
            _vals.Clear();
            _desc.Clear();
        }

        public bool Contains(KeyValuePair<string, dynamic> item)
        {
            return _keys.Contains(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return _keys.Contains(key);
        }

        public void CopyTo(KeyValuePair<string, dynamic>[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < _keys.Count)
            {
                throw new Exception();
            }

            if (arrayIndex < 0 || arrayIndex >= array.Length)
            {
                throw new Exception();
            }

            int count = Math.Min(array.Length - arrayIndex, _keys.Count);
            for (int i = 0; i < count; i++)
            {
                array[i + arrayIndex] = new KeyValuePair<string, dynamic>(_keys[i], _vals[i]);
            }
        }

        public IEnumerator<KeyValuePair<string, dynamic>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<string, dynamic>(_keys[i], _vals[i]);
            }
        }

        public bool Remove(string key)
        {
            if (!_keys.Contains(key))
            {
                return false;
            }
            else
            {
                _vals.RemoveAt(_keys.IndexOf(key));
                _desc.RemoveAt(_keys.IndexOf(key));
                _keys.Remove(key);
                return true;
            }
        }

        public bool Remove(KeyValuePair<string, dynamic> item)
        {
            if (!_keys.Contains(item.Key))
            {
                return false;
            }
            else
            {
                _vals.RemoveAt(_keys.IndexOf(item.Key));
                _desc.RemoveAt(_keys.IndexOf(item.Key));
                _keys.Remove(item.Key);
                return true;
            }
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out dynamic value)
        {
            if (!_keys.Contains(key))
            {
                value = default;
                return false;
            }
            else
            {
                value = _vals[_keys.IndexOf(key)];
                return true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Custom Methods
        public void Add(string key, dynamic value, string description)
        {
            if (_keys.Contains(key))
            {
                throw new Exception();
            }
            else
            {
                _keys.Add(key);
                _vals.Add(value);
                _desc.Add(description);
            }
        }

        public string GetDescription(string key)
        {
            if (!_keys.Contains(key))
            {
                throw new Exception();
            }
            else
            {
                return _desc[_keys.IndexOf(key)];
            }
        }

        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            if (Count == 0)
            {
                return "Count=0";
            }
            return "Count=" + Count.ToString() + " :" + GetEnumerator().Current.ToString();
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _keys.Clear();
                    _vals.Clear();
                    _desc.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ConstantsEnum()
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

    public static class Constants
    {
        private static readonly ConstantsEnum _dict = new ConstantsEnum()
        {
            { "null", new None() , "Boş değer(null). Fonksiyonlarda argüman olarak kullanabilir." },
            { "e", (float)2.7182818 , "Euler sayısı(2.7182818), doğal logaritma tabanı." },
            { "pi", (float)3.1415926 , "Pi(3.1415926), bir çemberin çevresinin yarıçapına oranı." },
            { "tau", (float)6.2831853 , "Tau(6.2831853), bir çemberin çevresinin çapına oranı." },
            { "sq2", (float)1.4142135 , "2'nin karekökü(1.4142135)" },
            { "sq3", (float)1.7320508 , "3'ün karekökü(1.7320508)" },
            { "sq5", (float)2.2360679 , "5'in karekökü(2.2360679)" },
            { "gold", (float)1.6180339 , "Altın oran(1.6180339). (a+b)/a == a/b eşitliğinin a > b > 0 koşulu altındaki sonucu" },
        };

        public static ConstantsEnum Dict => _dict;

        /* // English
        private static readonly ConstantsDict Dict = new ConstantsDict()
        {
            { "null", new None() , "No value. Can be used as an argument." },
            { "e", (float)2.7182818 , "Euler's number, the base of natural logarithms." },
            { "pi", (float)3.1415926 , "Pi, ratio of a circle's circumference to it's diameter." },
            { "tau", (float)6.2831853 , "Tau, ratio of a circle's circumference to it's radius.\nEquals to 2*pi" },
            { "sq2", (float)1.4142135 , "Square root of 2." },
            { "sq3", (float)1.7320508 , "Square root of 3." },
            { "sq5", (float)2.2360679 , "Square root of 5." },
            { "golden", (float)1.6180339 , "Golden ratio. Equals to (a+b)/a == a/b where a > b > 0." },
        };
        */

        public static bool Contains(string key)
        {
            return _dict.ContainsKey(key);
        }

        public static object Get(string key)
        {
            if (Contains(key))
            {
                return _dict[key];
            }
            else
            {
                throw new Exception();
            }
        }

        public static string Description(string key)
        {
            if (Contains(key))
            {
                return _dict.GetDescription(key);
            }
            else
            {
                throw new Exception();
            }
        }
    };
}
