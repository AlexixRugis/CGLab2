using OpenTK.Mathematics;

public class Transform
{
    private Entity _entity;
    private Transform? _parent;
    private List<Transform> _children;

    private Vector3 _localPosition;
    private Quaternion _localRotation;
    private Vector3 _localScale;
    private bool _dirty;
    private Matrix4 _localToWorld;

    public Vector3 LocalPosition
    {
        get => _localPosition;
        set { _localPosition = value; MarkDirty(); }
    }

    public Quaternion LocalRotation
    {
        get => _localRotation;
        set { _localRotation = value; MarkDirty(); }
    }

    public Vector3 LocalScale
    {
        get => _localScale;
        set { _localScale = value; MarkDirty(); }
    }

    public Matrix4 LocalToWorld {
        get { if (_dirty) RecalculateDirtiness(); return _localToWorld; }
    }

    public Matrix4 WorldToLocal => Matrix4.Invert(LocalToWorld);

    public Vector3 Forward => Matrix3.CreateFromQuaternion(_localRotation).Row2;
    public Vector3 Up => Matrix3.CreateFromQuaternion(_localRotation).Row1;
    public Vector3 Right => Matrix3.CreateFromQuaternion(_localRotation).Row0;

    public Entity Entity => _entity;

    public Transform? Parent => _parent;
    public IReadOnlyList<Transform> Children => _children;

    public Transform(Entity entity)
    {
        _parent = null;
        _children = new List<Transform>();

        _localPosition = Vector3.Zero;
        _localRotation = Quaternion.Identity;
        _localScale = Vector3.One;
        _dirty = true;

        _entity = entity;
    }

    public void SetParent(Transform? parent)
    {
        if (parent == this) throw new ArgumentException("Can't parent to self");
        if (parent == _parent) return;
        if (IsChild(parent)) throw new ArgumentException("Can't parent to child");

        if (_parent != null)
        {
            _parent.RemoveChild(this);
            _parent = null;
        }
        else
        {
            Entity.World?.RemoveRootEntity(Entity);
        }

        if (parent != null)
        {
            _parent = parent;
            _parent.AddChild(this);
        }
        else
        {
            Entity.World?.AddRootEntity(Entity);
        }

        MarkDirty();
    }

    public bool IsChild(Transform child)
    {
        if (child == null) throw new ArgumentNullException();
        if (child == this) return false;

        foreach (var c in Children)
        {
            if (c == child) return true;
            if (c.IsChild(child)) return true;
        }

        return false;
    }

    public void MarkDirty()
    {
        _dirty = true;

        foreach (var child in _children)
        {
            child.MarkDirty();
        }
    }

    private void RecalculateDirtiness()
    {
        _localToWorld = Matrix4.CreateScale(_localScale)
            * Matrix4.CreateFromQuaternion(_localRotation)
            * Matrix4.CreateTranslation(_localPosition);

        if (Parent != null)
            _localToWorld = _localToWorld * Parent.LocalToWorld;

        _dirty = false;
    }

    private void RemoveChild(Transform child)
    {
        _children.Remove(child);
    }

    private void AddChild(Transform child)
    {
        _children.Add(child);
    }
}