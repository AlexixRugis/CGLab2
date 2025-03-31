using OpenTK.Mathematics;

public class RotatorComponent : Component, IUpdatable
{
    public float Speed = 1.0f;
    public void Update(float deltaTime)
    {
        Entity.Transform.Rotation *= Quaternion.FromEulerAngles(0, deltaTime * Speed, 0);
    }
}
