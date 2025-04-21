using OpenTK.Mathematics;

public class RTMaterial : Material
{
    private Camera _camera;

    public RTMaterial(Camera camera)
    {
        _camera = camera;
        Shader = Game.Instance.Assets.GetShader("ShaderRT");
    }

    public override void Use()
    {
        Matrix4 invProj = _camera.GetProjectionMatrix().Inverted();
        Matrix4 invTransform = _camera.Transform.LocalToWorld;

        Shader.Bind();
        Shader.SetMatrix("_CameraToWorld", ref invTransform);
        Shader.SetMatrix("_CameraInverseProjection", ref invProj);
    }
}
