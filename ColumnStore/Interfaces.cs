using System.Collections.Generic;

namespace ColumnStore
{
    public interface IColumnStoreTyped
    {
        /// <summary> Read column data with type V from container. Return empty dictionary if column or no data found. </summary>
        /// <param name="columnName">case insensitive column name</param>
        /// <exception cref="ArgumentException"></exception>
        Dictionary<CDT, V> Read<V>(CDT from, CDT to, string columnName);

        /// <summary> Write data for column to container</summary>
        /// <param name="columnName">case insensitive column name</param>
        /// <exception cref="ArgumentException"></exception>
        void Write<V>(string columnName, Dictionary<CDT, V> values);
    }

    public interface IColumnStoreEntity
    {
        /// <summary> Read entities with all properties from container. Return empty dictionary if no data found for period </summary>
        /// <exception cref="ArgumentException"></exception>
        Dictionary<CDT, E> Read<E>(CDT from, CDT to) where E : class, new();

        /// <summary> Write entities as column-based to container. Property of entity used as column name</summary>
        /// <exception cref="ArgumentException"></exception>
        void Write<E>(Dictionary<CDT, E> entities) where E : class;
    }

    public interface IColumnStoreUntyped
    {
        /// <summary> Read column(s) data from container. Return only founded data for existing column. </summary>
        /// <param name="columnNames">case insensitive</param>
        /// <exception cref="ArgumentException"></exception>
        Dictionary<string, UntypedColumn> Read(CDT from, CDT to, params string[] columnNames);

        /// <summary> Write data to column data </summary>
        /// <param name="columnName">case insensitive</param>
        /// <exception cref="ArgumentException"></exception>
        void Write(string columnName, UntypedColumn newData);

        /// <summary> Write data to multiple columns data </summary>
        /// <param name="newData">keys must be case insensitive</param>
        void Write(Dictionary<string, UntypedColumn> newData);
    }
}