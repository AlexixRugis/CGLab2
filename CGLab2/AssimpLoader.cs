using Assimp;

public class AssimpLoader
{
    private AssetLoader _assetLoader;

    public AssimpLoader(AssetLoader assets)
    {
        _assetLoader = assets;
    }

    public Entity Load(string resourcePath, string texturesDir, float scale = 0.01f, bool keepMeshesOnCpu = false)
    {
        PostProcessSteps steps = PostProcessSteps.JoinIdenticalVertices
            | PostProcessSteps.Triangulate
            | PostProcessSteps.FixInFacingNormals
            | PostProcessSteps.GenerateNormals
            | PostProcessSteps.GenerateUVCoords;
        return Load(resourcePath, texturesDir, steps, scale, keepMeshesOnCpu);
    }

    public Entity Load(string resourcePath, string texturesDir, PostProcessSteps postProcessSteps, float scale = 0.01f, bool keepMeshesOnCpu = false)
    {
        var importer = new AssimpContext();
        var scene = importer.ImportFile(resourcePath, postProcessSteps);

        Entity e = ProcessNode(scene.RootNode, scene, keepMeshesOnCpu);
        e.Transform.LocalScale *= scale;
        return e;
    }

    private Entity ProcessNode(Assimp.Node node, Assimp.Scene scene, bool keepMeshesOnCpu = false)
    {
        Entity e = new Entity(null, node.Name);
        if (node.MeshCount > 0) {
            e.AddComponent(ProcessMeshes(scene, node.MeshIndices, keepMeshesOnCpu));
        }

        Matrix4x4 tr = node.Transform;
        e.Transform.LocalPosition = new OpenTK.Mathematics.Vector3(tr.A4, tr.B4, tr.C4);
        float scaleX = new Vector3D(tr.A1, tr.B1, tr.C1).Length();
        float scaleY = new Vector3D(tr.A2, tr.B2, tr.C2).Length();
        float scaleZ = new Vector3D(tr.A3, tr.B3, tr.C3).Length();
        e.Transform.LocalScale = new OpenTK.Mathematics.Vector3(scaleX, scaleY, scaleZ);

        OpenTK.Mathematics.Matrix3 rot = new OpenTK.Mathematics.Matrix3();
        rot.M11 = tr.A1 / scaleX; rot.M12 = tr.B1 / scaleX; rot.M13 = tr.C1 / scaleX;
        rot.M21 = tr.A2 / scaleY; rot.M22 = tr.B2 / scaleY; rot.M23 = tr.C2 / scaleY;
        rot.M31 = tr.A3 / scaleZ; rot.M32 = tr.B3 / scaleZ; rot.M33 = tr.C3 / scaleZ;
        e.Transform.LocalRotation = OpenTK.Mathematics.Quaternion.FromMatrix(rot);

        foreach (var c in node.Children)
        {
            Entity ce = ProcessNode(c, scene, keepMeshesOnCpu);
            ce.Transform.SetParent(e.Transform);
        }

        return e;
    }

    private StaticMeshComponent ProcessMeshes(Assimp.Scene scene, List<int> meshIndices, bool keepOnCpu = false)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> indices = new List<uint>();
        List<Mesh.SubMeshInfo> subMeshes = new List<Mesh.SubMeshInfo>();
        List<Material> materials = new List<Material>();

        foreach (var ind in meshIndices)
        {
            var aiMesh = scene.Meshes[ind];

            int material = aiMesh.MaterialIndex;

            Color4D col = scene.Materials[material].ColorDiffuse;
            LitTexturedMaterial mat = new LitTexturedMaterial(Game.Instance.Assets.GetTexture("Blank"))
            {
                Color = System.Drawing.Color.FromArgb((int)(col.A * 255.0f), (int)(col.R * 255.0f), (int)(col.G * 255.0f), (int)(col.B * 255.0f))
            };
            materials.Add(mat);

            int vCount = aiMesh.Vertices.Count;
            int iCount = aiMesh.Faces.Count * 3;

            subMeshes.Add(new() { Index = indices.Count, Size = iCount });

            for (int i = 0; i < vCount; i++)
            {
                Vertex v = new Vertex();
                v.Position.X = aiMesh.Vertices[i].X;
                v.Position.Y = aiMesh.Vertices[i].Y;
                v.Position.Z = aiMesh.Vertices[i].Z;
                v.Normal.X = aiMesh.Normals[i].X;
                v.Normal.Y = aiMesh.Normals[i].Y;
                v.Normal.Z = aiMesh.Normals[i].Z;
                v.UV.X = aiMesh.TextureCoordinateChannels[0][i].X;
                v.UV.Y = aiMesh.TextureCoordinateChannels[0][i].Y;
                vertices.Add(v);
            }

            uint startIndex = (uint)indices.Count;
            List<Face> faces = aiMesh.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    indices.Add((uint)faces[i].Indices[j] + startIndex);
                }
            }
        }

        string meshAssetName = $"Mesh_{Guid.NewGuid()}";
        _assetLoader.LoadMesh(meshAssetName, vertices.ToArray(), indices.ToArray(), subMeshes.ToArray(), keepOnCpu);

        return new StaticMeshComponent()
        {
            Mesh = _assetLoader.GetMesh(meshAssetName),
            Materials = materials
        };
    }
}