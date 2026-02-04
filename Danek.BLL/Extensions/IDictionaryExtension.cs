namespace Danek.BLL.Extesions;

public static class IDictionaryExtension
{
    public static TK GetOrKey<TK>(this IDictionary<TK, TK> dictionary, TK key)
    {
        return dictionary.GetOrDefault(key, key);
    }

    public static TV GetOrNull<TK, TV>(this IDictionary<TK, TV> dictionary, TK key) where TV : class
    {
        return dictionary.GetOrDefault(key, null);
    }

    public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV defaultValue)
    {
        return dictionary.GetOrAction(key, () => defaultValue);
    }

    public static TV GetOrThrow<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, string errorNumber)
    {
        return dictionary.GetOrAction(key, () => throw new ArgumentException($"{errorNumber} Элемент '{key}' не найден"));
    }

    public static TV GetOrAction<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> action)
    {
        return key.IsNotNull()
            ? dictionary.TryGetValue(key, out var value)
                ? value
                : action()
            : action();
    }

    public static (TK Key, int Count)? MaxCountKey<TK>(this IDictionary<TK, int> dict)
    {
        var count = 0;
        TK key = default;
        foreach (var (k, v) in dict)
            if (count < v)
            {
                count = v;
                key = k;
            }

        return count > 1
            ? ((TK Key, int Count)?)(key, count)
            : null;
    }

    public static void AddOrInc<TK>(this IDictionary<TK, int> dict, TK key)
    {
        if (dict.ContainsKey(key))
            dict[key] += 1;
        else
            dict[key] = 1;
    }

    public static void AddOrAppend<TK, TV>(this IDictionary<TK, ICollection<TV>> dict, TK key, TV value)
    {
        if (dict.ContainsKey(key))
            dict[key].Add(value);
        else
            dict[key] = new List<TV>
            {
                value,
            };
    }

    public static void AddOrAppend<TK, TV>(this IDictionary<TK, ISet<TV>> dict, TK key, TV value)
    {
        if (dict.ContainsKey(key))
            dict[key].Add(value);
        else
            dict[key] = new HashSet<TV>
            {
                value,
            };
    }

    public static void AddOrAppend<TK0, TD, TK1, TV>(this IDictionary<TK0, TD> dict, TK0 key0, TK1 key1, TV value)
        where TD : IDictionary<TK1, TV>, new()
    {
        if (dict.ContainsKey(key0))
            dict[key0].AddOrReplace(key1, value);
        else
            dict[key0] = new TD
            {
                {key1, value }
            };
    }

    public static void AddOrReplace<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV value)
    {
        if (dict.ContainsKey(key))
            dict[key] = value;
        else
            dict.Add(key, value);
    }

    public static void AddOrIgnore<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV value)
    {
        if (dict.ContainsKey(key))
            return;

        dict.Add(key, value);
    }
}
