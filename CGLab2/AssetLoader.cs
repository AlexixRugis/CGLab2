
using System.Drawing;

public class AssetLoader
{
    private Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();
    private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
    private Dictionary<string, CubemapTexture> _cubemaps = new Dictionary<string, CubemapTexture>();
    private Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();
    private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

    public void LoadShader(string name, string vertexPath, string fragmentPath)
    {
        if (_shaders.ContainsKey(name)) throw new ArgumentException($"Shader {name} already exists.");

        Shader shader = new Shader(
            File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), vertexPath)),
            File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fragmentPath))
            );

        _shaders.Add(name, shader);
    }

    public Shader GetShader(string name)
    {
        if (_shaders.TryGetValue (name, out Shader? shader)) return shader;
        throw new InvalidOperationException();
    }

    public void LoadTexture(string name, string path, bool generateMipmaps)
    {
        if (_textures.ContainsKey(name)) throw new ArgumentException($"Texture {name} already exists.");

        Texture tex = new Texture(path, generateMipmaps);

        _textures.Add(name, tex);
    }

    public Texture GetTexture(string name)
    {
        if (_textures.TryGetValue(name, out Texture? tex)) return tex;
        throw new InvalidOperationException();
    }

    public void LoadCubemap(string name, string[] paths, bool generateMipmaps)
    {
        if (_cubemaps.ContainsKey(name)) throw new ArgumentException($"Cubemap {name} already exists.");

        Bitmap[] bitmaps = new Bitmap[6];
        for (int i = 0; i < 6; i++) bitmaps[i] = new System.Drawing.Bitmap(paths[i]);

        CubemapTexture tex = new CubemapTexture(bitmaps, generateMipmaps);

        _cubemaps.Add(name, tex);
    }

    public CubemapTexture GetCubemap(string name)
    {
        if (_cubemaps.TryGetValue(name, out CubemapTexture? tex)) return tex;
        throw new InvalidOperationException();
    }

    public void LoadMesh(string name, Vertex[] vertices, uint[] indices, Mesh.SubMeshInfo[] subMeshes)
    {
        if (_meshes.ContainsKey(name)) throw new ArgumentException($"Mesh {name} already exists.");

        Mesh mesh = new Mesh(vertices, indices, subMeshes);
        _meshes.Add(name, mesh);
    }

    public Mesh GetMesh(string name)
    {
        if (_meshes.TryGetValue(name, out Mesh? mesh)) return mesh;
        throw new InvalidOperationException();
    }

    public void LoadEntity(string name, string path, float scale = 0.01f)
    {
        if (_entities.ContainsKey(name)) throw new ArgumentException($"Entity {name} already exists.");

        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
        AssimpLoader loader = new AssimpLoader(this);

        Entity e = loader.Load(fullPath, "", scale);
        _entities.Add(name, e);
    }

    public void LoadEntity(string name, Entity entity)
    {
        if (_entities.ContainsKey(name)) throw new ArgumentException($"Entity {name} already exists.");

        if (entity.World != null) throw new ArgumentException("Can only add entities that are not attached to the world.");

        _entities.Add(name, entity);
    }

    public Entity GetEntity(string name)
    {
        if (_entities.TryGetValue(name, out Entity? entity)) return entity;
        throw new InvalidOperationException();
    }

    public void UnloadAll()
    {
        foreach (var kv in _shaders)
        {
            kv.Value.Dispose();
        }
        _shaders.Clear();

        foreach (var kv in _textures)
        {
            kv.Value.Dispose();
        }
        _textures.Clear();

        foreach (var kv in _cubemaps)
        {
            kv.Value.Dispose();
        }
        _cubemaps.Clear();
    }
}