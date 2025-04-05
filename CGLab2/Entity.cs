public class Entity
{
    static ulong NextId = 1;
    public ulong Id { get; }
    public bool Destroyed { get; private set; }

    public string Name;

    public World? World { get; }

    public Transform Transform { get; }

    private List<Component> _components = new();

    public IEnumerable<Component> Components => _components;

    public Entity(World? world, string name)
    {
        Id = NextId++;
        Name = name;
        Destroyed = false;

        World = world;
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

        World?.ProcessComponentAdding(component);

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

    public Entity Clone()
    {
        Entity e = Game.Instance.World.CreateEntity(Name);
        e.Transform.LocalPosition = Transform.LocalPosition;
        e.Transform.LocalRotation = Transform.LocalRotation;
        e.Transform.LocalScale = Transform.LocalScale;

        foreach (var ch in Transform.Children)
        {
            Entity chEntity = ch.Entity.Clone();
            chEntity.Transform.SetParent(e.Transform);
        }

        foreach (var c  in Components)
        {
            e.AddComponent(c.Clone());
        }

        return e;
    }

    public Entity? GetChild(string name)
    {
        if (Name == name) return this;

        foreach (var ch in Transform.Children)
        {
            Entity che = ch.Entity;
            if (che.Name == name)
            {
                return che;
            }

            Entity? res = che.GetChild(name);
            if (res != null) return res;
        }

        return null;
    }

    public Entity? GetChild(ulong id)
    {
        if (Id == id) return this;

        foreach (var ch in Transform.Children)
        {
            Entity che = ch.Entity;
            if (che.Id == id)
            {
                return che;
            }

            Entity? res = che.GetChild(id);
            if (res != null) return res;
        }

        return null;
    }
}