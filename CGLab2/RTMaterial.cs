using OpenTK.Mathematics;
using System.Runtime.InteropServices;

public class RTMaterial : Material
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Material
    {
        public Vector4 Color;
    }
    public struct Sphere
    {
        public Vector3 Position;
        public float Radius;
        public Material Material;
    }

    private Camera _camera;
    private SSBO<Sphere> _spheres;

    public RTMaterial(Camera camera, SSBO<Sphere> spheres)
    {
        _camera = camera;
        _spheres = spheres;
        Shader = Game.Instance.Assets.GetShader("ShaderRT");
    }

    public override void Use()
    {
        Matrix4 invProj = _camera.GetProjectionMatrix().Inverted();
        Matrix4 invTransform = _camera.Transform.LocalToWorld;

        Shader.Bind();
        _spheres.Bind(0);

        Shader.SetMatrix("_CameraToWorld", ref invTransform);
        Shader.SetMatrix("_CameraInverseProjection", ref invProj);
    }
}
