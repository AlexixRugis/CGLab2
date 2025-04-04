using OpenTK.Graphics.OpenGL;
using System.Drawing;

public class UnlitTexturedMaterial : Material
{
    public Color Color = Color.White;
    public Texture Texture;

    public UnlitTexturedMaterial(Texture texture)
    {
        Shader = Game.Instance.Assets.GetShader("ShaderTexUnlit");
        Texture = texture;
    }   

    public override void Use()
    {
        Shader.Bind();
        Shader.SetVector4("Color", new OpenTK.Mathematics.Vector4(Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f, Color.A / 255.0f));
        Texture.Bind(TextureUnit.Texture0);
    }
}
