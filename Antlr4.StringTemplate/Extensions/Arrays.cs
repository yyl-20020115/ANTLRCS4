namespace Antlr4.StringTemplate.Extensions;

using System;

public static class Arrays
{
    public static int GetArrayHashCode<T>(this T[] array)
    {
        if (array == null) return 0;
        var result = 1;
        foreach (var element in array)
            result = 31 * result + element.GetHashCode();
        return result;
    }
    public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Func<TInput, TOutput> transform)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (transform == null)
            throw new ArgumentNullException(nameof(transform));

        var result = new TOutput[array.Length];
        for (int i = 0; i < array.Length; i++)
            result[i] = transform(array[i]);

        return result;
    }
}
