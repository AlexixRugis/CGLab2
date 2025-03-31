public class Entity
{
    static ulong NextId = 0;
    public ulong Id { get; }
    public bool Destroyed { get; private set; }

    public string Name;

    public Transform Transform = new Transform();

    private List<Component> _components = new();

    public IEnumerable<Component> Components => _components;

    public Entity(string name)
    {
        Id = NextId++;
        Name = name;
        Destroyed = false;
    }

    public void Destroy()
    {
        Destroyed = true;
    }

    public void AddComponent(Component component)
    {
        if (component == null) throw new ArgumentNullException(nameof(component));
        if (component.Entity != null | _components.Contains(component))
            throw new ArgumentException("Component already added to entity");

        _components.Add(component);
        component.Entity = this;
    }

    public T? GetComponent<T>() where T : Component
    {
        foreach (var component in Components)
        {
            T? castedComponent = component as T;
            if (castedComponent != null) return castedComponent;
        }

        return null;
    }
}