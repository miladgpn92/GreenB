using Azure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public class SearchUtility
    {

 
        public static IQueryable<T> SortData<T>(IQueryable<T> data, string sortField, bool ascending)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, sortField);
            var lambda = Expression.Lambda(property, parameter);
            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var orderByMethod = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName && method.IsGenericMethodDefinition && method.GetParameters().Length == 2);
            var genericOrderByMethod = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            return (IQueryable<T>)genericOrderByMethod.Invoke(null, new object[] { data, lambda });
        }

 
    
    }

    public static class ListExtensions
    {
        public static IQueryable<T> DynamicFilter<T>(this IQueryable<T> source, List<FilterCriteria> filters , bool IsAnd=false)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body = null;
            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperties()
                     .FirstOrDefault(p => string.Equals(p.Name, filter.Field, StringComparison.OrdinalIgnoreCase));
                if (property == null)
                {
                    throw new ArgumentException($"Field {filter.Field} not found on type {typeof(T).FullName}");
                }
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var constantValue = filter.Value;
                var constantType = property.PropertyType;

                if (constantValue != null && constantType != typeof(string) && constantType != typeof(object))
                {
                    // Convert integer filter values to the correct type
                    if (IsIntegerType(constantType))
                    {
                        constantValue = Convert.ChangeType(constantValue, constantType);
                    }
                    else if (constantType == typeof(DateTime))
                    {
                      
                        constantValue = DateTime.Parse(constantValue.ToString());
                    }
                    else
                    {
                        var converter = System.ComponentModel.TypeDescriptor.GetConverter(constantType);
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            constantValue = converter.ConvertFromInvariantString(constantValue.ToString());
                        }
                    }
                }

                if (constantValue != null && constantValue.GetType() == typeof(JsonElement))
                {
                    constantValue = ((JsonElement)constantValue).GetString();
                }
                else if (constantValue != null && constantType != typeof(string) && constantType != typeof(object))
                {
                    var converter = System.ComponentModel.TypeDescriptor.GetConverter(constantType);
                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                    {
                        constantValue = converter.ConvertFromInvariantString(constantValue.ToString());
                    }
                }
                var constant = Expression.Constant(constantValue, constantType);
                Expression filterExpression;
                switch (filter.Operator)
                {
                    case Operator.Equal:
                        filterExpression = Expression.Equal(propertyAccess, constant);
                        break;
                    case Operator.GreaterThan:
                        filterExpression = Expression.GreaterThan(propertyAccess, constant);
                        break;
                    case Operator.GreaterThanOrEqual:
                        filterExpression = Expression.GreaterThanOrEqual(propertyAccess, constant);
                        break;
                    case Operator.LessThan:
                        filterExpression = Expression.LessThan(propertyAccess, constant);
                        break;
                    case Operator.LessThanOrEqual:
                        filterExpression = Expression.LessThanOrEqual(propertyAccess, constant);
                        break;
                    case Operator.Contains:
                        if (property.PropertyType != typeof(string))
                        {
                            throw new ArgumentException($"Cannot use Contains operator on non-string property {filter.Field}");
                        }
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        filterExpression = Expression.Call(propertyAccess, containsMethod, constant);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported operator {filter.Operator}");
                }
                if (body == null)
                {
                    body = filterExpression;
                }
                else if(IsAnd == true)
                {
                    body = Expression.AndAlso(body, filterExpression);
                }
                else
                {
                    // Combine filter conditions using OR instead of AND
                    body = Expression.OrElse(body, filterExpression);
                }
            }
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return source.Where(lambda);
        }

        private static bool IsIntegerType(Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte);
        }

    }

    public class FilterCriteria
    {
        public string Field { get; set; }
        public object Value { get; set; }
        public Operator Operator { get; set; }
    }

    public enum Operator
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains
    }

    public static class JsonElementExtensions
    {
        public static int? GetInt32(this JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            return element.GetInt32();
        }
    }
}
