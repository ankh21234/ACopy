﻿using System;
using ADatabase.Interfaces;

namespace ADatabase
{
    public class IndexColumn: IIndexColumn
    {
        public IndexColumn(string name, string expression)
        {
            if (String.IsNullOrWhiteSpace(expression))
            {
                _name = name.ToLower();
                IsExpression = false;
            }
            else
            {
                _name = "expression";
                IsExpression = true;
                Expression = expression;
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value.ToLower(); }
        }

        public bool IsExpression { get; set; }

        public string Expression { get; set; }
    }
}
