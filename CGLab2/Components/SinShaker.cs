using OpenTK.Mathematics;

public class SinShakerComponent : Component, IUpdatable
{
    public float Speed = 1.0f;
    public float Delta = 1.0f;

    private Vector3 _initialPosition;
    private float _time;

    public override Component Clone()
    {
        return new SinShakerComponent()
        {
            Speed = Speed,
            Delta = Delta
        };
    }

    public override void OnStart()
    {
        base.OnStart();

        _initialPosition = Transform.LocalPosition;
        _time = 0.0f;
    }

    public void Update(float deltaTime)
    {
        Entity.Transform.LocalPosition = _initialPosition + Vector3.UnitY * (Delta * MathF.Sin(_time * Speed));
        _time += deltaTime;
    }
}
