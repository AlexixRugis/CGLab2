public abstract class Component
{
    public Entity Entity { get; set; }
    public Transform Transform => Entity.Transform;

    public abstract Component Clone();

    public virtual void OnStart() { }
    public virtual void OnDestroy() { }
}