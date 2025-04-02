public class Entity
{
    static ulong NextId = 1;
    public ulong Id { get; }
    public bool Destroyed { get; private set; }

    public string Name;

    public Transform Transform { get; }

    private List<Component> _components = new();

    public IEnumerable<Component> Components => _components;

    public Entity(string name)
    {
        Id = NextId++;
        Name = name;
        Destroyed = false;

        Transform = new Transform(this);
    }

    public void Destroy()
    {
        Destroyed = true;

        foreach (var c in Transform.Children)
        {
            c.Entity.Destroy();
        }
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