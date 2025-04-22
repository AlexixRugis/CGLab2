public class FullscreenMaterial : Material
{
    public FullscreenMaterial()
    {
        Shader = Game.Instance.Assets.GetShader("ShaderFullscreen");
    }

    public override void Use()
    {
        Shader.Bind();
    }
}
