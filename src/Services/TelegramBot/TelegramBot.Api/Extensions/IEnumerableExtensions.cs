using System.Collections.Generic;
using System;

namespace TelegramBot.Api.Extensions;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        foreach (T item in enumeration)
        {
            action(item);
        }
    }
}