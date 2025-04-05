using OpenTK.Mathematics;

public class Light : Component
{
    [field: EditorField] public Color4 Color { get; set; } = Color4.White;

    public override void OnStart()
    {
        base.OnStart();

        if (Entity.World.Light == null)
            Entity.World.Light = this;
    }

    public override Component Clone()
    {
        return new Light() { Color = Color };
    }
}