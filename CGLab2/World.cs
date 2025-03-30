
public class World
{
    private List<Entity> _entities = new List<Entity>();

    public Camera Camera { get; } = new Camera();
    public Transform CameraTransform = new Transform();

    public IReadOnlyList<Entity> Entities => _entities;

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
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }
}