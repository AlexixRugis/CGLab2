using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

public class LitTexturedMaterial : Material
{
    public Color Color = Color.White;
    public Texture Texture;

    public LitTexturedMaterial(Texture texture)
    {
        Shader = Game.Instance.Assets.GetShader("ShaderTexLit");
        Texture = texture;
    }

    public override void Use()
    {
        World world = Game.Instance.World;
        Color4 ambient = world.AmbientColor;
        Light l = world.Light;

        Shader.Bind();
        Shader.SetVector3("ViewPos", world.CurrentCamera.Transform.LocalPosition);
        Shader.SetVector3("AmbientColor", new Vector3(ambient.R, ambient.G, ambient.B));
        Shader.SetVector3("LightColor", new Vector3(l.Color.R, l.Color.G, l.Color.B));
        Shader.SetVector3("LightPos", l.Transform.LocalPosition);
        Shader.SetVector4("Color", new Vector4(Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f, Color.A / 255.0f));
        Texture.Bind(TextureUnit.Texture0);
    }
}
