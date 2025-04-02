using Assimp;

public class AssimpLoader
{
    public Mesh Load(string resourcePath, string texturesDir)
    {
        PostProcessSteps steps = PostProcessSteps.JoinIdenticalVertices
            | PostProcessSteps.Triangulate
            | PostProcessSteps.FixInFacingNormals
            | PostProcessSteps.GenerateNormals
            | PostProcessSteps.GenerateUVCoords;
        return Load(resourcePath, texturesDir, steps);
    }

    public Mesh Load(string resourcePath, string texturesDir, PostProcessSteps postProcessSteps)
    {
        var importer = new AssimpContext();
        var scene = importer.ImportFile(resourcePath, postProcessSteps);

        return ProcessMeshes(scene);
    }

    private Mesh ProcessMeshes(Assimp.Scene scene)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> indices = new List<uint>();
        List<Mesh.SubMeshInfo> subMeshes = new List<Mesh.SubMeshInfo>();

        foreach (var aiMesh in scene.Meshes)
        {
            int vCount = aiMesh.Vertices.Count;
            int iCount = aiMesh.Faces.Count * 3;

            subMeshes.Add(new() { Index = indices.Count, Size = iCount });

            for (int i = 0; i < vCount; i++)
            {
                Vertex v = new Vertex();
                v.Position.X = aiMesh.Vertices[i].X;
                v.Position.Y = aiMesh.Vertices[i].Y;
                v.Position.Z = aiMesh.Vertices[i].Z;
                v.UV.X = aiMesh.TextureCoordinateChannels[0][i].X;
                v.UV.Y = aiMesh.TextureCoordinateChannels[0][i].Y;
                vertices.Add(v);
            }

            List<Face> faces = aiMesh.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    indices.Add((uint)faces[i].Indices[j]);
                }
            }
        }

        return new Mesh(vertices.ToArray(), indices.ToArray(), subMeshes.ToArray());
    }
}