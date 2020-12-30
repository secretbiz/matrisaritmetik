using System.Collections.Generic;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Core
{
    public static class ArrayMethods
    {
        /// <summary>
        /// Return a copy of given list using <typeparamref name="PARSE"/> type to parse, if parse fails, string of the object is used 
        /// </summary>
        /// <param name="lis">List to copy</param>
        /// <returns>Deep copy of the <paramref name="lis"/></returns>
        public static List<object> CopyList<PARSE>(List<object> lis)
        {
            if (lis == null)
            {
                return null;
            }
            if (lis.Count == 0)
            {
                return new List<object>();
            }

            List<object> newlis = new List<object>();
            if (typeof(PARSE) == typeof(float))
            {
                for (int i = 0; i < lis.Count; i++)
                {
                    if (lis[i] is null || lis[i] is None)
                    {
                        newlis.Add(new None());
                    }
                    else if (float.TryParse(lis[i].ToString(), out float newval))
                    {
                        newlis.Add(newval);
                    }
                    else
                    {
                        newlis.Add(lis[i].ToString());
                    }
                }
            }
            else if (typeof(PARSE) == typeof(string))
            {
                for (int i = 0; i < lis.Count; i++)
                {
                    newlis.Add(lis[i].ToString());
                }
            }
            else
            {
                for (int i = 0; i < lis.Count; i++)
                {
                    if (lis[i] is null || lis[i] is None)
                    {
                        newlis.Add(new None());
                    }
                    else
                    {
                        newlis.Add(lis[i].ToString());
                    }
                }
            }

            return newlis;
        }

        /// <summary>
        /// Try to find the max of given list of objects
        /// </summary>
        /// <param name="lis">List of objects</param>
        /// <param name="numberOnly">1 to use only the numbers, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Max value in the list or null if list had values that are not parsable as floats</returns>
        public static float? ArrayMax(List<object> lis,
                                      int numberOnly = 0)
        {
            if (lis == null)
            {
                return null;
            }

            if (lis.Count == 0)
            {
                return null;
            }

            if (lis.Count == 1)
            {
                return float.TryParse((lis[0] ?? string.Empty).ToString(), out float a) ? a : (float?)null;
            }

            if (numberOnly == 1)
            {
                float current = float.MinValue;
                bool compared = false;
                for (int i = 0; i < lis.Count; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            continue;
                        }

                        current = (current > r) ? current : r;
                        compared = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (compared)
                {
                    return current;
                }
                return float.NaN;

            }
            else
            {
                float current = (float.TryParse((lis[0] ?? string.Empty).ToString(), out float res)) ? res : float.NaN;
                if (float.IsNaN(current))
                {
                    return null;
                }

                for (int i = 0; i < lis.Count; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            return null;
                        }

                        current = (current > r) ? current : r;
                    }
                    else
                    {
                        return null;
                    }
                }
                return current;
            }

        }

        /// <summary>
        /// Try to find the min of given list of objects
        /// </summary>
        /// <param name="lis">List of objects</param>
        /// <param name="numberOnly">1 to use only the numbers, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Min value in the list or null if list had values that are not parsable as floats</returns>
        public static float? ArrayMin(List<object> lis,
                                      int numberOnly = 0)
        {
            if (lis == null)
            {
                return null;
            }

            if (lis.Count == 0)
            {
                return null;
            }

            if (lis.Count == 1)
            {
                return float.TryParse((lis[0] ?? string.Empty).ToString(), out float a) ? a : (float?)null;
            }
            if (numberOnly == 1)
            {
                float current = float.MaxValue;
                bool compared = false;
                for (int i = 0; i < lis.Count; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            continue;
                        }

                        current = (current < r) ? current : r;
                        compared = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (compared)
                {
                    return current;
                }
                return float.NaN;

            }
            else
            {
                float current = (float.TryParse((lis[0] ?? string.Empty).ToString(), out float res)) ? res : float.NaN;
                if (float.IsNaN(current))
                {
                    return null;
                }

                for (int i = 0; i < lis.Count; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            return null;
                        }

                        current = (current < r) ? current : r;
                    }
                    else
                    {
                        return null;
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// Sum all values in the given list within given range
        /// </summary>
        /// <param name="lis">List of objects to use</param>
        /// <param name="start">Starting index</param>
        /// <param name="end">Ending index exclusively</param>
        /// <param name="numberOnly">1 to use only the numbers, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Sum of values within given range</returns>
        public static object ArraySum<T>(List<T> lis,
                                         int start,
                                         int end,
                                         int numberOnly = 0)
        {
            if (lis == null)
            {
                return null;
            }

            if (lis.Count == 0)
            {
                return null;
            }

            if (lis.Count == 1)
            {
                return float.TryParse(((dynamic)lis[0] ?? string.Empty).ToString(), out float a) ? start == 0 ? a : (object)null : null;
            }

            if (numberOnly == 1)
            {
                float current = 0;
                bool compared = false;
                for (int i = start; i < end; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            continue;
                        }

                        current += r;
                        compared = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (compared)
                {
                    return current;
                }
                return float.NaN;

            }
            else
            {
                float current = 0;
                if (float.IsNaN(current))
                {
                    return null;
                }

                for (int i = start; i < end; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            return null;
                        }

                        current += r;
                    }
                    else
                    {
                        return null;
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// Multiply all values in the given list within given range
        /// </summary>
        /// <param name="lis">List of objects to use</param>
        /// <param name="start">Starting index</param>
        /// <param name="end">Ending index exclusively</param>
        /// <param name="numberOnly">1 to use only the numbers, 0 to write <see cref="float.NaN"/> when needed</param>
        /// <returns>Multiplication of values within given range</returns>
        public static object ArrayMul<T>(List<T> lis,
                                         int start,
                                         int end,
                                         int numberOnly = 0)
        {
            if (lis == null)
            {
                return null;
            }

            if (lis.Count == 0)
            {
                return null;
            }

            if (lis.Count == 1)
            {
                return float.TryParse(((dynamic)lis[0] ?? string.Empty).ToString(), out float a) ? start == 0 ? a : (object)null : null;
            }

            if (numberOnly == 1)
            {
                float current = 1;
                bool compared = false;
                for (int i = start; i < end; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            continue;
                        }

                        current *= r;
                        compared = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (compared)
                {
                    return current;
                }
                return float.NaN;

            }
            else
            {
                float current = 1;
                if (float.IsNaN(current))
                {
                    return null;
                }

                for (int i = start; i < end; i++)
                {
                    object n = lis[i];
                    if (float.TryParse(n.ToString(), out float r))
                    {
                        if (float.IsNaN(r))
                        {
                            return null;
                        }

                        current *= r;
                    }
                    else
                    {
                        return null;
                    }
                }
                return current;
            }
        }
    }
}
