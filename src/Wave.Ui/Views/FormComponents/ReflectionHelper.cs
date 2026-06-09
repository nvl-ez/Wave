using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Wave.Ui.Views.FormComponents;

public static class ReflectionHelper
{
    public static object? GetValue(object source, string path)
    {
        object? current = source;

        foreach (var segment in SplitPath(path))
        {
            if (current is null)
                return null;

            current = GetSegmentValue(current, segment);
        }

        return current;
    }

    public static void SetValue(object source, string path, object? value)
    {
        var segments = SplitPath(path);

        if (segments.Count == 0)
            return;

        object? current = source;

        for (var i = 0; i < segments.Count - 1; i++)
        {
            if (current is null)
                return;

            current = GetSegmentValue(current, segments[i]);
        }

        if (current is null)
            return;

        SetSegmentValue(current, segments[^1], value);
    }

    private static object? GetSegmentValue(object source, string segment)
    {
        var parsed = ParseSegment(segment);

        object? current = source;

        if (!string.IsNullOrWhiteSpace(parsed.PropertyName))
        {
            var property = current.GetType().GetProperty(parsed.PropertyName);

            if (property is null)
                return null;

            current = property.GetValue(current);
        }

        if (parsed.Key is not null)
        {
            if (current is null)
                return null;

            return GetIndexedValue(current, parsed.Key);
        }

        return current;
    }

    private static void SetSegmentValue(object source, string segment, object? value)
    {
        var parsed = ParseSegment(segment);

        if (parsed.Key is not null)
        {
            object? target = source;

            if (!string.IsNullOrWhiteSpace(parsed.PropertyName))
            {
                var property = source.GetType().GetProperty(parsed.PropertyName);

                if (property is null)
                    return;

                target = property.GetValue(source);
            }

            if (target is null)
                return;

            SetIndexedValue(target, parsed.Key, value);
            return;
        }

        if (string.IsNullOrWhiteSpace(parsed.PropertyName))
            return;

        var finalProperty = source.GetType().GetProperty(parsed.PropertyName);

        if (finalProperty is null || !finalProperty.CanWrite)
            return;

        var convertedValue = ConvertValue(value, finalProperty.PropertyType);
        var currentValue = finalProperty.GetValue(source);

        if (Equals(currentValue, convertedValue))
            return;

        finalProperty.SetValue(source, convertedValue);
    }

    private static object? GetIndexedValue(object source, string key)
    {
        if (source is IDictionary dictionary)
        {
            var keyType = GetDictionaryKeyType(source.GetType()) ?? typeof(string);
            var convertedKey = ConvertValue(key, keyType);

            return dictionary.Contains(convertedKey)
                ? dictionary[convertedKey]
                : null;
        }

        var indexer = GetIndexer(source.GetType());

        if (indexer is null)
            return null;

        var parameterType = indexer.GetIndexParameters()[0].ParameterType;
        var convertedIndexerKey = ConvertValue(key, parameterType);

        return indexer.GetValue(source, new[] { convertedIndexerKey });
    }

    private static void SetIndexedValue(object source, string key, object? value)
    {
        if (source is IDictionary dictionary)
        {
            var keyType = GetDictionaryKeyType(source.GetType()) ?? typeof(string);
            var valueType = GetDictionaryValueType(source.GetType()) ?? typeof(object);

            var convertedKey = ConvertValue(key, keyType);
            var convertedValue = ConvertValue(value, valueType);

            if (dictionary.Contains(convertedKey) && Equals(dictionary[convertedKey], convertedValue))
                return;

            dictionary[convertedKey] = convertedValue;
            return;
        }

        var indexer = GetIndexer(source.GetType());

        if (indexer is null)
            return;

        var parameterType = indexer.GetIndexParameters()[0].ParameterType;
        var convertedIndexerKey = ConvertValue(key, parameterType);
        var convertedValueForIndexer = ConvertValue(value, indexer.PropertyType);

        object? currentValue = null;
        var hasCurrentValue = true;

        try
        {
            currentValue = indexer.GetValue(source, new[] { convertedIndexerKey });
        }
        catch
        {
            hasCurrentValue = false;
        }

        if (hasCurrentValue && Equals(currentValue, convertedValueForIndexer))
            return;

        indexer.SetValue(source, convertedValueForIndexer, new[] { convertedIndexerKey });
    }

    private static PropertyInfo? GetIndexer(Type type)
    {
        foreach (var property in type.GetProperties())
        {
            var parameters = property.GetIndexParameters();

            if (parameters.Length == 1)
                return property;
        }

        return null;
    }

    private static Type? GetDictionaryKeyType(Type type)
    {
        return GetGenericDictionaryInterface(type)?.GetGenericArguments()[0];
    }

    private static Type? GetDictionaryValueType(Type type)
    {
        return GetGenericDictionaryInterface(type)?.GetGenericArguments()[1];
    }

    private static Type? GetGenericDictionaryInterface(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            return type;

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                return interfaceType;
            }
        }

        return null;
    }

    private static List<string> SplitPath(string path)
    {
        var result = new List<string>();
        var current = new StringBuilder();

        var bracketDepth = 0;
        char? quote = null;

        foreach (var c in path)
        {
            if (quote is not null)
            {
                current.Append(c);

                if (c == quote)
                    quote = null;

                continue;
            }

            if (c is '"' or '\'')
            {
                quote = c;
                current.Append(c);
                continue;
            }

            if (c == '[')
            {
                bracketDepth++;
                current.Append(c);
                continue;
            }

            if (c == ']')
            {
                bracketDepth--;
                current.Append(c);
                continue;
            }

            if (c == '.' && bracketDepth == 0)
            {
                result.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(c);
        }

        if (current.Length > 0)
            result.Add(current.ToString());

        return result;
    }

    private static PathSegment ParseSegment(string segment)
    {
        var bracketStart = segment.IndexOf('[');

        if (bracketStart < 0)
            return new PathSegment(segment, null);

        var bracketEnd = segment.LastIndexOf(']');

        if (bracketEnd < bracketStart)
            return new PathSegment(segment, null);

        var propertyName = segment[..bracketStart];

        var rawKey = segment[(bracketStart + 1)..bracketEnd].Trim();

        if ((rawKey.StartsWith("\"") && rawKey.EndsWith("\"")) ||
            (rawKey.StartsWith("'") && rawKey.EndsWith("'")))
        {
            rawKey = rawKey[1..^1];
        }

        return new PathSegment(propertyName, rawKey);
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        var realType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (value is null)
        {
            if (Nullable.GetUnderlyingType(targetType) is not null)
                return null;

            if (!realType.IsValueType)
                return null;

            return Activator.CreateInstance(realType);
        }

        if (realType.IsAssignableFrom(value.GetType()))
            return value;

        if (realType.IsEnum)
            return Enum.Parse(realType, value.ToString()!, ignoreCase: true);

        return Convert.ChangeType(value, realType);
    }

    private readonly record struct PathSegment(string? PropertyName, string? Key);
}
