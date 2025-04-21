using OpenTK.Mathematics;

public abstract class World
{
    private List<Entity> _entities = new List<Entity>();
    private List<Entity> _rootEntities = new List<Entity>();
    private List<IRenderable> _renderers = new List<IRenderable>();
    private List<IUpdatable> _updates = new List<IUpdatable>();

    public Color4 AmbientColor { get; set; } = new Color4(209, 207, 220, 255);

    public Game Game { get; set; }
    public AssetLoader Assets => Game.Assets;
    public Camera CurrentCamera { get; set; }
    public Light Light { get; set; }
    public bool Started { get; private set; } = false;

    public IReadOnlyList<Entity> RootEntites => _rootEntities;
    public IReadOnlyList<IRenderable> Renderers => _renderers;
    public IReadOnlyList<IUpdatable> Updatables => _updates;

    public abstract void LoadResources();
    public abstract void UnloadResources();
    public abstract void LoadEntities();

    public Entity CreateEntity(string name, Entity? parent = null)
    {
        if (parent != null && parent.World != this)
            throw new InvalidOperationException("Can't attach parent from another world.");

        Entity e = new Entity(this, name);
        _entities.Add(e);
        AddRootEntity(e);

        if (parent != null)
        {
            e.Transform.SetParent(parent.Transform);
        }

        return e;
    }

    public Entity? FindEntity(ulong id)
    {
        return _entities.Find(x => x.Id == id);
    }

    public Entity? FindEntity(string name)
    {
        return _entities.Find(x => x.Name == name);
    }

    public void AddRootEntity(Entity entity)
    {
        if (_rootEntities.Contains(entity)) return;
        _rootEntities.Add(entity);
    }

    public void RemoveRootEntity(Entity entity)
    {
        _rootEntities.Remove(entity);
    }

    public void ProcessComponentAdding(Component component)
    {
        if (Started) component.OnStart();

        IRenderable? r = component as IRenderable;
        if (r != null)
        {
            _renderers.Add(r);
        }
        IUpdatable? u = component as IUpdatable;
        if (u != null)
        {
            _updates.Add(u);
        }
    }

    public void OnStart()
    {
        if (Started) return;

        Started = true;

        foreach (var e in _rootEntities)
        {
            foreach (var c in e.Components)
            {
                c.OnStart();
            }
        }
    }

    public void DestroyAll()
    {
        foreach(var e in _rootEntities)
        {
            e.Destroy();
        }

        ProcessDestroy();
    }

    public void ProcessDestroy()
    {
        List<Entity> toDestroy = new List<Entity>();

        foreach (var e in _rootEntities)
        {
            if (e.Destroyed) toDestroy.Add(e);
        }

        foreach (var e in toDestroy)
        {
            RemoveEntity(e);
        }
    }

    private void RemoveEntity(Entity entity)
    {
        if (entity.Transform.Parent != null)
        {
            entity.Transform.SetParent(null);
        }

        _rootEntities.Remove(entity);
        _entities.Remove(entity);

        foreach (var c in entity.Components)
        {
            c.OnDestroy();

            IRenderable? r = c as IRenderable;
            if (r != null)
            {
                _renderers.Remove(r);
            }
            IUpdatable? u = c as IUpdatable;
            if (u != null)
            {
                _updates.Remove(u);
            }
        }
    }
}