using OpenTK.Mathematics;

public static class Primitives
{
    private static readonly Vertex[] _quadVertices =
    {
        new Vertex() { Position = new Vector3(0.5f, 0.5f, 0.0f), UV = new Vector2(1.0f,1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, 0.0f), UV = new Vector2(1.0f,0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, 0.0f), UV = new Vector2(0.0f,0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 0.5f, 0.0f), UV = new Vector2(0.0f,1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) }
    };

    private static readonly uint[] _quadIndices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private static readonly Vertex[] _cubeVertices =
    {
        // Front face
        new Vertex() { Position = new Vector3(-0.5f, -0.5f,  0.5f), UV = new Vector2(0.0f, 0.0f), Normal = new Vector3(0.0f, 0.0f,  1.0f) },
        new Vertex() { Position = new Vector3( 0.5f, -0.5f,  0.5f), UV = new Vector2(1.0f, 0.0f), Normal = new Vector3(0.0f, 0.0f,  1.0f) },
        new Vertex() { Position = new Vector3( 0.5f,  0.5f,  0.5f), UV = new Vector2(1.0f, 1.0f), Normal = new Vector3(0.0f, 0.0f,  1.0f) },
        new Vertex() { Position = new Vector3(-0.5f,  0.5f,  0.5f), UV = new Vector2(0.0f, 1.0f), Normal = new Vector3(0.0f, 0.0f,  1.0f) },

        // Back face
        new Vertex() { Position = new Vector3( 0.5f, -0.5f, -0.5f), UV = new Vector2(0.0f, 0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, -0.5f), UV = new Vector2(1.0f, 0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-0.5f,  0.5f, -0.5f), UV = new Vector2(1.0f, 1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3( 0.5f,  0.5f, -0.5f), UV = new Vector2(0.0f, 1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },

        // Left face
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, -0.5f), UV = new Vector2(0.0f, 0.0f), Normal = new Vector3(-1.0f, 0.0f, 0.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f,  0.5f), UV = new Vector2(1.0f, 0.0f), Normal = new Vector3(-1.0f, 0.0f, 0.0f) },
        new Vertex() { Position = new Vector3(-0.5f,  0.5f,  0.5f), UV = new Vector2(1.0f, 1.0f), Normal = new Vector3(-1.0f, 0.0f, 0.0f) },
        new Vertex() { Position = new Vector3(-0.5f,  0.5f, -0.5f), UV = new Vector2(0.0f, 1.0f), Normal = new Vector3(-1.0f, 0.0f, 0.0f) },

        // Right face
        new Vertex() { Position = new Vector3(0.5f, -0.5f,  0.5f), UV = new Vector2(0.0f, 0.0f), Normal = new Vector3(1.0f, 0.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, -0.5f), UV = new Vector2(1.0f, 0.0f), Normal = new Vector3(1.0f, 0.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f,  0.5f, -0.5f), UV = new Vector2(1.0f, 1.0f), Normal = new Vector3(1.0f, 0.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f,  0.5f,  0.5f), UV = new Vector2(0.0f, 1.0f), Normal = new Vector3(1.0f, 0.0f, 0.0f) },

        // Top face
        new Vertex() { Position = new Vector3(-0.5f, 0.5f,  0.5f), UV = new Vector2(0.0f, 0.0f), Normal = new Vector3(0.0f, 1.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f, 0.5f,  0.5f), UV = new Vector2(1.0f, 0.0f), Normal = new Vector3(0.0f, 1.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f, 0.5f, -0.5f), UV = new Vector2(1.0f, 1.0f), Normal = new Vector3(0.0f, 1.0f, 0.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 0.5f, -0.5f), UV = new Vector2(0.0f, 1.0f), Normal = new Vector3(0.0f, 1.0f, 0.0f) },

        // Bottom face
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, -0.5f), UV = new Vector2(0.0f, 0.0f), Normal = new Vector3(0.0f, -1.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, -0.5f), UV = new Vector2(1.0f, 0.0f), Normal = new Vector3(0.0f, -1.0f, 0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f,  0.5f), UV = new Vector2(1.0f, 1.0f), Normal = new Vector3(0.0f, -1.0f, 0.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f,  0.5f), UV = new Vector2(0.0f, 1.0f), Normal = new Vector3(0.0f, -1.0f, 0.0f) },
    };

    private static readonly uint[] _cubeIndices =
    {
        // Front face
        0, 1, 2,  2, 3, 0,
        // Back face
        4, 5, 6,  6, 7, 4,
        // Left face
        8, 9, 10, 10, 11, 8,
        // Right face
        12, 13, 14, 14, 15, 12,
        // Top face
        16, 17, 18, 18, 19, 16,
        // Bottom face
        20, 21, 22, 22, 23, 20
    };

    public static void Load(AssetLoader assets)
    {
        assets.LoadMesh("MeshQuad", _quadVertices, _quadIndices,
            new Mesh.SubMeshInfo[] { new() { Index = 0, Size = 6 } });
        assets.LoadMesh("MeshCube", _cubeVertices, _cubeIndices,
            new Mesh.SubMeshInfo[] { new() { Index = 0, Size = 36 } });

        Entity quadEntity = new Entity(null, "Quad");
        quadEntity.AddComponent(new StaticMeshComponent() { 
            Mesh = assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new LitTexturedMaterial(assets.GetTexture("Blank")) }
        });
        assets.LoadEntity("PrefabQuad", quadEntity);

        Entity cubeEntity = new Entity(null, "Cube");
        cubeEntity.AddComponent(new StaticMeshComponent()
        {
            Mesh = assets.GetMesh("MeshCube"),
            Materials = new List<Material>() { new LitTexturedMaterial(assets.GetTexture("Blank")) }
        });
        assets.LoadEntity("PrefabCube", cubeEntity);

        Vertex[] sphereV;
        uint[] sphereI;
        GenerateSphere(0.5f, 16, 32, out sphereV, out sphereI);
        assets.LoadMesh("MeshSphere", sphereV, sphereI,
            new Mesh.SubMeshInfo[] { new() { Index = 0, Size = sphereI.Length } });

        Entity sphereEntity = new Entity(null, "Sphere");
        sphereEntity.AddComponent(new StaticMeshComponent()
        {
            Mesh = assets.GetMesh("MeshSphere"),
            Materials = new List<Material>() { new LitTexturedMaterial(assets.GetTexture("Blank")) }
        });
        assets.LoadEntity("PrefabSphere", sphereEntity);
    }

    public static void GenerateSphere(float radius, int latitudeSegments, int longitudeSegments,
        out Vertex[] vertices, out uint[] indices)
    {
        List<Vertex> verticesL = new List<Vertex>();
        List<uint> indicesL = new List<uint>();
        
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * MathF.PI / latitudeSegments;
            float sinTheta = MathF.Sin(theta);
            float cosTheta = MathF.Cos(theta);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2.0f * MathF.PI / longitudeSegments;
                float sinPhi = MathF.Sin(phi);
                float cosPhi = MathF.Cos(phi);

                Vector3 normal = new Vector3(
                    cosPhi * sinTheta,
                    cosTheta,
                    sinPhi * sinTheta
                );

                Vector3 position = normal * radius;
                Vector2 uv = new Vector2(
                    (float)lon / longitudeSegments,
                    1f - (float)lat / latitudeSegments
                );

                verticesL.Add(new Vertex()
                {
                    Position = position,
                    UV = uv,
                    Normal = normal
                });
            }
        }

        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                uint first = (uint)(lat * (longitudeSegments + 1) + lon);
                uint second = first + (uint)longitudeSegments + 1;

                indicesL.Add(first);
                indicesL.Add(second);
                indicesL.Add(first + 1);

                indicesL.Add(second);
                indicesL.Add(second + 1);
                indicesL.Add(first + 1);
            }
        }

        vertices = verticesL.ToArray();
        indices = indicesL.ToArray();
    }
}