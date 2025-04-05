using System.Reflection;

public static class FieldCollector
{
    private static Dictionary<Type, List<FieldInfo>> _cache = new Dictionary<Type, List<FieldInfo>>();

    public static List<FieldInfo> GetEditorFields(Type type)
    {
        if (_cache.ContainsKey(type)) return _cache[type];

        var result = new List<FieldInfo>();

        // Получаем все поля
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (Attribute.IsDefined(field, typeof(EditorFieldAttribute)))
                result.Add(field);
        }

        _cache.Add(type, result);

        return result;
    }
}