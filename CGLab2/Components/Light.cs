using OpenTK.Mathematics;

public class Light : Component
{
    public Color4 Color { get; set; } = Color4.White;

    public override Component Clone()
    {
        return new Light() { Color = Color };
    }
}