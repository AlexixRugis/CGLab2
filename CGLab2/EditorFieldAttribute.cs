[AttributeUsage(AttributeTargets.Field)]
public class EditorFieldAttribute : Attribute
{
    public string? DisplayName { get; }

    public EditorFieldAttribute(string? displayName = null)
    {
        DisplayName = displayName;
    }
}