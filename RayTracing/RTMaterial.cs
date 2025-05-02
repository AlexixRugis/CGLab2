using OpenTK.Mathematics;
using System.Runtime.InteropServices;

public class RTMaterial : Material
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Vertex
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] private float _pad0;
        [FieldOffset(16)] public Vector3 Normal;
        [FieldOffset(28)] private float _pad1;
    };

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Material
    {
        [FieldOffset(0)] public Vector3 Color;
        [FieldOffset(12)] public float Smoothness;
        [FieldOffset(16)] public Vector3 EmissionColor;
        [FieldOffset(28)] public float Metallic;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Sphere
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public float Radius;

        [FieldOffset(16)] public uint MaterialIndex;
        [FieldOffset(20)] private Vector3 _pad0;
    }

    [StructLayout(LayoutKind.Explicit, Size = 144)]
    public struct MeshInfo
    {
        [FieldOffset(0)] public int NodeIndex;
        [FieldOffset(4)] public uint MaterialIndex;
        [FieldOffset(8)] public uint IndexOffset;
        [FieldOffset(12)] public uint VertexOffset;

        [FieldOffset(16)] public Matrix4 Transform;
        [FieldOffset(80)] public Matrix4 InvTransform;
    };

    private Camera _camera;
    private SSBO<Sphere> _spheres;
    private SSBO<Vertex> _vertices;
    private SSBO<uint> _indices;
    private SSBO<MeshInfo> _meshes;
    private SSBO<BVHNode> _nodes;
    private SSBO<Material> _materials;

    public uint Frame { get; set; } = 0;
    public int Bounces { get; set; } = 4;
    public int RaysPerPixel { get; set; } = 2;
    public Texture PrevFrame { get; set; }
    public CubemapTexture Cubemap { get; set; }
    
    public RTMaterial(Camera camera, SSBO<Sphere> spheres, 
        SSBO<Vertex> vertices, 
        SSBO<uint> indices,
        SSBO<MeshInfo> meshes,
        SSBO<BVHNode> nodes,
        SSBO<Material> materials)
    {
        _camera = camera;
        _spheres = spheres;
        _vertices = vertices;
        _indices = indices;
        _meshes = meshes;
        _nodes = nodes;
        _materials = materials;
        Shader = Game.Instance.Assets.GetShader("ShaderRT");
    }

    public override void Use()
    {
        Matrix4 invProj = _camera.GetProjectionMatrix().Inverted();
        Matrix4 invTransform = _camera.Transform.LocalToWorld;
        Vector2 screenSize = Game.Instance.ClientSize;

        Shader.Bind();
        _spheres.Bind(0);
        _vertices.Bind(1);
        _indices.Bind(2);
        _meshes.Bind(3);
        _nodes.Bind(4);
        _materials.Bind(5);

        PrevFrame.Bind(OpenTK.Graphics.OpenGL.TextureUnit.Texture0);
        Cubemap.Bind(OpenTK.Graphics.OpenGL.TextureUnit.Texture1);
        Shader.SetInt("prevFrame", 0);
        Shader.SetInt("_Skybox", 1);
        Shader.SetInt("_SpheresCount", _spheres.Count);
        Shader.SetInt("_MeshesCount", _meshes.Count);
        Shader.SetUInt("_Frame", Frame);
        Shader.SetInt("_Bounces", Bounces);
        Shader.SetInt("_RaysPerPixel", RaysPerPixel);
        Shader.SetVector2("_ScreenSize", screenSize);
        Shader.SetMatrix("_CameraToWorld", ref invTransform);
        Shader.SetMatrix("_CameraInverseProjection", ref invProj);
    }
}
