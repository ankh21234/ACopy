﻿using System;
using System.Collections.Generic;

namespace ADatabase
{
    public interface IColumn
    {
        string Name { get; set; }
        ColumnType Type { get; set; }
        Dictionary<string, object> Details { get; }
        bool IsNullable { get; set; }
        string Default { get; set; }
        string TypeToString();
        string GetColumnDefinition();
        string ToString(object value);
        object ToInternalType(string value);
        Type GetDotNetType();
    }
}