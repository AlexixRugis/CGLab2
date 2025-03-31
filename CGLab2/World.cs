public class World
{
    private List<Entity> _entities = new List<Entity>();
    private List<IRenderable> _renderers = new List<IRenderable>();
    private List<IUpdatable> _updates = new List<IUpdatable>();

    public Camera CurrentCamera { get; set; }
    public bool Started { get; private set; } = false;

    public IReadOnlyList<Entity> Entities => _entities;
    public IReadOnlyList<IRenderable> Renderers => _renderers;
    public IReadOnlyList<IUpdatable> Updatables => _updates;

    public Entity? FindEntity(ulong id)
    {
        return _entities.Find(x => x.Id == id);
    }

    public Entity? FindEntity(string name)
    {
        return _entities.Find(x => x.Name == name);
    }

    public void AddEntity(Entity entity)
    {
        if (_entities.Contains(entity)) return;
        _entities.Add(entity);

        foreach (var c in entity.Components)
        {
            if (Started) c.OnStart();

            IRenderable? r = c as IRenderable;
            if (r != null)
            {
                _renderers.Add(r);
            }
            IUpdatable? u =  c as IUpdatable;
            if (u != null)
            {
                _updates.Add(u);
            }
        }
    }

    public void RemoveEntity(Entity entity)
    {
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

    public void ProcessDestroy()
    {
        List<Entity> toDestroy = new List<Entity>();

        foreach (var e in _entities)
        {
            if (e.Destroyed) toDestroy.Add(e);
        }

        foreach (var e in toDestroy)
        {
            RemoveEntity(e);
        }
    }

    public void OnStart()
    {
        if (Started) return;

        Started = true;

        foreach (var e in _entities)
        {
            foreach (var c in e.Components)
            {
                c.OnStart();
            }
        }
    }

    public void DestroyAll()
    {
        foreach(var e in _entities)
        {
            e.Destroy();
        }

        ProcessDestroy();
    }
}