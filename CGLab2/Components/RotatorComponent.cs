using OpenTK.Mathematics;

public class RotatorComponent : Component, IUpdatable
{
    public float Speed = 1.0f;

    public override Component Clone()
    {
        return new RotatorComponent()
        {
            Speed = Speed,
        };
    }

    public void Update(float deltaTime)
    {
        Entity.Transform.LocalRotation *= Quaternion.FromEulerAngles(0, deltaTime * Speed, 0);
    }
}
