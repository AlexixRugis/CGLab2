public abstract class Component
{
    public Entity Entity { get; set; }
    public Transform Transform => Entity.Transform;

    public virtual void OnStart() { }
    public virtual void OnDestroy() { }
}