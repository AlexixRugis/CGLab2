using OpenTK.Mathematics;
using System.Runtime.InteropServices;

public class RTMaterial : Material
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Material
    {
        [FieldOffset(0)] public Vector3 Color;
        [FieldOffset(12)] public float _pad0;

        [FieldOffset(16)] public Vector3 EmissionColor;
        [FieldOffset(28)] public float EmissionStrength;
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Sphere
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public float Radius;

        [FieldOffset(16)] public Material Material;
    }

    private Camera _camera;
    private SSBO<Sphere> _spheres;
    private uint _frame = 0;

    public float AccumFactor { get; set; } = 0.8f;
    public Texture PrevFrame { get; set; }
    
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
        Vector2 screenSize = Game.Instance.ClientSize;

        Shader.Bind();
        _spheres.Bind(0);

        PrevFrame.Bind(OpenTK.Graphics.OpenGL.TextureUnit.Texture0);
        Shader.SetUInt("_Frame", _frame++);
        Shader.SetFloat("_AccumFactor", AccumFactor);
        Shader.SetVector2("_ScreenSize", screenSize);
        Shader.SetMatrix("_CameraToWorld", ref invTransform);
        Shader.SetMatrix("_CameraInverseProjection", ref invProj);
    }
}
