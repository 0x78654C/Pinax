﻿namespace Pinax;

public static class ExtensionMethods
{
    public static bool IsNotNullEmptyOrWhiteSpace(this string text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }

    public static bool Matches(this string text, string comparisonText)
    {
        if (text == null || comparisonText == null)
        {
            return false;
        }

        return text.Equals(comparisonText, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool None<T>(this IEnumerable<T> elements, Func<T, bool> func = null)
    {
        return func == null
            ? !elements.Any()
            : !elements.Any(func.Invoke);
    }
}