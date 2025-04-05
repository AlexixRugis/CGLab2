public class CubemapMaterial : Material
{
    public CubemapTexture Texture;

    public CubemapMaterial(CubemapTexture texture)
    {
        Shader = Game.Instance.Assets.GetShader("ShaderSkybox");
        Texture = texture;
    }

    public override void Use()
    {
        Shader.Bind();
        Texture.Bind();
    }
}
