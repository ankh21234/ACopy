﻿using System;
using System.Globalization;

namespace ADatabase.SqlServer.Columns
{
    public class SqlServerFloatColumn : SqlServerInt32Column
    {
        public SqlServerFloatColumn(string name, bool isNullable, string def)
            : base(name, isNullable, def)
        {
            Type = ColumnTypeName.Float;
        }

        public override string TypeToString()
        {
            return "float";
        }

        public override string ToString(object value)
        {
            return Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);
        }

        public override object ToInternalType(string value)
        {
            if (value == null)
            {
                return null;
            }
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        public override Type GetDotNetType()
        {
            return typeof(double);
        }
    }
}