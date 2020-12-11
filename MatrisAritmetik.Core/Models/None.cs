using System;
using System.Diagnostics;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Models.Core
{
    /// <summary>
    /// Used as "null" value for <see cref="Token"/>s
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class None
    {
        public None() { }

        public override string ToString()
        {
            return "null";
        }

        #region Operator overloads
#pragma warning disable IDE0060 // Remove unused parameter

        #region Unary
        public static dynamic operator +(None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator -(None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        #endregion

        #region Addition
        public static dynamic operator +(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator +(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator +(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        #endregion

        #region Subtraction
        public static dynamic operator -(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator -(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator -(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        #endregion

        #region Division
        public static dynamic operator /(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator /(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator /(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }

        #endregion

        #region Multiplication
        public static dynamic operator *(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator *(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator *(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }

        #endregion

        #region Modulo
        public static dynamic operator %(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator %(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator %(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }

        #endregion

        #region Exponential
        public static dynamic operator ^(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator ^(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator ^(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }

        #endregion

        #region Equals
        public static dynamic operator ==(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator ==(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator ==(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        #endregion

        #region Not Equals
        public static dynamic operator !=(dynamic val, None none)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator !=(None none, dynamic val)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }
        public static dynamic operator !=(None none, None none2)
        {
            throw new Exception(CompilerMessage.OP_WITH_NULL);
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

#pragma warning restore IDE0060 // Remove unused parameter
        #endregion
    }
}
