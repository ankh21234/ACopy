﻿using System;
using System.Collections.Generic;
using ADatabase.Exceptions;
using ADatabase.SqlServer.Columns;

namespace ADatabase.SqlServer
{
    public class SqlServerColumnFactory : IColumnFactory
    {
        public IColumn CreateInstance(ColumnTypeName type, string name, int length, int prec, int scale, bool isNullable, bool isIdentity, string def, string collation)
        {
            switch (type)
            {
                case ColumnTypeName.BinaryDouble:
                    return new SqlServerFloatColumn(name, 53, isNullable, def);
                case ColumnTypeName.BinaryFloat:
                    return new SqlServerRealColumn(name, isNullable, def);
                case ColumnTypeName.Blob:
                    return new SqlServerVarBinaryColumn(name, length, isNullable, def);
                case ColumnTypeName.Bool:
                    return new SqlServerBitColumn(name, isNullable, def);
                case ColumnTypeName.Char:
                    return new SqlServerCharColumn(name, length, isNullable, def, collation);
                case ColumnTypeName.Date:
                    return new SqlServerDateColumn(name, isNullable, def);
                case ColumnTypeName.DateTime:
                    return new SqlServerDatetimeColumn(name, isNullable, def);
                case ColumnTypeName.DateTime2:
                    return new SqlServerDatetime2Column(name, scale, isNullable, def);
                case ColumnTypeName.Dec:
                    return new SqlServerDecColumn(name, prec, scale, isNullable, isIdentity, def);
                case ColumnTypeName.Float:
                    return new SqlServerFloatColumn(name, prec, isNullable, def);
                case ColumnTypeName.Guid:
                    return new SqlServerGuidColumn(name, isNullable, def);
                case ColumnTypeName.Int:
                    return new SqlServerIntColumn(name, isNullable, isIdentity, def);
                case ColumnTypeName.Int16:
                    return new SqlServerSmallIntColumn(name, isNullable, isIdentity, def);
                case ColumnTypeName.Int64:
                    return new SqlServerBigIntColumn(name, isNullable, isIdentity, def);
                case ColumnTypeName.Int8:
                    return new SqlServerTinyIntColumn(name, isNullable, isIdentity, def);
                case ColumnTypeName.LongText:
                    return new SqlServerVarcharColumn(name, -1, isNullable, def, collation);
                case ColumnTypeName.NChar:
                    return new SqlServerNCharColumn(name, length, isNullable, def, collation);
                case ColumnTypeName.NLongText:
                    return new SqlServerNVarcharColumn(name, -1, isNullable, def, collation);
                case ColumnTypeName.NOldText:
                    return new SqlServerNTextColumn(name, isNullable, def, collation);
                case ColumnTypeName.NVarchar:
                    return new SqlServerNVarcharColumn(name, length, isNullable, def, collation);
                case ColumnTypeName.OldBlob:
                    return new SqlServerImageColumn(name, isNullable, def);
                case ColumnTypeName.OldText:
                    return new SqlServerTextColumn(name, isNullable, def, collation);
                case ColumnTypeName.Money:
                    return new SqlServerMoneyColumn(name, isNullable, def);
                case ColumnTypeName.Raw:
                    return new SqlServerBinaryColumn(name, length, isNullable, def);
                case ColumnTypeName.SmallDateTime:
                    return new SqlServerSmallDateTimeColumn(name, isNullable, def);
                case ColumnTypeName.SmallMoney:
                    return new SqlServerSmallMoneyColumn(name, isNullable, def);
                case ColumnTypeName.Time:
                    return new SqlServerTimeColumn(name, isNullable, def);
                case ColumnTypeName.Timestamp:
                    return new SqlServerTimestampColumn(name, isNullable, def);
                case ColumnTypeName.Varchar:
                    return new SqlServerVarcharColumn(name, length, isNullable, def, collation);
                case ColumnTypeName.VarRaw:
                    return new SqlServerVarBinaryColumn(name, length, isNullable, def);
            }

            throw new AColumnTypeException($"Illegal type: {type}");
        }

        public IColumn CreateInstance(ColumnTypeName type, string name, int length, bool isNullable, string def, string collation)
        {
            return CreateInstance(type, name, length, 0, 0, isNullable, false, def, collation);
        }

        public IColumn CreateInstance(ColumnTypeName type, string name, bool isNullable, string def)
        {
            return CreateInstance(type, name, 0, 0, 0, isNullable, false, def, "");
        }

        public IColumn CreateInstance(ColumnTypeName columnType, string colName, bool isNullable, string def, Dictionary<string, object> details)
        {
            int length = 0;
            int prec = 0;
            int scale = 0;
            string collation = "";
            bool isIdentity = false;

            if (details.ContainsKey("Length"))
            {
                length = Convert.ToInt32(details["Length"]);
            }
            if (details.ContainsKey("Prec"))
            {
                prec = Convert.ToInt32(details["Prec"]);
            }
            if (details.ContainsKey("Scale"))
            {
                scale = Convert.ToInt32(details["Scale"]);
            }
            if (details.ContainsKey("Collation"))
            {
                collation = details["Collation"].ToString();
            }
            if (details.ContainsKey("Identity"))
            {
                isIdentity = Convert.ToBoolean(details["Identity"]);
            }

            return CreateInstance(columnType, colName, length, prec, scale, isNullable, isIdentity, def, collation);
        }
    }
}