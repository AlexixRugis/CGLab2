using OpenTK.Graphics.OpenGL;

public class UnlitTexturedMaterial : Material
{
    public Texture Texture;

    public UnlitTexturedMaterial(Texture texture)
    {
        Shader = Game.Instance.Assets.GetShader("ShaderTexUnlit");
        Texture = texture;
    }   

    public override void Use()
    {
        Shader.Bind();
        Texture.Bind(TextureUnit.Texture0);
    }
}
