using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ColumnStore;

static class ReflectionExtenders
{
    static readonly Dictionary<StoredDataType, ReadWriteBase> readWriteHandlers = new()
                                                                                  {
                                                                                      [StoredDataType.Boolean] = new ReadWriteHandlerBoolean(),

                                                                                      [StoredDataType.Byte]  = new ReadWriteHandlerByte(),
                                                                                      [StoredDataType.SByte] = new ReadWriteHandlerGeneric<sbyte>(),

                                                                                      [StoredDataType.Int]  = new ReadWriteHandlerGenericInt<int>(),
                                                                                      [StoredDataType.UInt] = new ReadWriteHandlerGenericInt<uint>(),

                                                                                      [StoredDataType.Int16]  = new ReadWriteHandlerGenericInt<short>(),
                                                                                      [StoredDataType.UInt16] = new ReadWriteHandlerGenericInt<ushort>(),

                                                                                      [StoredDataType.Int64]  = new ReadWriteHandlerGenericInt<Int64>(),
                                                                                      [StoredDataType.UInt64] = new ReadWriteHandlerGenericInt<UInt64>(),

                                                                                      [StoredDataType.Double]   = new ReadWriteHandlerDouble(),
                                                                                      [StoredDataType.DateTime] = new ReadWriteHandlerDateTime(),
                                                                                      [StoredDataType.Guid]     = new ReadWriteHandlerGuid(),
                                                                                      [StoredDataType.String]   = new ReadWriteHandlerString(),
                                                                                      [StoredDataType.TimeSpan] = new ReadWriteHandlerTimeSpan(),
                                                                                      [StoredDataType.Half]     = new ReadWriteHandlerGeneric<Half>(),
                                                                                      [StoredDataType.Decimal]  = new ReadWriteHandlerGeneric<decimal>(),
                                                                                      [StoredDataType.DateOnly] = new ReadWriteHandlerGeneric<DateOnly>(),
                                                                                      [StoredDataType.TimeOnly] = new ReadWriteHandlerTimeOnly()
                                                                                  };

    internal static ReadWriteBase GetHandler(this StoredDataType type) =>
        readWriteHandlers.TryGetValue(type, out var handler)
            ? handler
            : throw new NotSupportedException(type + ": can't find ReadWrite handler");

    #region TryDetectDataType

    internal static bool TryDetectDataType(Type type, out StoredDataType r)
    {
        if (type == typeof(bool))
        {
            r = StoredDataType.Boolean;
            return true;
        }

        if (type == typeof(byte))
        {
            r = StoredDataType.Byte;
            return true;
        }

        if (type == typeof(sbyte))
        {
            r = StoredDataType.SByte;
            return true;
        }

        if (type == typeof(int))
        {
            r = StoredDataType.Int;
            return true;
        }

        if (type == typeof(uint))
        {
            r = StoredDataType.UInt;
            return true;
        }

        if (type == typeof(short))
        {
            r = StoredDataType.Int16;
            return true;
        }

        if (type == typeof(ushort))
        {
            r = StoredDataType.UInt16;
            return true;
        }

        if (type == typeof(Int64))
        {
            r = StoredDataType.Int64;
            return true;
        }

        if (type == typeof(UInt64))
        {
            r = StoredDataType.UInt64;
            return true;
        }

        if (type == typeof(string))
        {
            r = StoredDataType.String;
            return true;
        }

        if (type == typeof(double))
        {
            r = StoredDataType.Double;
            return true;
        }

        if (type == typeof(Guid))
        {
            r = StoredDataType.Guid;
            return true;
        }

        if (type == typeof(DateTime))
        {
            r = StoredDataType.DateTime;
            return true;
        }

        if (type == typeof(TimeSpan))
        {
            r = StoredDataType.TimeSpan;
            return true;
        }

        if (type == typeof(decimal))
        {
            r = StoredDataType.Decimal;
            return true;
        }

        if (type == typeof(Half))
        {
            r = StoredDataType.Half;
            return true;
        }

        if (type == typeof(DateOnly))
        {
            r = StoredDataType.DateOnly;
            return true;
        }

        if (type == typeof(TimeOnly))
        {
            r = StoredDataType.TimeOnly;
            return true;
        }

        r = 0;
        return false;
    }

    #endregion

    internal static Dictionary<string, CSPropertyInfo> GetProps<E>()
    {
        var r = new Dictionary<string, CSPropertyInfo>(StringComparer.InvariantCulture);
        foreach (var prop in typeof(E)
                            .GetProperties()
                            .Where(p => p is {CanRead: true, CanWrite: true} &&
                                        p.GetCustomAttribute<IgnoreColumnAttribute>() == null))
        {
            if (prop.PropertyType      == typeof(int)) r.Add(prop.Name,      createPropertyInfo<E, int>(StoredDataType.Int, prop));
            else if (prop.PropertyType == typeof(uint)) r.Add(prop.Name,     createPropertyInfo<E, uint>(StoredDataType.UInt, prop));
            else if (prop.PropertyType == typeof(short)) r.Add(prop.Name,    createPropertyInfo<E, short>(StoredDataType.Int16, prop));
            else if (prop.PropertyType == typeof(ushort)) r.Add(prop.Name,   createPropertyInfo<E, ushort>(StoredDataType.UInt16, prop));
            else if (prop.PropertyType == typeof(Int64)) r.Add(prop.Name,    createPropertyInfo<E, Int64>(StoredDataType.Int64, prop));
            else if (prop.PropertyType == typeof(UInt64)) r.Add(prop.Name,   createPropertyInfo<E, UInt64>(StoredDataType.UInt64, prop));
            else if (prop.PropertyType == typeof(byte)) r.Add(prop.Name,     createPropertyInfo<E, byte>(StoredDataType.Byte, prop));
            else if (prop.PropertyType == typeof(sbyte)) r.Add(prop.Name,    createPropertyInfo<E, sbyte>(StoredDataType.SByte, prop));
            else if (prop.PropertyType == typeof(bool)) r.Add(prop.Name,     createPropertyInfo<E, bool>(StoredDataType.Boolean, prop));
            else if (prop.PropertyType == typeof(Guid)) r.Add(prop.Name,     createPropertyInfo<E, Guid>(StoredDataType.Guid, prop));
            else if (prop.PropertyType == typeof(DateTime)) r.Add(prop.Name, createPropertyInfo<E, DateTime>(StoredDataType.DateTime, prop));
            else if (prop.PropertyType == typeof(TimeSpan)) r.Add(prop.Name, createPropertyInfo<E, TimeSpan>(StoredDataType.TimeSpan, prop));
            else if (prop.PropertyType == typeof(double)) r.Add(prop.Name,   createPropertyInfo<E, double>(StoredDataType.Double, prop));
            else if (prop.PropertyType == typeof(string)) r.Add(prop.Name,   createPropertyInfo<E, string>(StoredDataType.String, prop));
            else if (prop.PropertyType == typeof(decimal)) r.Add(prop.Name,  createPropertyInfo<E, decimal>(StoredDataType.Decimal, prop));
            else if (prop.PropertyType == typeof(Half)) r.Add(prop.Name,     createPropertyInfo<E, Half>(StoredDataType.Half, prop));
            else if (prop.PropertyType == typeof(DateOnly)) r.Add(prop.Name, createPropertyInfo<E, DateOnly>(StoredDataType.DateOnly, prop));
            else if (prop.PropertyType == typeof(TimeOnly)) r.Add(prop.Name, createPropertyInfo<E, TimeOnly>(StoredDataType.TimeOnly, prop));
        }

        return r;
    }

    #region createPropertyInfo

    static readonly ReaderWriterLockSlim rwLock = new();

    static readonly Dictionary<PropertyInfo, CSPropertyInfo> propsCache = new();

    static CSPropertyInfo createPropertyInfo<E, V>(StoredDataType dataType, PropertyInfo prop)
    {
        rwLock.EnterUpgradeableReadLock();
        if (propsCache.TryGetValue(prop, out var r))
        {
            rwLock.ExitUpgradeableReadLock();
            return r;
        }

        rwLock.EnterWriteLock();
        if (propsCache.TryGetValue(prop, out r))
        {
            rwLock.ExitWriteLock();
            rwLock.ExitUpgradeableReadLock();
            return r;
        }

        propsCache.Add(prop, r = new CSPropertyInfo(dataType,
                                                    getActionGet<E, V>(prop),
                                                    getActionSet<E, V>(prop),
                                                    getInstanceCreator<E>()));

        rwLock.ExitWriteLock();
        rwLock.ExitUpgradeableReadLock();
        return r;
    }

    static Action<E, V> getActionSet<E, V>(this PropertyInfo property)
    {
        var parmInstance = Expression.Parameter(typeof(E));
        var parmValue    = Expression.Parameter(typeof(V));

        var bodyInstance = Expression.Convert(parmInstance, property.DeclaringType!);
        var bodyValue    = Expression.Convert(parmValue,    property.PropertyType);
        var bodyCall     = Expression.Call(bodyInstance, property.GetSetMethod()!, bodyValue);

        return Expression.Lambda<Action<E, V>>(bodyCall, parmInstance, parmValue).Compile();
    }

    static Func<E, V> getActionGet<E, V>(this PropertyInfo property)
    {
        var parmInstance     = Expression.Parameter(typeof(E));
        var bodyToObjectType = Expression.Convert(parmInstance, property.DeclaringType!);
        var bodyGetType      = Expression.Property(bodyToObjectType, property.Name);
        var bodyCall         = Expression.Convert(bodyGetType, typeof(V));

        return Expression.Lambda<Func<E, V>>(bodyCall, parmInstance).Compile();
    }

    static Func<T> getInstanceCreator<T>()
    {
        var ctor       = Expression.New(typeof(T));
        var memberInit = Expression.MemberInit(ctor);
        return Expression.Lambda<Func<T>>(memberInit).Compile();
    }

    #endregion
}

/// <param name="Getter">Func<E, V></param>
/// <param name="Setter">Action<E, V></param>
internal record CSPropertyInfo(StoredDataType DataType,
                               object         Getter, object Setter,
                               object         InstanceCreator);