using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using static Property;

public class Property
{
    public class Key
    {
        string name;
        GameObject subject;

        public Key(string name, GameObject subject)
        {
            this.name = name;
            this.subject = subject;
        }
        public Key(string name)
        {
            this.name = name;
            subject = null;
        }

        public override string ToString()
        {
            if(subject != null)
                return "(" + subject.name + ") " + name;
            else
                return "(Global) " + name;
        }

        public bool Equals(Key x)
        {
            if (x.subject == null && subject == null)
                return x.name == name;
            return x.name == name && x.subject == subject;
        }

        public GameObject GetSubject()
        {
            return subject;
        }

        public class EqualityComparer : IEqualityComparer<Key>
        {
            public bool Equals(Key x, Key y)
            {
                if(x.subject == null && y.subject == null)
                    return x.name == y.name;
                return x.name == y.name && x.subject == y.subject;
            }
            public int GetHashCode(Key obj)
            {
                int hash = 0;
                if(obj.subject != null)
                    hash ^= obj.subject.GetHashCode();
                hash ^= obj.name.GetHashCode();
                return hash;
            }
        }
    }
    public class Value
    {
        public object data { get; private set; }
        public Type dataType { get; private set; }

        /// <summary>
        /// This dictates what kind of comparison will be drawn when examining the action. i.e. Greater means that the other value must be greater than
        /// </summary>
        public enum CompareType { EQUAL, GREATER, LESS, GREATER_EQUAL, LESS_EQUAL, NOT_EQUAL }
        public CompareType compareType;

        /// <summary>
        /// This dictates what action to take when applying this action, specifically for numbers
        /// </summary>
        public enum MergeType { ADD, MULTIPLY, SET }
        public MergeType mergeType;

        public object max { get; private set; } = 100;
        public object min { get; private set; } = 0;

        #region Constructors
        public Value(object data)
        {
            this.data = data;
            dataType = this.data.GetType();
            compareType = CompareType.EQUAL;
            mergeType = MergeType.SET;
        }
        public Value(object data, CompareType comp)
        {
            this.data = data;
            dataType = this.data.GetType();
            compareType = comp;
            mergeType = MergeType.SET;
        }
        public Value(object data, MergeType mergeType)
        {
            this.data = data;
            dataType = this.data.GetType();
            compareType = CompareType.EQUAL;
            this.mergeType = mergeType;
        }
        public Value(object data, Value copy)
        {
            this.data = data;
            dataType = this.data.GetType();
            compareType = copy.compareType;
            mergeType = copy.mergeType;
        }
        #endregion

        #region Simple Methods
        public bool Equals(Value other)
        {
            return Equals(data, other.data);
        }
        public override string ToString()
        {
            return "( Data: " + data.ToString() + ", Type: " + dataType.ToString() + ", Compare: " + compareType.ToString() + ", Merge: " + mergeType.ToString() + ")";
        }
        #endregion

        #region Comparison Methods
        public bool UnifyCompare(Value v)
        {
            // this = precondition
            // v = newState

            if (dataType != v.dataType)
                return false;

            if (Type.GetTypeCode(dataType) != TypeCode.Boolean)
            {
                return true;
            }

            switch (Type.GetTypeCode(dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
            }

            if (Type.GetTypeCode(dataType) == TypeCode.Boolean)
            {
                switch (compareType)
                {
                    case CompareType.EQUAL:
                        return this == v;
                    case CompareType.NOT_EQUAL:
                        return this != v;
                    default: return false;
                }
            }
            else
                return true;
        }
        public bool CompareAgainst(Value v)
        {
            if (dataType != v.dataType)
                return false;

            switch (compareType)
            {
                case CompareType.EQUAL:
                    return this == v;
                case CompareType.GREATER:
                    return v > this;
                case CompareType.LESS:
                    return v < this;
                case CompareType.NOT_EQUAL:
                    return this != v;
                case CompareType.GREATER_EQUAL:
                    return v >= this;
                case CompareType.LESS_EQUAL:
                    return v <= this;
                default: return false;
            }
        }
        public bool CompareWith(Value v)
        {
            if (dataType != v.dataType)
                return false;

            switch (compareType)
            {
                case CompareType.EQUAL:
                    return this == v;
                case CompareType.GREATER:
                    return this > v;
                case CompareType.LESS:
                    return this < v;
                case CompareType.NOT_EQUAL:
                    return this != v;
                case CompareType.GREATER_EQUAL:
                    return this >= v;
                case CompareType.LESS_EQUAL:
                    return this <= v;
                default: return false;
            }
        }
        #endregion

        public Value Unify(Value v)
        {
            if (dataType != v.dataType)
                return null;

            switch (mergeType)
            {
                case MergeType.ADD:
                    return v - this;
                case MergeType.MULTIPLY:
                    return v / this;
                case MergeType.SET:
                    return this;
                default: return null;
            }
        }
        public  Value Merge(Value v)
        {
            if (dataType != v.dataType)
                return null;

            switch (mergeType)
            {
                case MergeType.ADD:
                    return this + v;
                case MergeType.MULTIPLY:
                    return this * v;
                case MergeType.SET:
                    return this;
                default: return null;
            }
        }

        #region Custom Operators

        public static bool operator>(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int)left.data > (int)right.data;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (float)left.data > (float)right.data;
                default:
                    throw new NotSupportedException("The data " + left.data.ToString() + " was not supported");
            }
        }
        public static bool operator<(Value left, Value right)
        {
            switch(Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int)left.data < (int)right.data;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (float)left.data < (float)right.data;
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool operator ==(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int)left.data == (int)right.data;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (float)left.data == (float)right.data;
                case TypeCode.Boolean:
                    return (bool)left.data == (bool)right.data;
                default:
                    throw new NotSupportedException();
            }
        }
        public static bool operator !=(Value left, Value right)
        {
            if (left.dataType != right.dataType)
                return false;

            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int)left.data != (int)right.data;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (float)left.data != (float)right.data;
                case TypeCode.Boolean:
                    return (bool)left.data != (bool)right.data;
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool operator >=(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int)left.data >= (int)right.data;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (float)left.data >= (float)right.data;
                default:
                    throw new NotSupportedException();
            }
        }
        public static bool operator <=(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int)left.data <= (int)right.data;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (float)left.data <= (float)right.data;
                default:
                    throw new NotSupportedException();
            }
        }

        public static Value operator +(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return new Value((int)left.data + (int)right.data, left);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return new Value((float)left.data + (float)right.data, left);
                default:
                    throw new NotSupportedException();
            }
        }
        public static Value operator -(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return new Value((int)left.data - (int)right.data, left);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return new Value((float)left.data - (float)right.data, left);
                default:
                    throw new NotSupportedException();
            }
        }

        public static Value operator *(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return new Value((int)left.data * (int)right.data, left);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return new Value((float)left.data * (float)right.data, left);
                default:
                    throw new NotSupportedException();
            }
        }
        public static Value operator /(Value left, Value right)
        {
            switch (Type.GetTypeCode(left.dataType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return new Value((int)left.data / (int)right.data, left);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return new Value((float)left.data / (float)right.data, left);
                default:
                    throw new NotSupportedException();
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(data, ((Value)obj).data);
        }
        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        #endregion
    }
}
