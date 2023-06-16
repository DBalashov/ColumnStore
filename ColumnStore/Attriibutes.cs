using System;

namespace ColumnStore;

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreColumnAttribute : Attribute
{
}