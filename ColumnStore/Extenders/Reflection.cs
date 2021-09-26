using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable AssignNullToNotNullAttribute

namespace ColumnStore
{
    static class ReflectionExtenders
    {
        internal static readonly Type[] supportedTypes = new[]
        {
            typeof(bool),
            typeof(byte),
            typeof(int),
            typeof(short),
            typeof(Int64),
            typeof(Guid),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(double),
            typeof(string),
        };


        internal static Dictionary<string, PropertyInfo> GetProps(this Type type) =>
            type.GetProperties()
                .Where(p => p.CanRead && p.CanWrite && supportedTypes.Contains(p.PropertyType))
                .ToDictionary(p => p.Name, StringComparer.InvariantCulture);

        #region getActionSet

        internal static Dictionary<PropertyInfo, object> actionSet = new();
        
        internal static Action<T, V> getActionSet<T, V>(this PropertyInfo property)
        {
            lock (actionSet)
            {
                if (actionSet.TryGetValue(property, out var o))
                    return (Action<T, V>)o;

                var parmInstance = Expression.Parameter(typeof(T));
                var parmValue    = Expression.Parameter(typeof(V));

                var bodyInstance = Expression.Convert(parmInstance, property.DeclaringType);
                var bodyValue    = Expression.Convert(parmValue, property.PropertyType);
                var bodyCall     = Expression.Call(bodyInstance, property.GetSetMethod(), bodyValue);

                o = Expression.Lambda<Action<T, V>>(bodyCall, parmInstance, parmValue).Compile();
                actionSet.Add(property, o);
                return (Action<T, V>)o;
            }
        }

        #endregion

        #region getActionGet

        internal static Dictionary<PropertyInfo, object> actionGet = new();
        
        internal static Func<T, V> getActionGet<T, V>(this PropertyInfo property)
        {
            lock (actionGet)
            {
                if (actionGet.TryGetValue(property, out var o))
                    return (Func<T, V>)o;

                var parmInstance     = Expression.Parameter(typeof(T));
                var bodyToObjectType = Expression.Convert(parmInstance, property.DeclaringType);
                var bodyGetType      = Expression.Property(bodyToObjectType, property.Name);
                var bodyCall         = Expression.Convert(bodyGetType, typeof(V));

                o = Expression.Lambda<Func<T, V>>(bodyCall, parmInstance).Compile();

                actionGet.Add(property, o);
                return (Func<T, V>)o;
            }
        }

        #endregion
    }
}