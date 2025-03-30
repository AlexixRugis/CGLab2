
public class Entity
{
    static ulong NextId = 0;
    public ulong Id { get; }

    public string Name;
    public Transform Transform = new Transform();

    public Mesh? Mesh;

    public Entity(string name)
    {
        Id = NextId++;
        Name = name;
    }
}